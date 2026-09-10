(() => {
  "use strict";

  const shell = document.querySelector("[data-api-prefix]");
  const form = document.getElementById("reset-form");
  const status = document.getElementById("status");
  const submit = form.querySelector("button[type='submit']");
  const minimumPasswordLength = {{MINIMUM_PASSWORD_LENGTH}};
  const query = new URLSearchParams(window.location.search);
  const context = {
    challengeId: String(query.get("challenge") || "").trim(),
    returnUri: String(query.get("return_uri") || "").trim() || null,
    state: String(query.get("state") || "").trim() || null
  };
  const contextValid = context.challengeId;
  const messages = {
    "identity.verification_invalid": "The code is invalid, expired, or has already been used.",
    "identity.verification_exhausted": "Too many incorrect attempts. Request a new recovery code.",
    "identity.password_reuse": "Choose a password you have not used recently.",
    "identity.invalid_request": `Check the values and use a password of at least ${minimumPasswordLength} characters.`
  };

  const show = (message, kind) => {
    status.textContent = message;
    status.className = `status ${kind}`;
  };

  const readProblem = async (response) => {
    const problem = await response.json().catch(() => ({}));
    return messages[problem.code] || (response.status === 429
      ? "Too many attempts. Wait before trying again."
      : "Password recovery could not be completed.");
  };

  for (const toggle of form.querySelectorAll(".reveal")) {
    toggle.addEventListener("click", () => {
      const input = document.getElementById(toggle.dataset.target);
      if (!input) return;
      const revealed = input.type === "text";
      input.type = revealed ? "password" : "text";
      toggle.setAttribute("aria-pressed", String(!revealed));
    });
  }

  const newPasswordInput = document.getElementById("new-password");
  const confirmInput = document.getElementById("confirm-password");
  const matchHint = document.getElementById("confirm-help");
  const syncConfirmState = () => {
    const mismatch = confirmInput.value.length > 0 && confirmInput.value !== newPasswordInput.value;
    confirmInput.setCustomValidity(mismatch ? "Passwords do not match." : "");
    confirmInput.classList.toggle("field-invalid", mismatch);
    matchHint.textContent = mismatch ? "Passwords do not match." : "";
    matchHint.classList.toggle("hint-error", mismatch);
  };
  newPasswordInput.addEventListener("input", syncConfirmState);
  confirmInput.addEventListener("input", syncConfirmState);

  if (!contextValid) {
    submit.disabled = true;
    show("This recovery link is incomplete. Request a new password reset from your application.", "error");
  }

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!contextValid) return;
    show("", "");

    const data = new FormData(form);
    const code = String(data.get("code") || "").trim();
    const newPassword = String(data.get("newPassword") || "");
    const confirmation = String(data.get("confirmPassword") || "");
    if (!/^\d{4,12}$/.test(code) || newPassword.length < minimumPasswordLength) {
      show(`Enter the numeric code and a password of at least ${minimumPasswordLength} characters.`, "error");
      return;
    }
    if (newPassword !== confirmation) {
      show("The new password and confirmation do not match.", "error");
      return;
    }

    submit.disabled = true;
    try {
      const commonHeaders = {
        "Content-Type": "application/json",
        "Accept": "application/json",
        "X-Kida-Challenge": context.challengeId
      };
      const verification = await fetch(`${shell.dataset.apiPrefix}/password/resets/verify`, {
        method: "POST",
        credentials: "same-origin",
        headers: commonHeaders,
        body: JSON.stringify({ ...context, code })
      });
      if (!verification.ok) {
        show(await readProblem(verification), "error");
        return;
      }

      const grant = await verification.json();
      const completion = await fetch(`${shell.dataset.apiPrefix}/password/resets/complete`, {
        method: "POST",
        credentials: "same-origin",
        headers: { ...commonHeaders, "X-Kida-Challenge": grant.grantId },
        body: JSON.stringify({
          grantId: grant.grantId,
          newPassword,
          returnUri: context.returnUri,
          state: context.state
        })
      });
      if (!completion.ok) {
        show(await readProblem(completion), "error");
        return;
      }

      const receipt = await completion.json();
      form.reset();
      show("Password reset. Every existing session has been signed out.", "success");
      if (receipt.returnUri) {
        const destination = new URL(receipt.returnUri);
        destination.searchParams.set("kida_result", "password_reset");
        if (receipt.state) destination.searchParams.set("state", receipt.state);
        window.location.assign(destination.toString());
      }
    } catch {
      show("The identity service could not be reached. Try again shortly.", "error");
    } finally {
      submit.disabled = false;
    }
  });
})();

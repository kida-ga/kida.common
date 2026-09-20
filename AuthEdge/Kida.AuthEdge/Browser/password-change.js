(() => {
  "use strict";

  const shell = document.querySelector("[data-api-prefix]");
  const form = document.getElementById("password-form");
  const status = document.getElementById("status");
  const submit = form.querySelector("button[type='submit']");
  const minimumPasswordLength = {{MINIMUM_PASSWORD_LENGTH}};
  const query = new URLSearchParams(window.location.search);
  const returnUri = String(query.get("return_uri") || "").trim();
  const returnState = String(query.get("state") || "").trim();
  const messages = {
    "identity.invalid_credentials": "The username or current password is incorrect.",
    "identity.password_reuse": "Your new password must be different from your current password.",
    "identity.invalid_request": `Check the values and use a password of at least ${minimumPasswordLength} characters.`
  };

  const show = (message, kind) => {
    status.textContent = message;
    status.className = `status ${kind}`;
  };

  // Password visibility toggles.
  for (const toggle of form.querySelectorAll(".reveal")) {
    toggle.addEventListener("click", () => {
      const input = document.getElementById(toggle.dataset.target);
      if (!input) return;
      const revealed = input.type === "text";
      input.type = revealed ? "password" : "text";
      toggle.setAttribute("aria-pressed", String(!revealed));
    });
  }

  // Live confirmation-match feedback: mark the confirm field invalid and show
  // a hint as soon as the two entries diverge, without waiting for submit.
  const newPasswordInput = document.getElementById("new-password");
  const confirmInput = document.getElementById("confirm-password");
  const matchHint = document.getElementById("confirm-help");

  const syncConfirmState = () => {
    const mismatch = confirmInput.value.length > 0 && confirmInput.value !== newPasswordInput.value;
    confirmInput.setCustomValidity(mismatch ? "Passwords do not match." : "");
    confirmInput.classList.toggle("field-invalid", mismatch);
    if (matchHint) {
      matchHint.textContent = mismatch ? "Passwords do not match." : "";
      matchHint.classList.toggle("hint-error", mismatch);
    }
  };

  newPasswordInput.addEventListener("input", syncConfirmState);
  confirmInput.addEventListener("input", syncConfirmState);

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    show("", "");

    const data = new FormData(form);
    const username = String(data.get("username") || "").trim();
    const currentPassword = String(data.get("currentPassword") || "");
    const newPassword = String(data.get("newPassword") || "");
    const confirmation = String(data.get("confirmPassword") || "");
    if (!username || !currentPassword || newPassword.length < minimumPasswordLength) {
      show(`Complete every field and use a password of at least ${minimumPasswordLength} characters.`, "error");
      return;
    }
    if (newPassword !== confirmation) {
      show("The new password and confirmation do not match.", "error");
      return;
    }

    submit.disabled = true;
    try {
      const response = await fetch(`${shell.dataset.apiPrefix}/password/change`, {
        method: "POST",
        credentials: "same-origin",
        headers: {
          "Content-Type": "application/json",
          "Accept": "application/json",
          "X-Kida-Username": username
        },
        body: JSON.stringify({
          username,
          currentPassword,
          newPassword,
          returnUri: returnUri || null,
          state: returnState || null
        })
      });
      if (!response.ok) {
        const problem = await response.json().catch(() => ({}));
        show(problem.detail || messages[problem.code] || problem.title || (response.status === 429
          ? "Too many attempts. Wait a minute and try again."
          : "The password could not be changed. Try again."), "error");
        return;
      }

      const receipt = response.status === 204 ? null : await response.json().catch(() => null);
      form.reset();
      show("Password changed. Existing sessions were signed out.", "success");
      if (receipt?.returnUri) {
        const destination = new URL(receipt.returnUri);
        destination.searchParams.set("kida_result", "password_changed");
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

using Haley.Models;
using Haley.Utils;
using Kida.Tool.Models;

namespace Kida.Tool.Services;

internal static class EnvelopeProcessor
{
    internal static int Run(EnvelopeOptions options)
    {
        var context = ApplicationInfoReader.Read(options);
        var input = new DeploymentRequestInput
        {
            Product = context.Info.Product,
            ProductVersion = context.Info.ProductVersion,
            Features = context.Info.Features,
            Limits = context.Info.Limits,
            BaseDirectory = context.BaseDirectory,
            DeploymentInfoLocation = context.DeploymentInfoLocation
        };
        var result = options.Command switch
        {
            EnvelopeCommand.Request => DeploymentUtils.PrepareRequest(input),
            EnvelopeCommand.Renew => DeploymentUtils.RenewRequest(input),
            EnvelopeCommand.Show or EnvelopeCommand.Verify => DeploymentUtils.LoadRequest(input),
            _ => throw new ArgumentOutOfRangeException(nameof(options.Command))
        };
        if (!result.IsValid || result.Request is null || string.IsNullOrWhiteSpace(result.Envelope))
            throw new InvalidOperationException(result.Error + (string.IsNullOrWhiteSpace(result.Message) ? string.Empty : ": " + result.Message));

        Console.WriteLine(options.Command == EnvelopeCommand.Verify ? "VERIFIED" : "READY");
        Console.WriteLine("PRODUCT  " + result.Request.Product);
        Console.WriteLine("VERSION  " + result.Request.ProductVersion);
        Console.WriteLine("DEPLOY   " + result.Request.DeployId);
        Console.WriteLine("EVIDENCE lite=" + result.Request.MachineEvidence.Lite.Count + " strong=" + result.Request.MachineEvidence.Strong.Count);
        Console.WriteLine("OUTPUT   " + result.RequestPath);
        Console.WriteLine();
        Console.WriteLine("-----BEGIN KIDA DEPLOYMENT REQUEST-----");
        Console.WriteLine(result.Envelope);
        Console.WriteLine("-----END KIDA DEPLOYMENT REQUEST-----");
        return 0;
    }
}

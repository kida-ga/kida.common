using Kida.Tool.Models;
using Kida.Tool.Parsing;
using Kida.Tool.Services;

namespace Kida.Tool.Commands;

internal static class EnvelopeCli
{
    internal static int Run(string[] args)
    {
        try
        {
            var options = CommandLineParser.Parse(args);
            if (options.Command == EnvelopeCommand.Help)
            {
                ShowHelp();
                return 0;
            }

            return EnvelopeProcessor.Run(options);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("ERROR   " + exception.Message);
            Console.Error.WriteLine("Run 'kida-envelope --help' for usage.");
            return 1;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Kida Envelope Tool - prepare deployment requests without starting the product host");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  kida-envelope request [-appinfo <appinfo.json>] [-base <deployment-directory>]");
        Console.WriteLine("  kida-envelope renew  [-appinfo <appinfo.json>] [-base <deployment-directory>]");
        Console.WriteLine("  kida-envelope show   [-appinfo <appinfo.json>] [-base <deployment-directory>]");
        Console.WriteLine("  kida-envelope verify [-appinfo <appinfo.json>] [-base <deployment-directory>]");
        Console.WriteLine();
        Console.WriteLine("Defaults:");
        Console.WriteLine("  appinfo: ./appinfo.json");
        Console.WriteLine("  base:    directory containing appinfo.json");
        Console.WriteLine();
        Console.WriteLine("request creates a new .deployinfo identity when none exists.");
        Console.WriteLine("renew reuses the existing deployment ID and keypair.");
        Console.WriteLine("The request can be used to issue a term license and a deployment-bound feature entitlement.");
    }
}

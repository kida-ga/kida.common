using Kida.Tool.Models;

namespace Kida.Tool.Parsing;

internal static class CommandLineParser
{
    internal static EnvelopeOptions Parse(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
            return new EnvelopeOptions { Command = EnvelopeCommand.Help };

        var command = args[0].ToLowerInvariant() switch
        {
            "request" => EnvelopeCommand.Request,
            "renew" => EnvelopeCommand.Renew,
            "show" => EnvelopeCommand.Show,
            "verify" => EnvelopeCommand.Verify,
            _ => throw new ArgumentException("Unknown command '" + args[0] + "'.")
        };
        var appInfo = "appinfo.json";
        string? baseDirectory = null;
        for (var index = 1; index < args.Length; index++)
        {
            var option = args[index].ToLowerInvariant();
            if (option is "-appinfo" or "--appinfo")
            {
                appInfo = ReadValue(args, ref index, option);
                continue;
            }
            if (option is "-base" or "--base")
            {
                baseDirectory = ReadValue(args, ref index, option);
                continue;
            }
            throw new ArgumentException("Unknown option '" + args[index] + "'.");
        }

        return new EnvelopeOptions
        {
            Command = command,
            AppInfoPath = appInfo,
            BaseDirectory = baseDirectory
        };
    }

    private static string ReadValue(string[] args, ref int index, string option)
    {
        if (++index >= args.Length || string.IsNullOrWhiteSpace(args[index]))
            throw new ArgumentException("Option '" + option + "' requires a value.");
        return args[index];
    }
}

namespace Kida.Tool.Models;

internal sealed class EnvelopeOptions
{
    internal EnvelopeCommand Command { get; init; }
    internal string AppInfoPath { get; init; } = "appinfo.json";
    internal string? BaseDirectory { get; init; }
}

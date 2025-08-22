namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// Rich output object for Write-Rule when -PassThruObject is used.
    /// </summary>
    public sealed record RuleInfo(
        string Line,
        string? Title,
        int Width,
        char Char,
        bool Center,
        int Gap);
}

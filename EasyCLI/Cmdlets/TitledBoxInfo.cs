using System.Collections.Generic;

namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// Rich output object for Write-TitledBox when -PassThruObject is used.
    /// </summary>
    public sealed record TitledBoxInfo(
        string Title,
        IReadOnlyList<string> Lines,
        IReadOnlyList<string> BoxLines);
}

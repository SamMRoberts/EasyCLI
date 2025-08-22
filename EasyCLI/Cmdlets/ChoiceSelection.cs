namespace EasyCLI.Cmdlets;

/// <summary>
/// Strongly typed selection result for -PassThruObject in Read-Choice cmdlet.
/// </summary>
public sealed record ChoiceSelection(int Index, string Value);

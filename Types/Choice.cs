namespace ShopGame.Types;

internal readonly struct Choice
{
  public string What { get; init; }
  public string? Destination { get; init; }
  public ChoiceAction? Action { get; init; }
  public bool NoSnapshot { get; init; }
}

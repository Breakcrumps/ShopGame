namespace ShopGame.Types;

internal sealed class Choice
{
  public string What { get; init; } = "";
  public string? Destination { get; init; }
  public string? Action { get; init; }
  public bool NoSnapshot { get; init; }
}

namespace ShopGame.Types;

internal sealed record Choice(
  string What = "",
  string? Destination = null,
  string? Action = null,
  bool NoSnapshot = false
);

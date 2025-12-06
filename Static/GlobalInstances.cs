using ShopGame.Characters;
using ShopGame.UI;

namespace ShopGame.Static;

internal static class GlobalInstances
{
  internal static TextBox? TextBox { get; set; }
  internal static ShelfViewportContainer? ShelfViewportContainer { get; set; }

  internal static Girl? Player { get; set; }

  internal static HandSprite? HandSprite { get; set; }
}

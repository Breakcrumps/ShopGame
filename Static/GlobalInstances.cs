using ShopGame.Characters;
using ShopGame.Scenes.ToyShelf;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.UI.Textbox;

namespace ShopGame.Static;

internal static class GlobalInstances
{
  internal static TextBox? TextBox { get; set; }
  internal static ShelfViewportContainer? ShelfViewportContainer { get; set; }

  internal static Girl? Girl { get; set; }

  internal static HandSprite? HandSprite { get; set; }
}

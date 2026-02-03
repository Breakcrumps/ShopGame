using ShopGame.Characters;
using ShopGame.Characters.Fight.Girl;
using ShopGame.Scenes.FightScene;
using ShopGame.Scenes.ToyShelf;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.UI.Textbox;

namespace ShopGame.Static;

internal static class GlobalInstances
{
  internal static Textbox TextBox { get; set; } = null!;
  internal static ShelfViewportContainer ShelfViewportContainer { get; set; } = null!;

  internal static Girl Girl { get; set; } = null!;
  internal static FightGirl FightGirl { get; set; } = null!;

  internal static FightCameraRoot FightCameraRoot { get; set; } = null!;

  internal static HandSprite HandSprite { get; set; } = null!;
}

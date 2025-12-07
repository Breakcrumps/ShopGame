using Godot;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.UI;

[GlobalClass]
internal sealed partial class ShelfHandZone : HandZone
{
  [Export] internal int Row { get; private set; }
  [Export] internal int Pos { get; private set; }

  private protected override void HandleMouseEntered()
  {
    if (GlobalInstances.HandSprite.IfValid() is not HandSprite handSprite)
      return;

    base.HandleMouseEntered();
    handSprite.FocusedShelfPos = new ShelfPos(Row, Pos);
  }

  private protected override void HandleMouseExited()
  {
    if (GlobalInstances.HandSprite.IfValid() is not HandSprite handSprite)
      return;

    base.HandleMouseExited();
    handSprite.FocusedShelfPos = new ShelfPos(-1, -1);
  }
}

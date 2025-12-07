using Godot;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.Static;

namespace ShopGame.Scenes.ToyShelf.UI;

[GlobalClass]
internal partial class HandZone : Control
{
  [Export] internal TurnOrientation TurnOrientation { get; private set; }

  public override void _Ready()
  {
    MouseEntered += HandleMouseEntered;
    MouseExited += HandleMouseExited;
  }

  private protected virtual void HandleMouseEntered()
  {
    if (GlobalInstances.HandSprite.IfValid() is not HandSprite handSprite)
      return;

    handSprite.Scale = handSprite.InitScale * .7f;
    handSprite.InZone = true;
  }

  private protected virtual void HandleMouseExited()
  {
    if (GlobalInstances.HandSprite.IfValid() is not HandSprite handSprite)
      return;
    
    handSprite.Scale = handSprite.InitScale;
    handSprite.InZone = false;
  }
}

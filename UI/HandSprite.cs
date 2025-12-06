using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class HandSprite : Sprite2D
{
  internal bool InZone { get; set; }

  internal ShelfPos FocusedShelfPos { get; set; } = new(-1, -1);

  internal Vector2 InitScale { get; private set; }

  public override void _Ready()
  {
    GlobalInstances.HandSprite = this;
    InitScale = Scale;
  }
  
  public override void _Process(double delta)
  {
    Frame = Input.IsActionPressed("Grab") ? 1 : 0;
    
    Vector2 mousePos = GetViewport().GetMousePosition();
    
    Position = mousePos;
  }
}

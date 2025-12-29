using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.In2D;

[GlobalClass]
internal sealed partial class HandSprite : Sprite2D
{
  internal bool InZone { private get; set; }

  internal ShelfPos FocusedShelfPos { get; set; } = new(-1, -1);

  internal Vector2 InitScale { get; private set; }

  public override void _EnterTree()
    => GlobalInstances.HandSprite = this;

  public override void _ExitTree()
    => GlobalInstances.HandSprite = null;

  public override void _Ready()
    => InitScale = Scale;
  
  public override void _Process(double delta)
  {
    Frame = Input.IsActionPressed("Grab") ? 1 : 0;
    
    Vector2 mousePos = GetViewport().GetMousePosition();
    
    Position = mousePos;
  }
}

using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class HandSprite : Sprite2D
{
  [Export] private ShelfAreas? _shelfAreas;

  private Vector2 _initScale;

  public override void _Ready()
  {
    _initScale = Scale;
    
    if (
      !_shelfAreas.IsValid()
      || _shelfAreas!.ZoneGroup.IfValid() is not Control zoneGroup
    )
      return;

    foreach (Node child in zoneGroup.GetChildren())
    {
      if (child is not HandZone zone)
        continue;

      zone.MouseEntered += () => Scale = _initScale * .7f;
      zone.MouseExited += () => Scale = _initScale;
    }
  }
  
  public override void _Process(double delta)
  {
    Frame = Input.IsActionPressed("Grab") ? 1 : 0;
    
    Vector2 mousePos = GetViewport().GetMousePosition();
    
    Position = mousePos;
  }
}

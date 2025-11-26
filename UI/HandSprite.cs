using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class HandSprite : Sprite2D
{
  [Export] private ShelfAreas? _shelfAreas;

  internal bool InZone { get; private set; }
  internal bool InShelfZone { get; private set; }

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

      zone.HandEntered += zoneOrientation =>
      {
        Scale = _initScale * .7f;
        InZone = true;
        InShelfZone = zoneOrientation == TurnOrientation.Up;
      };
      zone.MouseExited += () =>
      {
        Scale = _initScale;
        InZone = false;
        InShelfZone = false;
      };
    }
  }
  
  public override void _Process(double delta)
  {
    Frame = Input.IsActionPressed("Grab") ? 1 : 0;
    
    Vector2 mousePos = GetViewport().GetMousePosition();
    
    Position = mousePos;
  }
}

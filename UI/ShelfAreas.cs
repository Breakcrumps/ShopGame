using Godot;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class ShelfAreas : CanvasLayer
{
  [Export] internal Control? ZoneGroup { get; private set; }
  [Export] internal Control? TurnAreaGroup { get; private set; }
}

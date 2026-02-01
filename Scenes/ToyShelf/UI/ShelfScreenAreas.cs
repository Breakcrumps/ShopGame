using Godot;

namespace ShopGame.Scenes.ToyShelf.UI;

[GlobalClass]
internal sealed partial class ShelfScreenAreas : CanvasLayer
{
  [Export] internal Control ZoneGroup { get; private set; } = null!;
  [Export] internal Control TurnAreaGroup { get; private set; } = null!;
}

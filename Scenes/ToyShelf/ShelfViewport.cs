using Godot;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class ShelfViewport : SubViewport
{
  [Export] internal ShelfCamera? ShelfCamera { get; private set; }
}

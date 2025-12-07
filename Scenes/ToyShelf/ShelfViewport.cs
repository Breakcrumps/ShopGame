using Godot;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.Scenes.ToyShelf.In3D;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewport : SubViewport
{
  [Export] internal ShelfCamera? ShelfCamera { get; private set; }
  [Export] internal ShelfPosGroup? ShelfPosGroup { get; private set; }
}

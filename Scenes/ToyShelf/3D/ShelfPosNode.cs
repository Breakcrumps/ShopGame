using Godot;
using ShopGame.Static;
using ShopGame.UI;

namespace ShopGame.World;

[GlobalClass]
internal sealed partial class ShelfPosNode : Node3D
{
  [Export] internal int Row { get; private set; }
  [Export] internal int Pos { get; private set; }
  
  private BoxItem? _heldItem;

  internal void PutItem(BoxItem newItem)
  {
    if (_heldItem.IsValid())
      _heldItem!.ReturnToInitPos = true;

    _heldItem = newItem;
    newItem.GlobalPosition = GlobalPosition;
    newItem.Scale = .8f * Vector3.One;
  }
}

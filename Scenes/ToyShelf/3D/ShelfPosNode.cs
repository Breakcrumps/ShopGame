using Godot;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.UI;

namespace ShopGame.World;

[GlobalClass]
internal sealed partial class ShelfPosNode : Node3D
{
  [Export] internal int Row { get; private set; }
  [Export] internal int Pos { get; private set; }

  [Export] private AnimationPlayer? _animPlayer;
  [Export] private Sprite3D? _hoverSprite;
  
  private BoxItem? _heldItem;

  public override void _Ready()
  {
    if (!_hoverSprite.IsValid())
      return;
    
    _hoverSprite!.Visible = false;
  }

  internal void StartHover()
  {
    if (!_hoverSprite.IsValid())
      return;

    _hoverSprite!.Visible = true;
    _animPlayer?.Play("Hover");
  }

  internal void StopHover()
  {
    if (!_hoverSprite.IsValid())
      return;
    
    _hoverSprite!.Visible = false;
    _animPlayer?.Stop();
  }

  internal void PutItem(BoxItem newItem)
  {
    if (_heldItem.IsValid())
      _heldItem!.ReturnToInitPos = true;

    _heldItem = newItem;
    newItem.GlobalPosition = GlobalPosition;
    newItem.Scale = .8f * Vector3.One;
  }

  internal bool PosEqualsTo(ShelfPos shelfPos)
    => Row == shelfPos.Row && Pos == shelfPos.Pos;
}

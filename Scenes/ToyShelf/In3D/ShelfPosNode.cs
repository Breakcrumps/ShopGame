using Godot;
using ShopGame.Scenes.ToyShelf.Toys;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.In3D;

[GlobalClass]
internal sealed partial class ShelfPosNode : Node3D
{
  [Export] internal int Row { get; private set; }
  [Export] internal int Pos { get; private set; }

  [Export] private AnimationPlayer? _animPlayer;
  [Export] private Sprite3D? _hoverSprite;
  
  internal BoxItem? HeldItem { get; set; }

  private ShelfPosGroup? _posGroup;

  public override void _Ready()
  {
    _posGroup = GetParent<ShelfPosGroup>();
    
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
    if (!_posGroup!.ShelfViewport.IsValid())
      return;
    
    if (HeldItem.IsValid())
      HeldItem!.ReturnToInitPos = true;

    newItem.GlobalPosition = GlobalPosition;
    newItem.Scale = .8f * Vector3.One;
    newItem.AssociatedPosNode = this;

    HeldItem = newItem;

    _posGroup.ShelfViewport!.BoxItems.Remove(newItem);
  }

  internal void DestroyItem()
  {
    if (HeldItem is null)
      return;

    HeldItem.QueueFree();
    HeldItem = null;
  }

  internal bool PosEqualsTo(ShelfPos shelfPos)
    => Row == shelfPos.Row && Pos == shelfPos.Pos;
}

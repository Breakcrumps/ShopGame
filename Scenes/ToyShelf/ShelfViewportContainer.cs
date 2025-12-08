using System.Collections.Generic;
using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewportContainer : SubViewportContainer
{
  [Export] private ShelfViewport? _shelfViewport;
  [Export] private CanvasLayer? _ui;
  
  public override void _Ready()
  {
    Visible = false;
    GlobalInstances.ShelfViewportContainer = this;
    ProcessMode = ProcessModeEnum.Disabled;
  }

  internal void Activate(Dictionary<int, BoxItemType> boxItems)
  {
    if (!_ui.IsValid())
      return;
    
    Visible = true;
    _ui!.ProcessMode = ProcessModeEnum.Disabled;
    ProcessMode = ProcessModeEnum.Always;
    Input.MouseMode = Input.MouseModeEnum.Hidden;

    if (!_shelfViewport.IsValid())
      return;
    
    _shelfViewport!.ShelfCamera?.Reset();
    _shelfViewport.ShelfPosGroup?.DiscardItems();
    _shelfViewport.ShelfPosGroup?.StockItems(boxItems);
  }

  internal void Deactivate()
  {
    if (!_ui.IsValid())
      return;

    Visible = false;
    _ui!.ProcessMode = ProcessModeEnum.Always;
    ProcessMode = ProcessModeEnum.Disabled;
    Input.MouseMode = Input.MouseModeEnum.Visible;
  }

  public override void _Input(InputEvent @event)
  {
    if (!@event.IsActionPressed("Interact"))
      return;

    Deactivate();

    AcceptEvent();
  }
}

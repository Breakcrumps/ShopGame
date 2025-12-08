using Godot;
using ShopGame.Scenes.Shop;
using ShopGame.Static;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewportContainer : SubViewportContainer
{
  [Export] private ShelfViewport? _shelfViewport;
  [Export] private CanvasLayer? _ui;

  private Shelf? _currentShelf;
  
  public override void _Ready()
  {
    Visible = false;
    GlobalInstances.ShelfViewportContainer = this;
    ProcessMode = ProcessModeEnum.Disabled;
  }

  internal void Activate(Shelf shelf)
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
    _shelfViewport.ShelfPosGroup?.StockItems(shelf.ItemsOnShelf);
    _shelfViewport.DespawnToysInBox();
    _shelfViewport.SpawnToysInBox();

    _currentShelf = shelf;
  }

  internal void Deactivate()
  {
    if (!_ui.IsValid())
      return;

    Visible = false;
    _ui!.ProcessMode = ProcessModeEnum.Always;
    ProcessMode = ProcessModeEnum.Disabled;
    Input.MouseMode = Input.MouseModeEnum.Visible;

    if (!_shelfViewport.IsValid() || !_shelfViewport!.ShelfPosGroup.IsValid())
      return;

    if (!_currentShelf.IsValid())
      return;

    _currentShelf!.ItemsOnShelf = _shelfViewport!.ShelfPosGroup!.GetItems();
  }

  public override void _Input(InputEvent @event)
  {
    if (!@event.IsActionPressed("Interact"))
      return;

    Deactivate();

    AcceptEvent();
  }
}

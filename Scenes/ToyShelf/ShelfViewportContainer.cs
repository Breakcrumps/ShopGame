using Godot;
using ShopGame.Scenes.Shop;
using ShopGame.Static;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewportContainer : SubViewportContainer
{
  [Export] private Vector2I _subviewportSize = new(990, 540);
  
  [Export] private PackedScene? _shelfVPScene;
  [Export] private CanvasLayer? _ui;

  private ShelfViewport? _shelfViewport;
  private Shelf? _currentShelf;

  public override void _EnterTree()
    => GlobalInstances.ShelfViewportContainer = this;

  public override void _ExitTree()
    => GlobalInstances.ShelfViewportContainer = null;
  
  public override void _Ready()
  {
    Visible = false;
    ProcessMode = ProcessModeEnum.Disabled;
  }

  internal void Activate(Shelf shelf)
  {
    if (_shelfVPScene is null || !_ui.IsValid())
      return;

    if (!_shelfViewport.IsValid())
    {
      _shelfViewport = _shelfVPScene.Instantiate<ShelfViewport>();
      _shelfViewport.Size = _subviewportSize;
      AddChild(_shelfViewport);
    }
    
    _shelfViewport.ShelfCamera?.Reset();
    _shelfViewport.ShelfPosGroup?.DiscardItems();
    _shelfViewport.ShelfPosGroup?.StockItems(shelf.ItemsOnShelf);
    _shelfViewport.DespawnToysInBox();
    _shelfViewport.SpawnToysInBox();

    _currentShelf = shelf;

    Callable.From(FinaliseActivation).CallDeferred();

    void FinaliseActivation()
    {
      Visible = true;
      _ui.ProcessMode = ProcessModeEnum.Disabled;
      ProcessMode = ProcessModeEnum.Always;
      Input.MouseMode = Input.MouseModeEnum.Hidden;
      GrabFocus();
    }
  }

  internal void Deactivate()
  {
    if (!_ui.IsValid())
      return;

    if (!_shelfViewport.IsValid() || !_shelfViewport.ShelfPosGroup.IsValid())
      return;

    if (!_currentShelf.IsValid())
      return;

    Visible = false;
    _ui.ProcessMode = ProcessModeEnum.Always;
    ProcessMode = ProcessModeEnum.Disabled;
    Input.MouseMode = Input.MouseModeEnum.Visible;

    _currentShelf.ItemsOnShelf = _shelfViewport.ShelfPosGroup.GetItems();
  }

  public override void _Input(InputEvent @event)
  {
    if (!@event.IsActionPressed("Interact"))
      return;

    Deactivate();
    AcceptEvent();
  }
}

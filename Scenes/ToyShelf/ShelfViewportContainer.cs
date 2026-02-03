using Godot;
using ShopGame.Scenes.Shop;
using ShopGame.Static;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewportContainer : SubViewportContainer
{
  [Export] private Vector2I _subviewportSize = new(990, 540);
  [Export] private CanvasLayer _ui = null!;
  
  private ShelfViewport _shelfViewport = null!;
  private Shelf? _currentShelf;

  public override void _EnterTree()
    => GlobalInstances.ShelfViewportContainer = this;
  
  public override void _Ready()
  {
    Visible = false;
    ProcessMode = ProcessModeEnum.Disabled;
    _shelfViewport = GetChild<ShelfViewport>(0);
    _shelfViewport.ProcessMode = ProcessModeEnum.Disabled;
    _shelfViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled;
  }

  internal void Activate(Shelf shelf)
  {
    _shelfViewport.ProcessMode = ProcessModeEnum.Always;
    _shelfViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
    _shelfViewport.ShelfCamera.Reset();
    _shelfViewport.ShelfPosGroup.DiscardItems();
    _shelfViewport.ShelfPosGroup.StockItems(shelf.ItemsOnShelf);
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
    if (!_currentShelf.IsValid())
      return;

    Visible = false;
    _ui.ProcessMode = ProcessModeEnum.Always;
    ProcessMode = ProcessModeEnum.Disabled;
    Input.MouseMode = Input.MouseModeEnum.Visible;

    _currentShelf.ItemsOnShelf = _shelfViewport.ShelfPosGroup.GetItems();

    _shelfViewport.ProcessMode = ProcessModeEnum.Disabled;
    _shelfViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled;
  }

  public override void _Input(InputEvent @event)
  {
    if (!@event.IsActionPressed("Interact"))
      return;

    Deactivate();
    AcceptEvent();
  }
}

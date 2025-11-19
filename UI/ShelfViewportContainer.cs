using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

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

  internal void Activate()
  {
    if (!_ui.IsValid())
      return;
    
    Visible = true;
    _ui!.ProcessMode = ProcessModeEnum.Disabled;
    ProcessMode = ProcessModeEnum.Always;

    if (!_shelfViewport.IsValid())
      return;
    
    _shelfViewport!.ShelfCamera?.ResetRotation();
  }

  internal void Deactivate()
  {
    if (!_ui.IsValid())
      return;

    Visible = false;
    _ui!.ProcessMode = ProcessModeEnum.Always;
    ProcessMode = ProcessModeEnum.Disabled;
  }

  public override void _Input(InputEvent @event)
  {
    if (!@event.IsActionPressed("Interact"))
      return;

    Deactivate();

    AcceptEvent();
  }
}

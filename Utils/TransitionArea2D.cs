using Godot;
using ShopGame.Characters;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class TransitionArea2D : Area2D, IActionHandler
{
  [Export] private string? _destinationPath;
  [Export] private Prompt2D? _prompt;

  private bool _enabled;

  public override void _Ready()
  {
    CollisionLayer = 4;
    CollisionMask = 1;
    
    BodyEntered += node =>
    {
      if (node is not Girl)
        return;

      _enabled = true;
      _prompt?.Activate();
    };

    BodyExited += node =>
    {
      if (node is not Girl)
        return;

      _enabled = false;
      _prompt?.Deactivate();
    };
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!_enabled)
      return;

    if (_destinationPath is null)
      return;

    if (!@event.IsActionPressed("Interact"))
      return;

    GetTree().CallDeferred("change_scene_to_file", _destinationPath);
  }

  public void HandleAction(string actionName)
  {
    if (_destinationPath is null)
      return;
    
    GetTree().CallDeferred("change_scene_to_file", _destinationPath);
  }
}

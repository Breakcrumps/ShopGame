using Godot;
using ShopGame.Characters.Fight.Girl;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class TransitionArea3D : Area3D
{
  [Export] private string? _destinationPath;
  [Export] private Prompt3D _prompt = null!;

  private bool _enabled;

  public override void _Ready()
  {
    CollisionLayer = 4;
    CollisionMask = 1;
    
    BodyEntered += node =>
    {
      if (node is not FightGirl)
        return;

      _enabled = true;
      _prompt.Activate();
    };

    BodyExited += node =>
    {
      if (node is not FightGirl)
        return;

      _enabled = false;
      _prompt.Deactivate();
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
}

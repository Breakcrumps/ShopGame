using Godot;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class TransitionButton : Button
{
  [Export] private string? _destinationPath;

  public override void _Ready()
    => Pressed += () => { GetTree().CallDeferred("change_scene_to_file", _destinationPath!); };
}

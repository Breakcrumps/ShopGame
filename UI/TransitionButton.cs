using Godot;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class TransitionButton : Button
{
  [Export] private PackedScene? _transitionTo;

  public override void _Ready()
    => Pressed += () => { GetTree().ChangeSceneToPacked(_transitionTo);; };
}

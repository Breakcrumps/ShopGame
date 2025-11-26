using Godot;
using ShopGame.Characters;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class TransitionArea : Area2D
{
  [Export] private PackedScene? _transitionTo;
  [Export] private Prompt? _prompt;

  public override void _Ready()
    => BodyEntered += node =>
    {
      if (node is not Girl)
        return;

      GetTree().ChangeSceneToPacked(_transitionTo);
    };
}

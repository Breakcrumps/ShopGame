using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.UI;

namespace ShopGame.World;

[GlobalClass]
internal sealed partial class DialogueInitArea : Area2D
{
  [Export] private int _consequentVariants = 1;
  
  private Node2D? _sceneRoot;
  [Export] private Node2D? _parent;
  [Export] private Prompt? _prompt;

  private int _currentVariant = 1;

  private bool _awaitInput;

  public override void _Ready()
  {
    if (GetTree().CurrentScene is not Node2D scene2D)
      return;

    _sceneRoot = scene2D;

    BodyEntered += body =>
    {
      if (body is not Player)
      return;

      _prompt?.Activate();
      _awaitInput = true;
    };

    BodyExited += body =>
    {
      if (body is not Player)
      return;

      _prompt?.Deactivate();
      _awaitInput = false;
    };
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!_awaitInput)
      return;

    if (!@event.IsActionPressed("interact"))
      return;

    if (!_parent.IsValid() || !_sceneRoot.IsValid())
      return;

    if (GlobalInstances.TextBox.IfValid() is not TextBox textBox)
      return;

    textBox.Activate(_sceneRoot!.Name, $"{_parent!.Name}{_currentVariant++}");

    if (_currentVariant > _consequentVariants)
      _currentVariant = _consequentVariants;
  }
}

using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.UI.Textbox;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class DialogueInitArea : Area2D
{  
  private Node2D? _sceneRoot;
  [Export] private Node? _parent;
  [Export] private Prompt? _prompt;

  private int _variantCount;
  private int _currentVariant = 1;

  private bool _awaitInput;

  public override void _Ready()
  {
    if (GetTree().CurrentScene.IfValid() is not Node2D scene2D)
      return;

    if (!GlobalInstances.TextBox.IsValid())
      return;

    if (!_parent.IsValid())
      return;

    _sceneRoot = scene2D;

    _variantCount = GlobalInstances.TextBox!.CountVariants(scene2D, _parent!);

    BodyEntered += body =>
    {
      if (body is not Girl)
      return;

      _prompt?.Activate();
      _awaitInput = true;
    };

    BodyExited += body =>
    {
      if (body is not Girl)
      return;

      _prompt?.Deactivate();
      _awaitInput = false;
    };
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!_awaitInput)
      return;

    if (!@event.IsActionPressed("Interact"))
      return;

    if (!_parent.IsValid() || !_sceneRoot.IsValid())
      return;

    if (GlobalInstances.TextBox.IfValid() is not TextBox textBox)
      return;

    textBox.Activate(_sceneRoot!, _parent!, _currentVariant);

    if (_currentVariant < _variantCount)
      _currentVariant++;
  }
}

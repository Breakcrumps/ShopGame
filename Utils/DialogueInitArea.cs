using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.UI.Textbox;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class DialogueInitArea : Area2D
{
  [Export] private Node? _parent;
  [Export] private Prompt? _prompt;

  private int _variantCount;
  private int _currentVariant = 1;

  internal bool AwaitInput { private get; set; }

  public override void _Ready()
  {
    if (!GlobalInstances.TextBox.IsValid())
      return;

    if (!_parent.IsValid())
      return;

    _variantCount = GlobalInstances.TextBox!.CountVariants(_parent!);

    BodyEntered += body =>
    {
      if (body is not (Girl or FieldGirl))
        return;

      AwaitInput = true;
    };

    BodyExited += body =>
    {
      if (body is not (Girl or FieldGirl))
        return;

      AwaitInput = false;
    };
  }

  public override void _Process(double delta)
  {
    if (!AwaitInput)
    {
      _prompt?.Deactivate();
      return;
    }

    if (_prompt?.IsActive() ?? true)
      return;

    _prompt.Activate();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!AwaitInput)
      return;

    if (!@event.IsActionPressed("Interact"))
      return;

    if (!_parent.IsValid())
      return;

    if (GlobalInstances.TextBox.IfValid() is not TextBox textBox)
      return;

    AwaitInput = false;
    textBox.Activate(_parent!, this, _currentVariant);

    if (_currentVariant < _variantCount)
      _currentVariant++;
  }
}

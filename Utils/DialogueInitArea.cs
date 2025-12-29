using Godot;
using ShopGame.Characters;
using ShopGame.Characters.Fight;
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
    CollisionLayer = 4;
    CollisionMask = 1;
    
    if (!GlobalInstances.TextBox.IsValid())
      return;

    if (!_parent.IsValid())
      return;

    _variantCount = GlobalInstances.TextBox.CountVariants(_parent);

    BodyEntered += body =>
    {
      if (body is not (Girl or FieldGirl or FightGirl))
        return;

      AwaitInput = true;
    };

    BodyExited += body =>
    {
      if (body is not (Girl or FieldGirl or FightGirl))
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

    if (!_prompt.IsValid() || _prompt.IsActive())
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

    if (GlobalInstances.TextBox.IfValid() is not Textbox textBox)
      return;

    AwaitInput = false;
    textBox.Activate(_parent, this, _currentVariant);

    if (_currentVariant < _variantCount)
      _currentVariant++;
  }
}

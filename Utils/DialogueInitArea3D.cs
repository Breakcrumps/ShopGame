using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Static;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class DialogueInitArea3D : Area3D, IDialogueInitArea
{
  [Export] private Node _parent = null!;
  [Export] private Prompt _prompt = null!;

  private int _variantCount;
  private int _currentVariant = 1;

  public bool AwaitInput { private get; set; }

  public override void _Ready()
  {
    CollisionLayer = 4;
    CollisionMask = 1;

    _variantCount = GlobalInstances.TextBox.CountVariants(_parent);

    BodyEntered += body =>
    {
      if (body is FightGirl)
        AwaitInput = true;
    };

    BodyExited += body =>
    {
      if (body is FightGirl)
        AwaitInput = false;
    };
  }

  public override void _Process(double delta)
  {
    if (!AwaitInput)
    {
      _prompt.Deactivate();
      return;
    }

    if (_prompt.IsActive())
      return;

    _prompt.Activate();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!AwaitInput)
      return;

    if (!@event.IsActionPressed("Interact"))
      return;

    AwaitInput = false;
    GlobalInstances.TextBox.Activate(_parent, this, _currentVariant);

    if (_currentVariant < _variantCount)
      _currentVariant++;
  }
}

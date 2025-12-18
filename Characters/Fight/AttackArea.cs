using System;
using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class AttackArea : Area2D
{
  private enum AttackDirection { Up, Down, Left, Right }
  [Export] private AttackDirection _attackDirection;

  [Export] private FightGirl? _fightGirl;
  [Export] private CollisionShape2D? _collider;

  private float _timeLeftInAttack;
  
  public override void _Ready()
  {
    if (!_collider.IsValid())
      return;
    
    _collider.Disabled = true;
    
    BodyEntered += TryHit;
    AreaEntered += TryHit;
  }

  private void TryHit(Node2D node)
  {
    if (node is not IHitProcessor hitProcessor)
      return;

    if (!_fightGirl.IsValid())
      return;

    Attack attack = new()
    {
      Strength = _fightGirl.AttackStrength,
      PushbackMagnitude = _fightGirl.PushbackMagnitude,
      Attacker = _fightGirl
    };

    hitProcessor.ProcessHit(attack);
    
    Vector2 pushbackDirection = _attackDirection switch
    {
      AttackDirection.Up => Vector2.Zero,
      AttackDirection.Down => Vector2.Up,
      AttackDirection.Left => Vector2.Right,
      AttackDirection.Right => Vector2.Left,
      _ => Vector2.Zero
    };

    _fightGirl.HandleOwnAttackPushback(pushbackDirection, pogo: _attackDirection is AttackDirection.Down);

    StopAttack();
  }


  public override void _PhysicsProcess(double delta)
  {
    if (!_collider.IsValid())
      return;

    if (!_fightGirl.IsValid())
      return;

    if (
      _timeLeftInAttack == 0f
      && Input.IsActionJustPressed("Attack")
      && NeededDirectionPressed()
      && !_fightGirl.InAttack
    )
    {
      _timeLeftInAttack = _fightGirl.AttackDuration;
      _fightGirl.InAttack = true;
      _collider.Disabled = false;
      return;
    }

    if (_timeLeftInAttack <= 0f)
    {
      if (_collider.Disabled)
        return;
      
      StopAttack();
      return;
    }

    _timeLeftInAttack -= (float)delta;
  }

  private bool NeededDirectionPressed() => _attackDirection switch
  {
    AttackDirection.Down => Input.IsActionPressed(Enum.GetName(_attackDirection)!),
    _ => Input.IsActionPressed(Enum.GetName(_attackDirection)!) && NothingElsePressed()
  };

  private bool NothingElsePressed()
  {
    for (int i = 0; i <= (int)AttackDirection.Right; i++)
    {
      if (i == (int)_attackDirection)
        continue;

      if (Input.IsActionPressed(Enum.GetName((AttackDirection)i)!))
        return false;
    }

    return true;
  }

  private void StopAttack()
  {
    _timeLeftInAttack = 0f;
    _collider!.SetDeferred("disabled", true);
    _fightGirl!.InAttack = false;
  }
}

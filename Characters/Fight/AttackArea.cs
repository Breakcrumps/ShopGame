using System;
using Godot;
using ShopGame.Characters.Fight.Enemies;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class AttackArea : Area2D
{
  private enum AttackDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }
  [Export] private AttackDirection _attackDirection;

  [Export] private FightGirl? _fightGirl;
  [Export] private CollisionShape2D? _collider;

  private float _timeLeftInAttack;
  
  public override void _Ready()
  {
    if (!_collider.IsValid())
      return;
    
    _collider.Disabled = true;
    
    BodyEntered += node =>
    {
      if (node is not Enemy enemy)
        return;

      if (!_fightGirl.IsValid())
        return;

      Attack attack = new()
      {
        Strength = _fightGirl.AttackStrength,
        PushbackMagnitude = _fightGirl.PushbackMagnitude,
        Attacker = _fightGirl
      };

      enemy.ProcessHit(attack);
      
      Vector2 pushbackDirection = _attackDirection switch
      {
        AttackDirection.Up => Vector2.Down,
        AttackDirection.Down => Vector2.Up,
        AttackDirection.Left => Vector2.Right,
        AttackDirection.Right => Vector2.Left,
        AttackDirection.UpLeft => -Vector2.One,
        AttackDirection.UpRight => new Vector2(-1, 1),
        AttackDirection.DownLeft => new Vector2(1, -1),
        AttackDirection.DownRight => Vector2.One,
        _ => Vector2.Zero
      };

      bool pogo = (
        _attackDirection is AttackDirection.Down
        or AttackDirection.DownLeft
        or AttackDirection.DownRight
      );

      _fightGirl.HandlePushback(pushbackDirection, pogo);

      StopAttack();
    };
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
      && IsDirectionPressed()
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

  private bool IsDirectionPressed() => _attackDirection switch
  {
    AttackDirection.UpLeft => Input.IsActionPressed("Up") && Input.IsActionPressed("Left"),
    AttackDirection.UpRight => Input.IsActionPressed("Up") && Input.IsActionPressed("Right"),
    AttackDirection.DownLeft => Input.IsActionPressed("Down") && Input.IsActionPressed("Left"),
    AttackDirection.DownRight => Input.IsActionPressed("Down") && Input.IsActionPressed("Right"),
    _ => Input.IsActionPressed(Enum.GetName(_attackDirection)!) && NothingElsePressed()
  };

  private bool NothingElsePressed()
  {
    for (int i = 0; i < (int)AttackDirection.UpLeft; i++)
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

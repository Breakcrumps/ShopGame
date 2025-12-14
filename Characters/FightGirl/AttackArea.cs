using System;
using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Characters.FightGirl;

[GlobalClass]
internal sealed partial class AttackArea : Area2D
{
  private enum AttackDirection { Up, Down, Left, Right }
  [Export] private AttackDirection _attackDirection;

  [Export] private float _attackDuration = .15f;
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
      ProcessAttackPushback(attack.PushbackMagnitude, enemy.GlobalPosition);
    };
  }

  private void ProcessAttackPushback(float pushbackMagnitude, Vector2 enemyPos)
  {
    Vector2 pushbackDirection = _fightGirl!.GlobalPosition - enemyPos;    
    _fightGirl.Velocity = pushbackDirection.Normalized() * pushbackMagnitude;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_collider.IsValid())
      return;
    
    if (
      _timeLeftInAttack == 0f
      && Input.IsActionJustPressed("Attack")
      && Input.IsActionPressed(Enum.GetName(_attackDirection)!)
    )
      _timeLeftInAttack = _attackDuration;

    if (_timeLeftInAttack <= 0f)
    {
      _timeLeftInAttack = 0f;
      _collider.Disabled = true;
      return;
    }

    _collider.Disabled = false;
    _timeLeftInAttack -= (float)delta;
  }
}

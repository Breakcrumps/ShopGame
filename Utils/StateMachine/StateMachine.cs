using System;
using System.Collections.Generic;
using Godot;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class StateMachine : Node
{
  internal State CurrentState { get; private set; } = null!;
  internal Dictionary<Type, State> States { get; } = [];
  [Export] private State _initState = null!;

  public override void _Ready()
  {
    foreach (Node child in GetChildren())
    {
      if (child is not State state)
        continue;

      States[state.GetType()] = state;

      state.SetPhysicsProcess(false);
      state.StateMachine = this;
    }

    Transition(_initState.GetType());
  }

  internal void Process(double delta)
    => CurrentState.Process(delta);

  internal void PhysicsProcess(double delta)
    => CurrentState.PhysicsProcess(delta);

  private void Transition(Type nextType)
  {
    if (!States.TryGetValue(nextType, out State? nextState))
      return;

    CurrentState?.Exit();
    CurrentState = nextState;
    CurrentState.Enter();
  }

  internal void Transition<T>() where T : State
    => Transition(typeof(T));
}

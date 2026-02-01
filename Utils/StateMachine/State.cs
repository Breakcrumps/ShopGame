using Godot;

namespace ShopGame.Utils;

[GlobalClass]
internal abstract partial class State : Node
{
  internal StateMachine StateMachine { private protected get; set; } = null!;
  
  internal virtual void Enter() { }
  internal virtual void Exit() { }

  internal virtual void Process(double delta) { }
  internal virtual void PhysicsProcess(double delta) { }
}

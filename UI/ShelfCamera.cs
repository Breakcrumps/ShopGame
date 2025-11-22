using System;
using System.Collections.Generic;
using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class ShelfCamera : Camera3D
{
  [Export] private ShelfAreas? _shelfAreas;
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private RayCast3D? _raycast;

  private PhysicsBody3D? _focusedItem;
  
  internal Vector3 InitRotation;

  private TurnOrientation _turnOrientation;

  private readonly Dictionary<TurnOrientation, ShelfTurnArea?> _shelfTurnAreas = new()
  {
    [TurnOrientation.Up] = null,
    [TurnOrientation.Down] = null
  };

  private readonly Dictionary<TurnOrientation, HandZone?> _handZones = new()
  {
    [TurnOrientation.Up] = null,
    [TurnOrientation.Down] = null
  };
  
  public override void _Ready()
  {
    InitRotation = GlobalRotation;

    if (!_shelfAreas.IsValid()
      || _shelfAreas!.TurnAreaGroup.IfValid() is not Control turnAreaGroup
      || _shelfAreas!.ZoneGroup.IfValid() is not Control handZoneGroup
    )
      return;
    
    foreach (Node child in turnAreaGroup.GetChildren())
    {
      if (child is not ShelfTurnArea shelfTurnArea)
        continue;

      _shelfTurnAreas[shelfTurnArea.FlickDirection] = shelfTurnArea;

      if (shelfTurnArea.FlickDirection == _turnOrientation)
        shelfTurnArea.MouseFilter = Control.MouseFilterEnum.Ignore;

      shelfTurnArea.RequestingTurn += TryTurn;
    }

    foreach (Node child in handZoneGroup.GetChildren())
    {
      if (child is not HandZone handZone)
        return;

      _handZones[handZone.TurnOrientation] = handZone;

      if (handZone.TurnOrientation != _turnOrientation)
        handZone.MouseFilter = Control.MouseFilterEnum.Ignore;
    }
  }

  private void TryTurn(TurnOrientation orientation)
  {
    if (orientation == _turnOrientation)
      return;
    
    _turnOrientation = orientation;
    
    UpdateTurnAreas();
    UpdateHandZones();

    if (orientation == TurnOrientation.Up)
      _animPlayer?.PlayBackwards("Turn");
    if (orientation == TurnOrientation.Down)
      _animPlayer?.Play("Turn");
  }

  private void UpdateTurnAreas()
  {
    if (_shelfTurnAreas[_turnOrientation] is ShelfTurnArea usedTurnArea)
      usedTurnArea.MouseFilter = Control.MouseFilterEnum.Ignore;
    if (_shelfTurnAreas[1 - _turnOrientation] is ShelfTurnArea nextTurnArea)
      nextTurnArea.MouseFilter = Control.MouseFilterEnum.Stop;
  }

  private void UpdateHandZones()
  {
    if (_handZones[_turnOrientation] is HandZone currentZone)
      currentZone.MouseFilter = Control.MouseFilterEnum.Stop;
    if (_handZones[1 - _turnOrientation] is HandZone pastZone)
      pastZone.MouseFilter = Control.MouseFilterEnum.Ignore;
  }

  internal void Reset()
  {
    GlobalRotation = InitRotation;
    _turnOrientation = TurnOrientation.Up;
    UpdateTurnAreas();
    UpdateHandZones();
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!Input.IsActionPressed("Grab"))
      return;
    
    if (!_focusedItem.IsValid())
      return;

    _focusedItem!.GlobalPosition = ToGlobal(TranslatedCursorDirection());
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is not InputEventMouseMotion)
      return;

    if (!_raycast.IsValid())
      return;

    Vector3 localDirection = TranslatedCursorDirection();

    _raycast!.TargetPosition = localDirection * 3f;

    if (!_raycast.IsColliding())
      return;

    GodotObject collider = _raycast.GetCollider();

    if (collider.IfValid() is not RigidBody3D item)
      return;

    _focusedItem = item;
  }

  private Vector3 TranslatedCursorDirection()
  {
    Vector2 mousePos = GetViewport().GetMousePosition();
    Vector3 rayNormal = ProjectRayNormal(mousePos);
    return GlobalBasis.Inverse() * rayNormal;
  }
}

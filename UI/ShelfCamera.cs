using System;
using System.Collections.Generic;
using Godot;
using ShopGame.Static;
using ShopGame.World;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class ShelfCamera : Camera3D
{
  [Export] private HandSprite? _handSprite;
  [Export] private ShelfPosGroup? _shelfPosGroup;
  [Export] private ShelfAreas? _shelfAreas;
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private RayCast3D? _raycast;

  private BoxItem? _focusedItem;
  
  internal Vector3 InitRotation;

  private TurnOrientation _turnOrientation;

  private readonly Dictionary<TurnOrientation, ShelfTurnArea?> _shelfTurnAreas = new()
  {
    [TurnOrientation.Up] = null,
    [TurnOrientation.Down] = null
  };

  private HandZone? _boxZone;
  private readonly List<ShelfZone> _shelfZones = [];
  
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

      if (handZone is not ShelfZone shelfZone)
      {
        _boxZone = handZone;
        _boxZone.MouseFilter = Control.MouseFilterEnum.Ignore;
        return;
      }

      _shelfZones.Add(shelfZone);
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
    if (!_boxZone.IsValid())
      return;
    
    if (_turnOrientation == TurnOrientation.Up)
    {
      _boxZone!.MouseFilter = Control.MouseFilterEnum.Ignore;
      _shelfZones.ForEach(x => x.MouseFilter = Control.MouseFilterEnum.Stop);
    }
    else
    {
      _boxZone!.MouseFilter = Control.MouseFilterEnum.Stop;
      _shelfZones.ForEach(x => x.MouseFilter = Control.MouseFilterEnum.Ignore);
    }
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
    if (!_focusedItem.IsValid())
      return;

    if (_handSprite!.FocusedShelfPos.Row != -1)
      _focusedItem!.Scale = .8f * Vector3.One;
    
    if (Input.IsActionPressed("Grab"))
    {
      _focusedItem!.GlobalPosition = ToGlobal(TranslatedCursorDirection());
      return;
    }

    HandleRelease();
    _focusedItem = null;
  }

  private void HandleRelease()
  {
    if (!_handSprite.IsValid())
      return;

    if (_handSprite!.FocusedShelfPos is not { Pos: not -1 } shelfPos)
    {
      _focusedItem!.ReturnToInitPos = true;
      return;
    }

    if (!_shelfPosGroup.IsValid())
      return;
    
    int hash = ShelfPosGroup.HashRowPos(shelfPos.Row, shelfPos.Pos);
    _shelfPosGroup!.ShelfPosDict[hash].PutItem(_focusedItem!);
  }

  public override void _Input(InputEvent @event)
    => HandleToyCapture(@event);

  private void HandleToyCapture(InputEvent @event)
  {
    if (@event is not InputEventMouseMotion)
      return;

    if (!_raycast.IsValid())
      return;

    Vector3 localDirection = TranslatedCursorDirection();

    _raycast!.TargetPosition = localDirection * 3f;

    if (!Input.IsActionPressed("Grab"))
      return;

    if (_focusedItem.IsValid())
      return;

    if (!_raycast.IsColliding())
      return;

    GodotObject collider = _raycast.GetCollider();

    if (collider.IfValid() is not BoxItem item)
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

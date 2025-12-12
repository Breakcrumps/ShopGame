using System.Collections.Generic;
using Godot;
using ShopGame.Scenes.ToyShelf.In3D;
using ShopGame.Scenes.ToyShelf.Toys;
using ShopGame.Scenes.ToyShelf.UI;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.In2D;

[GlobalClass]
internal sealed partial class ShelfCamera : Camera3D
{
  [Export] private HandSprite? _handSprite;
  [Export] private ShelfPosGroup? _shelfPosGroup;
  [Export] private ShelfScreenAreas? _shelfAreas;
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private RayCast3D? _raycast;

  private Toy? _focusedToy;
  private ShelfPosNode? _focusedPosNode;
  
  private Vector3 _initRotation;

  private TurnOrientation _turnOrientation;

  private readonly Dictionary<TurnOrientation, ShelfTurnArea?> _shelfTurnAreas = new()
  {
    [TurnOrientation.Up] = null,
    [TurnOrientation.Down] = null
  };

  private HandZone? _boxZone;
  private readonly List<ShelfHandZone> _shelfZones = [];
  
  public override void _Ready()
  {
    _initRotation = GlobalRotation;

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

      if (handZone is not ShelfHandZone shelfZone)
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
    _animPlayer?.Stop();
    GlobalRotation = _initRotation;
    _turnOrientation = TurnOrientation.Up;
    UpdateTurnAreas();
    UpdateHandZones();
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_focusedToy.IsValid() || !_handSprite.IsValid())
      return;

    if (_handSprite!.FocusedShelfPos.Row != -1)
      _focusedToy!.Scale = .8f * Vector3.One;

    if (!Input.IsActionPressed("Grab"))
    {
      HandleRelease();
      _focusedToy = null;
      return;
    }

    if (
      _focusedPosNode.IsValid()
      && !_focusedPosNode!.PosEqualsTo(_handSprite.FocusedShelfPos)
    )
    {
      _focusedPosNode.StopHover();
      _focusedPosNode = null;
    }
    
    if (_handSprite.FocusedShelfPos is { Row: not -1 } shelfPos)
    {
      int hash = ShelfPos.HashRowPos(shelfPos);
      _shelfPosGroup?.ShelfPosDict[hash].StartHover();
      _focusedPosNode = _shelfPosGroup?.ShelfPosDict[hash];
    }
    
    _focusedToy!.GlobalPosition = ToGlobal(TranslatedCursorDirection());
  }

  private void HandleRelease()
  {
    _focusedPosNode?.StopHover();
    _focusedPosNode = null;
    
    if (!_handSprite.IsValid())
      return;

    if (_handSprite!.FocusedShelfPos is not { Pos: not -1 } shelfPos)
    {
      _focusedToy!.ReturnToInitPos = true;
      return;
    }

    if (!_shelfPosGroup.IsValid())
      return;
    
    int hash = ShelfPos.HashRowPos(shelfPos);
    _shelfPosGroup!.ShelfPosDict[hash].PutItem(_focusedToy!);
    Inventory.RemoveToy(_focusedToy!.ToyType);
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

    if (_focusedToy.IsValid())
      return;

    if (!_raycast.IsColliding())
      return;

    GodotObject collider = _raycast.GetCollider();

    if (collider.IfValid() is not Toy item)
      return;

    item.FreeIfOnShelf();

    _focusedToy = item;
  }

  private Vector3 TranslatedCursorDirection()
  {
    Vector2 mousePos = GetViewport().GetMousePosition();
    Vector3 rayNormal = ProjectRayNormal(mousePos);
    return GlobalBasis.Inverse() * rayNormal;
  }
}

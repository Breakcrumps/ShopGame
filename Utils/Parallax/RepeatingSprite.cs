using System.Collections.Generic;
using Godot;
using ShopGame.Extensions;
using ShopGame.Scenes.FightScene;
using ShopGame.Static;

namespace ShopGame.Utils.Parallax;

[GlobalClass]
internal sealed partial class RepeatingSprite : Node2D
{
  [Export] private Texture2D? _texture;
  [Export] private FightCamera? _camera;

  [Export] private Vector2I _repeatTimes = new(1, 0);
  [Export] private Vector2 _scale = Vector2.One;

  private Vector2 _lastCameraPos;
  private Vector2 _textureSize;

  private readonly Dictionary<Vector2I, Sprite2D> _activeTiles = [];

  public override void _Ready()
  {
    if (!_camera.IsValid() || !_texture.IsValid())
      return;

    _lastCameraPos = ToLocal(_camera.GlobalPosition);
    _textureSize = _texture.GetSize() * _scale;

    UpdateTiles();
  }

  public override void _Process(double delta)
  {
    if (!_camera.IsValid())
      return;

    Vector2 cameraLocalPos = ToLocal(_camera.GlobalPosition);
    Vector2 difVector = cameraLocalPos - _lastCameraPos;
    
    if (difVector.Length() < _textureSize.MinAxis() * .5f)
      return;
    
    _lastCameraPos = cameraLocalPos;

    UpdateTiles();
  }

  private void UpdateTiles()
  {
    Vector2I centerCell = (ToLocal(_camera!.GlobalPosition) / _textureSize).RoundedToInt();
    Vector2I minCell = centerCell - _repeatTimes;
    Vector2I maxCell = centerCell + _repeatTimes;

    HashSet<Vector2I> _cellsToKeep = [];

    for (int x = minCell.X; x <= maxCell.X; x++)
    {
      for (int y = minCell.Y; y <= maxCell.Y; y++)
      {
        Vector2I cell = new(x, y);
        _cellsToKeep.Add(cell);
        
        if (_activeTiles.ContainsKey(cell))
          continue;

        Sprite2D newTile = new()
        {
          Texture = _texture,
          Position = cell * _textureSize,
          Scale = _scale
        };
        AddChild(newTile);
        _activeTiles[cell] = newTile;
      }
    }

    List<Vector2I> _cellsToRemove = [];

    foreach (Vector2I cell in _activeTiles.Keys)
    {
      if (!_cellsToKeep.Contains(cell))
        _cellsToRemove.Add(cell);
    }

    foreach (Vector2I cell in _cellsToRemove)
    {
      _activeTiles[cell].QueueFree();
      _activeTiles.Remove(cell);
    }
  }
}

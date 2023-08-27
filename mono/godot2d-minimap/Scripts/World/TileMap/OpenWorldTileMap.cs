using System;
using System.Collections.Generic;
using Godot;

public partial class OpenWorldTileMap : BaseTileMap
{
    private string _seed;
    private Vector2 _centerPos;
    private Position[,] _positions = new Position[3, 3];
    private List<Position> _addList = new List<Position>();
    private List<Position> _removeList = new List<Position>();
    private int _moveDir = 1;
    private Dictionary<string, string> _chunkDict = new Dictionary<string, string>();

    public override void _Ready()
    {
        _seed = Guid.NewGuid().ToString("N");
        GlobalData.Instance.NoiseMapSeed = _seed;
        _genData();
        _drawMap();
    }

    public override void _Process(double delta)
    {
    }

    public override Vector2 Generator()
    {
        return _centerPos;
    }

    private void _genData()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _positions[i, j] = new Position(i, j);
            }
        }
        _centerPos = new Vector2(15 * TileConstant.TileSize, 15 * TileConstant.TileSize);
        GlobalData.Instance.CenterPos = _centerPos;
    }

    private void _drawMap()
    {
        for (int i = 0; i < _positions.GetLength(0); i++)
        {
            string str = " ";
            for (int j = 0; j < _positions.GetLength(1); j++)
            {
                var chunkTileMap = GD.Load<PackedScene>("res://World/TileMap/ChunkTileMap.tscn")
                    .Instantiate<ChunkTileMap>();
                AddChild(chunkTileMap);
                chunkTileMap.Connect("OnPlayerEntered", new Callable(this, nameof(_handleEntered)));
                string id = chunkTileMap.CreateChunk(_seed, _positions[i, j].X, _positions[i, j].Y);
                _chunkDict.Add(_positions[i, j].X + "," + _positions[i, j].Y, id);
                str += _positions[i, j].X + "," + _positions[i, j].Y + " ";
            }

            GD.Print(str);
        }
    }

    private void _updateMapData()
    {
        Position[,] newPositions = new Position[3, 3];
        _addList.Clear();
        _removeList.Clear();
        for (int i = 0; i < 3; i++)
        {
            string str = " ";
            for (int j = 0; j < 3; j++)
            {
                if (_moveDir == 1)
                {
                    if (j == 0)
                    {
                        newPositions[i, j] = new Position(_positions[i, j].X, _positions[i, j].Y - 1);
                        _addList.Add(newPositions[i, j]);
                    }
                    else
                    {
                        if (j == 2)
                        {
                            _removeList.Add(_positions[i, j]);
                        }

                        newPositions[i, j] = _positions[i, j - 1];
                    }
                }

                if (_moveDir == 2)
                {
                    if (j == 2)
                    {
                        newPositions[i, j] = new Position(_positions[i, j].X, _positions[i, j].Y + 1);
                        _addList.Add(newPositions[i, j]);
                    }
                    else
                    {
                        newPositions[i, j] = _positions[i, j + 1];
                        if (j == 0)
                        {
                            _removeList.Add(_positions[i, j]);
                        }
                    }
                }

                if (_moveDir == 3)
                {
                    if (i == 0)
                    {
                        newPositions[i, j] = new Position(_positions[i, j].X - 1, _positions[i, j].Y);
                        _addList.Add(newPositions[i, j]);
                    }
                    else
                    {
                        newPositions[i, j] = _positions[i - 1, j];
                        if (i == 2)
                        {
                            _removeList.Add(_positions[i, j]);
                        }
                    }
                }

                if (_moveDir == 4)
                {
                    if (i == 2)
                    {
                        newPositions[i, j] = new Position(_positions[i, j].X + 1, _positions[i, j].Y);
                        _addList.Add(newPositions[i, j]);
                    }
                    else
                    {
                        newPositions[i, j] = _positions[i + 1, j];
                        if (i == 0)
                        {
                            _removeList.Add(_positions[i, j]);
                        }
                    }
                }

                str += newPositions[i, j].X + "," + newPositions[i, j].Y + "   ";
            }

            GD.Print(str);
        }

        _updateMap();
        _positions = newPositions;
    }

    private void _handleEntered(Vector2 newCenterPos)
    {
        _calculateRelativeDirection(newCenterPos, _centerPos);
        _centerPos = newCenterPos;
        GlobalData.Instance.CenterPos = _centerPos;
        _updateMapData();
    }

    private void _calculateRelativeDirection(Vector2 vectorA, Vector2 vectorB)
    {
        Vector2 direction = (vectorA - vectorB).Normalized();

        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            if (direction.X > 0)
            {
                _moveDir = 4;
                GD.Print("Right");
            }
            else
            {
                _moveDir = 3;
                GD.Print("Left");
            }
        }
        else
        {
            if (direction.Y > 0)
            {
                _moveDir = 2;
                GD.Print("Down");
            }
            else
            {
                _moveDir = 1;
                GD.Print("Up");
            }
        }
    }

    private void _updateMap()
    {
        string removestr = " ";
        foreach (var t in _removeList)
        {
            removestr += t.X + "," + t.Y + " ";
            for (int i = 0; i < GetChildCount(); i++)
            {
                if (GetChild<ChunkTileMap>(i).Id == _chunkDict[t.X + "," + t.Y])
                {
                    GetChild<ChunkTileMap>(i).QueueFree();
                    _chunkDict.Remove(t.X + "," + t.Y);
                    break;
                }
            }
        }

        GD.Print(removestr);

        string addstr = " ";
        foreach (var t in _addList)
        {
            addstr += t.X + "," + t.Y + " ";
            var chunkTileMap = GD.Load<PackedScene>("res://World/TileMap/ChunkTileMap.tscn")
                .Instantiate<ChunkTileMap>();
            AddChild(chunkTileMap);
            chunkTileMap.Connect("OnPlayerEntered", new Callable(this, nameof(_handleEntered)));
            string id = chunkTileMap.CreateChunk(_seed, t.X, t.Y);
            _chunkDict.Add(t.X + "," + t.Y, id);
        }

        GD.Print(addstr);
    }

    private void OnOptionButtonChange(int selected)
    {
        _moveDir = selected + 1;
    }
}
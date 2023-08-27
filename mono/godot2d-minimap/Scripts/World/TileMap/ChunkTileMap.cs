using Godot;
using System;

public partial class ChunkTileMap : BaseTileMap
{
    [Signal]
    public delegate void OnPlayerEnteredEventHandler(Vector2 centerPos);

    [Signal]
    public delegate void OnPlayerExitedEventHandler();

    public string Id;
    private int _width = 10;
    private int _height = 10;
    private int _roomSpace = 0;
    private bool _created;
    private bool _entered;
    private bool _enteredSignal;
    private bool _exitedSignal;
    private int _posX;
    private int _posY;
    private Vector2 _centerPos;
    private int _seed;

    private FastNoiseLite _fastNoiseLite1;
    private FastNoiseLite _fastNoiseLite2;

    public override void _PhysicsProcess(double delta)
    {
        if (_created)
        {
            if (GlobalData.Instance.PlayerPosition.X > _posX * TileConstant.TileSize &&
                GlobalData.Instance.PlayerPosition.Y > _posY * TileConstant.TileSize
                && GlobalData.Instance.PlayerPosition.X <
                (_posX + _width) * TileConstant.TileSize
                && GlobalData.Instance.PlayerPosition.Y <
                (_posY + _height) * TileConstant.TileSize
                && !_entered
               )
            {
                if (!_enteredSignal)
                {
                    _entered = true;
                    _enteredSignal = true;
                    _exitedSignal = false;
                    GD.Print(Id + "====entered...");
                    if (_centerPos != GlobalData.Instance.CenterPos)
                    {
                        EmitSignal(nameof(OnPlayerEntered), _centerPos);
                    }
                }
            }

            if ((GlobalData.Instance.PlayerPosition.X < _posX * TileConstant.TileSize ||
                 GlobalData.Instance.PlayerPosition.Y < _posY * TileConstant.TileSize
                 || GlobalData.Instance.PlayerPosition.X >
                 (_posX + _width) * TileConstant.TileSize
                 || GlobalData.Instance.PlayerPosition.Y >
                 (_posY + _height) * TileConstant.TileSize)
                && _entered
               )
            {
                if (!_exitedSignal)
                {
                    _entered = false;
                    _exitedSignal = true;
                    _enteredSignal = false;
                    GD.Print(Id + "----exited...");
                    EmitSignal(nameof(OnPlayerExited));
                }
            }
        }
    }

    public string CreateChunk(string seed, int x, int y)
    {
        _seed = (seed + (x + "-" + y)).GetHashCode();
        Id = Guid.NewGuid().ToString("N");
        _posX = (_width + _roomSpace) * x;
        _posY = (_height + _roomSpace) * y;

        _created = true;
        _centerPos = new Vector2((_posX + _width / 2) * TileConstant.TileSize,
            (_posY + _height / 2) * TileConstant.TileSize);

        _fastNoiseLite1 = new FastNoiseLite();
        _fastNoiseLite1.Seed = (GlobalData.Instance.NoiseMapSeed + "noise1").GetHashCode();
        _fastNoiseLite1.Frequency = 0.01f;
        _fastNoiseLite2 = new FastNoiseLite();
        _fastNoiseLite2.Seed = (GlobalData.Instance.NoiseMapSeed + "noise2").GetHashCode();
        _fastNoiseLite2.Frequency = 0.01f;

        // _drawRoom(new Vector2(_posX, _posY));
        _noiseMap(new Vector2(_posX, _posY));
        return Id;
    }

    private void _drawRoom(Vector2 pos)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (i == 0 || i == _width - 1 || j == 0 || j == _height - 1)
                {
                    _setTile(new Vector2(pos.X + i, pos.Y + j), TileConstant.DoorTileId);
                }
                else
                {
                    _setTile(new Vector2(pos.X + i, pos.Y + j), TileConstant.FloorTileId);
                }
            }
        }
    }

    private void _noiseMap(Vector2 pos)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 c = new Vector2(pos.X + i, pos.Y + j);
                var a = _fastNoiseLite1.GetNoise2D(c.X, c.Y);
                var a2 = _fastNoiseLite2.GetNoise2D(c.X, c.Y);
                SetCell(0, (Vector2I)c, 0, new Vector2I(_randomTile(a), _randomTile(a2)));
            }
        }
    }

    private int _randomTile(float f)
    {
        var r = f > 0 ? Math.Floor((f + 1) * 2) : Math.Floor((f + 2) * 2);
        GD.Print(r + "   " + f);
        return (int)r;
    }

    private int _randiRange(int num1, int num2)
    {
        num2 += 1;
        Random random = new Random(_seed);
        if (num1 < num2)
        {
            return random.Next(num1, num2);
        }

        return random.Next(num2, num1);
    }
}
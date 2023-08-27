using Godot;

public partial class World : Node2D
{
    private CanvasLayer _ui;
    private BaseTileMap _worldTileMap;
    private bool _dialogClose = true;
    private Player _player;
    private Minimap _minimap;

    public override void _Ready()
    {
        _ui = GetNode<CanvasLayer>("UI");
        _worldTileMap = GetNode<BaseTileMap>("TileMap");
        _minimap = GetNode<Minimap>("UI/Minimap");

        InitMap();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("map"))
        {
            var minimapDialog = GD.Load<PackedScene>("res://Minimap/MinimapDialog.tscn").Instantiate<MinimapDialog>();
            _ui.AddChild(minimapDialog);
            minimapDialog.Init();
            _minimap.Hide();
        }
        
        if (Input.IsActionJustReleased("map"))
        {
            var miniMap = _ui.GetNode<MinimapDialog>("MinimapDialog");
            miniMap.QueueFree();
            _minimap.Show();
        }
    }

    public void InitMap()
    {
        // 存储地图数据
        GlobalData.Instance.IsaacMapData = IsaacMapGenerator.GeneratorMapData();
        var playerPosition = _worldTileMap.Generator();

        if (_player == null)
        {
            _player = GD.Load<PackedScene>("res://Player/Player.tscn").Instantiate() as Player;
            AddChild(_player);
        }
        _player.Position = playerPosition;

        _minimap.Init();
    }
    
}
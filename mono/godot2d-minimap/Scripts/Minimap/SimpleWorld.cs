using Godot;
using System;

public partial class SimpleWorld : Node2D
{
    private BaseTileMap _baseTileMap;
    private CharacterBody2D _simplePlayer;
    public Camera2D MiniCamera;

    public override void _Ready()
    {
        _baseTileMap = GetNode<BaseTileMap>("TileMap");
        _simplePlayer = GetNode<CharacterBody2D>("SimplePlayer");
        MiniCamera = _simplePlayer.GetNode<Camera2D>("Camera2D");

        GeneratorMap();
    }

    public override void _Process(double delta)
    {
        _simplePlayer.Position = GlobalData.Instance.PlayerPosition;
    }

    public void GeneratorMap()
    {
        Vector2 playerPosition = _baseTileMap.Generator();
        _simplePlayer.Position = playerPosition;
    }
}
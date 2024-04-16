using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class World : Node2D
{
    private Player _player;

    private HexagonMap3 _hexagonMap;

    private DrawPath _debugger;

    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _hexagonMap = GetNode<HexagonMap3>("PointyHexMap");
        _debugger = GetNode<DrawPath>("DrawPath");
    }

    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("mouse_left"))
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Vector2> path = _hexagonMap.FindPath(_player.GlobalPosition, GetGlobalMousePosition());
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            GD.Print("代码执行耗时: " + elapsed.TotalMilliseconds + " 毫秒");
            if (path != null)
            {
                _player.SetPath(path);
                _debugger.UpdatePath(path);
            }
        }
    }
}
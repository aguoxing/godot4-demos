using Godot;
using System;

public partial class MinimapDialog : Control
{
    private SubViewport _subViewport;
    private SimpleWorld _simpleWorld;
    private Vector2 _zoom = new Vector2(1, 1);

    public override void _Ready()
    {
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("mouseUp"))
        {
            if (_zoom < new Vector2(5, 5))
            {
                _zoom = new Vector2(_zoom.X + 1, _zoom.Y + 1);
            }

            _simpleWorld.MiniCamera.Zoom = _zoom;
        }

        if (Input.IsActionJustPressed("mouseDown"))
        {
            if (_zoom > new Vector2(1, 1))
            {
                _zoom = new Vector2(_zoom.X - 1, _zoom.Y - 1);
            }

            _simpleWorld.MiniCamera.Zoom = _zoom;
        }
    }

    public void Init()
    {
        if (_simpleWorld == null)
        {
            _simpleWorld = GD.Load<PackedScene>("res://MiniMap/SimpleWorld.tscn").Instantiate<SimpleWorld>();
            _subViewport.AddChild(_simpleWorld);
        }
        else
        {
            _simpleWorld.GeneratorMap();
        }
    }
}
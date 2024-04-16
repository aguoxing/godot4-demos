using Godot;
using System;
using Godot.Collections;

public partial class Fog3 : Sprite2D
{
    private int _gridSize = 16;
    private int _displayWidth = 1280;
    private int _displayHeight = 720;
    private Texture2D _lightTexture = GD.Load<Texture2D>("res://Assets/Effects/light2.png");

    private Image _fogImage;
    private Color _fogColor = Colors.Black;
    private Image _lightImage;
    private Vector2 _lightOffset;

    private int _updateRate = 4;
    private float _distanceToUpdate = 300.0f;
    private Dictionary<Node2D, Vector2> _dictionary = new Dictionary<Node2D, Vector2>();

    public override void _Ready()
    {
        _initFog();
    }

    public override void _Process(double delta)
    {
        if (Engine.GetFramesDrawn() % _updateRate != 0)
        {
            return;
        }

        foreach (var node in GetTree().GetNodesInGroup("fog"))
        {
            Node2D node2D = (Node2D)node;
            if (!_dictionary.ContainsKey(node2D) || _dictionary[node2D].DistanceSquaredTo(node2D.Position) > _distanceToUpdate)
            {
                _updateFog(node2D.Position);
                _dictionary[node2D] = node2D.Position;
            }
        }
    }

    private void _initFog()
    {
        int fogImageWidth = _displayWidth / _gridSize;
        int fogImageHeight = _displayHeight / _gridSize;
        _fogImage = Image.Create(fogImageWidth, fogImageHeight, false, Image.Format.Rgbah);
        _fogImage.Fill(_fogColor);
        Texture = ImageTexture.CreateFromImage(_fogImage);
        Scale *= _gridSize;

        _lightImage = _lightTexture.GetImage();
        _lightOffset = new Vector2(_lightTexture.GetWidth() * 0.5f, _lightTexture.GetHeight() * 0.5f);
        _lightImage.Convert(Image.Format.Rgbah);
    }

    private void _updateFog(Vector2 pos)
    {
        pos = ToLocal(pos).Round();
        Rect2I lightRect = new Rect2I(Vector2I.Zero, new Vector2I(_lightImage.GetWidth(), _lightImage.GetHeight()));
        _fogImage.BlendRect(_lightImage, lightRect, (Vector2I)(pos - _lightOffset));
        Texture = ImageTexture.CreateFromImage(_fogImage);
    }
}
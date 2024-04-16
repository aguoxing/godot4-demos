using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Fog : Sprite2D
{
    private int _displayWidth = 1280;
    private int _displayHeight = 720;
    [Export] private int _fogPixelSize = 1;
    [Export] private Color _fogColor = new Color(1, 1, 1, 0.6f);
    private bool _smoothBorders = true;
    private int _updateRate = 4;
    private float _distanceToUpdate = 500.0f;
    private Image _fogImage;
    private Dictionary<Node2D, Vector2> _dictionary = new Dictionary<Node2D, Vector2>();

    public override void _Ready()
    {
        _initFogTexture();
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
                _updateFogTexture(node2D.Position, 100);
                _dictionary[node2D] = node2D.Position;
            }
        }
    }

    private void _initFogTexture()
    {
        int fogImageWidth = _displayWidth / _fogPixelSize;
        int fogImageHeight = _displayHeight / _fogPixelSize;
        _fogImage = Image.Create(fogImageWidth, fogImageHeight, true, Image.Format.Rgba8);
        _fogImage.Fill(_fogColor);
        Scale *= _fogPixelSize;
    }

    private void _updateFogTexture(Vector2 at, float radius)
    {
        at = ToLocal(at).Round();
        radius /= Scale.X;
        float ix = at.X - radius;
        float jx = at.X + radius;
        float iy = at.Y - radius;
        float jy = at.Y + radius;
        for (float i = ix; i < jx; i++)
        {
            for (float j = iy; j < jy; j++)
            {
                if (i < 0 || j < 0 || i >= _fogImage.GetWidth() || j >= _fogImage.GetHeight())
                {
                    continue;
                }

                var distance = at.DistanceTo(new Vector2(i, j));
                if (distance < radius)
                {
                    float newAlpha = 0.0f;
                    if (_smoothBorders)
                    {
                        newAlpha = Mathf.Ease(distance / radius, 3);
                    }

                    if (_fogImage.GetPixel((int)i, (int)j).A > newAlpha)
                    {
                        _fogImage.SetPixel((int)i, (int)j, new Color(0, 0, 0, newAlpha));
                    }
                }
            }
        }

        Texture = ImageTexture.CreateFromImage(_fogImage);
    }
}
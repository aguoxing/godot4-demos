using Godot;
using System;

public partial class Lines : Node2D
{
    private Vector2[][] directionVectors;
    private Vector2 _goal;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void ShowDirLine(Vector2[][] dVectors, Vector2 goal)
    {
        directionVectors = dVectors;
        _goal = goal;
        QueueRedraw();
    }

    public override void _Draw()
    {
        _tileDrawLine();
        _drawGoalRect();
    }

    private void _tileDrawLine()
    {
        for (int i = 0; i < directionVectors.Length; i++)
        {
            for (int j = 0; j < directionVectors[i].Length; j++)
            {
                Vector2 from = new Vector2(i * 16 + 8, j * 16 + 8);
                Vector2 to = from + directionVectors[i][j] * 8;
                DrawLine(from, to, new Color(1, 0, 0), 0.5f);
            }
        }
    }

    private void _drawGoalRect()
    {
        Vector2 t = new Vector2(_goal.X, _goal.Y);
        DrawRect(new Rect2(t, 16, 16), Colors.SkyBlue);
    }
}
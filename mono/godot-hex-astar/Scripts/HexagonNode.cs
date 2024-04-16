using System.Collections.Generic;
using Godot;

public class HexagonNode
{
    public Vector2 Position { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public HexagonNode Parent { get; set; }
    public HexagonNode CameFromNode { get; set; }
    public double G { get; set; }
    public double H { get; set; }
    public double F => G + H;
    // public int F;
    
    public List<HexagonNode> Neighbors = new List<HexagonNode>();
  
    public bool IsObstacle;

    public HexagonNode(Vector2 position)
    {
        G = 9999;
        x = (int)position.X;
        y = (int)position.Y;
        Position = position;
    }
    
    public HexagonNode(int x_, int y_)
    {
        G = 9999;
        x = x_;
        y = y_;
        Position = new Vector2(x, y);
    }
}
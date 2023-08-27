using Godot;

public partial class BaseTileMap: TileMap
{
    public override void _Ready()
    {
    }

    public virtual Vector2 Generator()
    {
        return Vector2.Zero;
    }
    
    public void _setTile(Vector2 pos, int tileType)
    {
        SetCell(0, (Vector2I)pos, tileType, new Vector2I());
    }

    public bool _getTile(Vector2 pos, int tileType)
    {
        return GetCellSourceId(0, (Vector2I)pos) == tileType;
    }
}
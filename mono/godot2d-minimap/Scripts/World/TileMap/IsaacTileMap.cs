using Godot;
using System;
using System.Collections.Generic;

public partial class IsaacTileMap : BaseTileMap
{
    public Vector2 PlayerPosition;
    
    // 房间宽高为奇数
    public int RoomWidth = 19;
    public int RoomHeight = 15;
    public int Spacing = 6; // 房间间隔 最好为偶数
    public int tileSize = 16;
    public int doorSize = 3; // 最好为奇数 最小为3

    public override Vector2 Generator()
    {
        Clear();
        // 读取全局地图数据
        // List<Position> rooms = IsaacMapGenerator.GeneratorMapData();
        List<Position> rooms = GlobalData.Instance.IsaacMapData;
        // IsaacMapGenerator.PrintMap();
        // DrawMap();
        // DrawRect(rooms);

        CreateRoom(rooms);
        ConnectRooms(rooms);

        return PlayerPosition;
    }

    public void DrawMap()
    {
        for (int i = 0; i < IsaacMapGenerator.mapArr.GetLength(0); i++)
        {
            for (int j = 0; j < IsaacMapGenerator.mapArr.GetLength(1); j++)
            {
                _setTile(new Vector2(i, j), IsaacMapGenerator.mapArr[i, j]);
                if (IsaacMapGenerator.mapArr[i, j] == IsaacMapGenerator.playerFloorTileId)
                {
                    PlayerPosition = new Vector2(i * tileSize + tileSize / 2, j * tileSize + tileSize / 2);
                    // _setTile(new Vector2(i, j), IsaacMapGenerator.mapArr[i, j]);
                }
                // else if (IsaacMapGenerator.mapArr[i, j] == IsaacMapGenerator.bossFloorTileId)
                // {
                // 	_setTile(new Vector2(i, j), IsaacMapGenerator.mapArr[i, j]);
                // }
                // else
                // {
                // 	_setTile(new Vector2(i, j), IsaacMapGenerator.initialTileId);
                // }
            }
        }
    }

    public void CreateRoom(List<Position> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            int x = (int)rooms[i].X;
            int y = (int)rooms[i].Y;

            int posX = (int)((RoomWidth + Spacing) * x);
            int posY = (int)((RoomHeight + Spacing) * y);

            if (IsaacMapGenerator.mapArr[x, y] == IsaacMapGenerator.floorTileId)
            {
                DrawRoom(new Vector2(posX, posY), IsaacMapGenerator.floorTileId, RoomWidth, RoomHeight);
            }

            if (IsaacMapGenerator.mapArr[x, y] == IsaacMapGenerator.playerFloorTileId)
            {
                // 玩家初始位置
                int middleX = (RoomWidth - 1) / 2;
                int middleY = (RoomHeight - 1) / 2;
                PlayerPosition = new Vector2((posX + middleX) * tileSize, (posY + middleY) * tileSize);
                DrawRoom(new Vector2(posX, posY), IsaacMapGenerator.playerFloorTileId, RoomWidth, RoomHeight);
            }

            if (IsaacMapGenerator.mapArr[x, y] == IsaacMapGenerator.bossFloorTileId)
            {
                DrawRoom(new Vector2(posX, posY), IsaacMapGenerator.bossFloorTileId, RoomWidth, RoomHeight);
            }
        }
    }

    // 打通各个房间的门与走廊
    public void ConnectRooms(List<Position> rooms)
    {
        int middleX = (RoomWidth - 1) / 2;
        int middleY = (RoomHeight - 1) / 2;
        int halfDoorSize = (doorSize - 1) / 2;

        for (int i = 0; i < rooms.Count; i++)
        {
            int x = (int)rooms[i].X;
            int y = (int)rooms[i].Y;

            int posX = (int)((RoomWidth + Spacing) * x);
            int posY = (int)((RoomHeight + Spacing) * y);

            if (y - 1 >= 0 && IsaacMapGenerator.mapArr[x, y - 1] != IsaacMapGenerator.initialTileId)
            {
                // 上
                for (int d = -halfDoorSize; d <= halfDoorSize; d++)
                {
                    for (int c = 0; c <= Spacing / 2; c++)
                    {
                        // 过道
                        if (c == 0)
                        {
                            _setTile(new Vector2(posX + middleX + d, posY), IsaacMapGenerator.doorTileId);
                        }
                        else
                        {
                            _setTile(new Vector2(posX + middleX + d, posY - c), IsaacMapGenerator.corridorTileId);
                        }

                        // 墙
                        int w = d + (d > 0 ? 1 : -1);
                        _setTile(new Vector2(posX + middleX - w, posY - c), IsaacMapGenerator.wallTileId);
                    }
                }
            }

            if (y + 1 < IsaacMapGenerator.mapHeight &&
                IsaacMapGenerator.mapArr[x, y + 1] != IsaacMapGenerator.initialTileId)
            {
                for (int d = -halfDoorSize; d <= halfDoorSize; d++)
                {
                    // 下
                    for (int c = 0; c <= Spacing / 2; c++)
                    {
                        if (c == 0)
                        {
                            _setTile(new Vector2(posX + middleX + d, posY + RoomHeight - 1),
                                IsaacMapGenerator.doorTileId);
                        }
                        else
                        {
                            _setTile(new Vector2(posX + middleX + d, posY + RoomHeight - 1 + c),
                                IsaacMapGenerator.corridorTileId);
                        }

                        int w = d + (d > 0 ? 1 : -1);
                        _setTile(new Vector2(posX + middleX - w, posY + RoomHeight - 1 + c),
                            IsaacMapGenerator.wallTileId);
                    }
                }
            }

            if (x - 1 >= 0 && IsaacMapGenerator.mapArr[x - 1, y] != IsaacMapGenerator.initialTileId)
            {
                for (int d = -halfDoorSize; d <= halfDoorSize; d++)
                {
                    // 左
                    for (int c = 0; c <= Spacing / 2; c++)
                    {
                        if (c == 0)
                        {
                            _setTile(new Vector2(posX + 0, posY + middleY + d), IsaacMapGenerator.doorTileId);
                        }
                        else
                        {
                            _setTile(new Vector2(posX + 0 - c, posY + middleY + d), IsaacMapGenerator.corridorTileId);
                        }

                        int w = d + (d > 0 ? 1 : -1);
                        _setTile(new Vector2(posX + 0 - c, posY + middleY - w), IsaacMapGenerator.wallTileId);
                    }
                }
            }

            if (x + 1 < IsaacMapGenerator.mapWidth &&
                IsaacMapGenerator.mapArr[x + 1, y] != IsaacMapGenerator.initialTileId)
            {
                for (int d = -halfDoorSize; d <= halfDoorSize; d++)
                {
                    // 右
                    for (int c = 0; c <= Spacing / 2; c++)
                    {
                        // 过道
                        if (c == 0)
                        {
                            _setTile(new Vector2(posX + RoomWidth - 1, posY + middleY + d),
                                IsaacMapGenerator.doorTileId);
                        }
                        else
                        {
                            _setTile(new Vector2(posX + RoomWidth - 1 + c, posY + middleY + d),
                                IsaacMapGenerator.corridorTileId);
                        }

                        // 墙
                        int w = d + (d > 0 ? 1 : -1);
                        _setTile(new Vector2(posX + RoomWidth - 1 + c, posY + middleY - w),
                            IsaacMapGenerator.wallTileId);
                    }
                }
            }
        }
    }

    public void DrawRoom(Vector2 pos, int type, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    _setTile(new Vector2(pos.X + i, pos.Y + j), IsaacMapGenerator.wallTileId);
                }
                else
                {
                    _setTile(new Vector2(pos.X + i, pos.Y + j), type);
                }
            }
        }
    }

    public void DrawRect(List<Position> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            int x = rooms[i].X;
            int y = rooms[i].Y;
            if (IsaacMapGenerator.mapArr[x, y] == IsaacMapGenerator.playerFloorTileId)
            {
                PlayerPosition = new Vector2(x * tileSize + tileSize / 2, y * tileSize + tileSize / 2);
            }

            _setTile(new Vector2(x, y), IsaacMapGenerator.mapArr[x, y]);
        }
    }

}

using System;
using System.Collections.Generic;
using Godot;

public static class IsaacMapGenerator
{
	// 地图 二维数组
	public static int[,] mapArr;
	public static int mapWidth;
	public static int mapHeight;

	// 房间位置坐标
	public static List<Position> roomCoordinates = new List<Position>();
	// 路径坐标
	public static List<Position> pathCoordinates = new List<Position>();

	public static int initialTileId = 0;
	public static int floorTileId = 1;
	public static int corridorTileId = 2;
	public static int wallTileId = 3;
	public static int doorTileId = 4;
	public static int playerFloorTileId = 5;
	public static int bossFloorTileId = 6;

	public static void Init()
	{
		// 确保地图长宽为奇数
		int size = randiRange(3, 5) * 2 + 1;
		int rectangularity = randiRange(0, (1 + size) / 2) * 2;
		int w = size, h = size;
		if (0 == randiRange(0, 1))
		{
			w += rectangularity;
		}
		else
		{
			h += rectangularity;
		}

		mapWidth = w;
		mapHeight = h;
		mapArr = new int[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				mapArr[i, j] = initialTileId;
			}
		}
		GD.Print("w: " + mapWidth + " h: " + mapHeight);
	}

	public static List<Position> GeneratorMapData()
	{
		Init();
		
		roomCoordinates.Clear();
		pathCoordinates.Clear();
		Position startPos = GenStartRoom();
		Position endPos = GenEndRoom(startPos.X, startPos.Y);
		ConnectCoordinates(startPos.X, startPos.Y, endPos.X, endPos.Y);
		RandomlyAddNeighbors(mapArr, pathCoordinates);
		mapArr[endPos.X, endPos.Y] = bossFloorTileId;
		GD.Print("roomCoordinates " + roomCoordinates.Count);

		return roomCoordinates;
	}

	// 生成起始房间
	public static Position GenStartRoom()
	{
		int startX = (mapWidth - 1) / 2;
		int startY = (mapHeight - 1) / 2;
		mapArr[startX, startY] = playerFloorTileId;
		roomCoordinates.Add(new Position(startX, startY));
		return new Position(startX, startY);
	}

	// 随机生成结束房间
	public static Position GenEndRoom(int startX, int startY)
	{
		int endX = 0;
		GD.Print("sx: " + startX + " sy: " + startY);
		int endY = 0;

		int dir = randiRange(0, 3);
		switch (dir)
		{
			case 0://右上
				endX = startX + randiRange(1, startX / 2);
				endY = startY - randiRange(1, startY / 2);
				break;
			case 1://左上
				endX = startX - randiRange(1, startX / 2);
				endY = startY - randiRange(1, startY / 2);
				break;
			case 2://左下
				endX = startX - randiRange(1, startX / 2);
				endY = startY + randiRange(1, startY / 2);
				break;
			case 3://右下
				endX = startX + randiRange(1, startX / 2);
				endY = startY + randiRange(1, startY / 2);
				break;
		}
		GD.Print("ex: " + endX + " ey: " + endY);

		roomCoordinates.Add(new Position(endX, endY));
		return new Position(endX, endY);
	}

	// 连接起始房间和结束房间
	public static void ConnectCoordinates(int startX, int startY, int endX, int endY)
	{
		int dx = System.Math.Abs(endX - startX);
		int dy = System.Math.Abs(endY - startY);

		int sx = startX < endX ? 1 : -1;
		int sy = startY < endY ? 1 : -1;

		int err = dx - dy;

		while (startX != endX || startY != endY)
		{
			int err2 = 2 * err;

			if (err2 > -dy)
			{
				err -= dy;
				startX += sx;
				mapArr[startX, startY] = floorTileId;
				pathCoordinates.Add(new Position(startX, startY));
				roomCoordinates.Add(new Position(startX, startY));
			}

			if (err2 < dx)
			{
				err += dx;
				startY += sy;
				mapArr[startX, startY] = floorTileId;
				pathCoordinates.Add(new Position(startX, startY));
				roomCoordinates.Add(new Position(startX, startY));
			}
		}

	}

	// 给连接的路径随机添加邻居房间
	public static void RandomlyAddNeighbors(int[,] mapArr, List<Position> pathCoordinates)
	{
		Random rand = new Random();

		for (int i = 1; i < pathCoordinates.Count - 1; i++)
		{
			int x = pathCoordinates[i].X;
			int y = pathCoordinates[i].Y;
			int neighborsCount = 0;

			// 检查当前坐标的上、下、左、右四个方向的邻居数量
			if (x > 0 && mapArr[x - 1, y] == floorTileId)
				neighborsCount++;
			if (x < mapArr.GetLength(0) - 1 && mapArr[x + 1, y] == floorTileId)
				neighborsCount++;
			if (y > 0 && mapArr[x, y - 1] == 1)
				neighborsCount++;
			if (y < mapArr.GetLength(1) - 1 && mapArr[x, y + 1] == floorTileId)
				neighborsCount++;

			// 添加邻居坐标，只添加没有邻居的邻居
			while (neighborsCount < 3)
			{
				List<int> directions = new List<int> { 0, 1, 2, 3 };
				bool addedNeighbor = false;

				while (directions.Count > 0 && neighborsCount < 3)
				{
					int index = rand.Next(directions.Count);
					int direction = directions[index];
					directions.RemoveAt(index);

					int newX = x, newY = y;
					switch (direction)
					{
						case 0: // 上
							newX = x - 1;
							break;
						case 1: // 下
							newX = x + 1;
							break;
						case 2: // 左
							newY = y - 1;
							break;
						case 3: // 右
							newY = y + 1;
							break;
					}

					// 确保新坐标不超过数组边界且不是起始或结束坐标的邻居
					if (newX >= 0 && newX < mapArr.GetLength(0) &&
							newY >= 0 && newY < mapArr.GetLength(1) &&
							!(newX == pathCoordinates[0].X && newY == pathCoordinates[0].Y) &&
							!(newX == pathCoordinates[pathCoordinates.Count - 1].X && newY == pathCoordinates[pathCoordinates.Count - 1].Y) &&
							mapArr[newX, newY] == initialTileId)
					{
						mapArr[newX, newY] = floorTileId;
						neighborsCount++;
						addedNeighbor = true;

						roomCoordinates.Add(new Position(newX, newY));
						// 添加邻居后跳出内部循环
						break;
					}
				}

				// 如果没有添加邻居，则跳出外部循环
				if (!addedNeighbor)
				{
					break;
				}
			}
		}
	}

	// 控制台打印
	public static void PrintMap()
	{
		for (int i = 0; i < mapWidth; i++)
		{
			string row = "";
			for (int j = 0; j < mapHeight; j++)
			{
				row = row + mapArr[i, j];
				Console.Write(mapArr[i, j]);
			}
			GD.Print(row);
			Console.WriteLine();
		}
	}

	private static int randiRange(int num1, int num2)
	{
		num2 += 1;
		Random random = new Random();
		if (num1 < num2)
		{
			return random.Next(num1, num2);
		}
        return random.Next(num2, num1);
	}
}

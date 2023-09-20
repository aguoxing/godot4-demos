using Godot;
using System.Collections.Generic;

public partial class FlowField : TileMap
{
    private Lines _lines;
    private int _width = 1280;
    private int _height = 720;
    private int _tileSize = 16;
    private int _rows;
    private int _cols;

    private List<int> _openList = new List<int>();
    private List<int> _closeList = new List<int>();

    public int[,] MapGridData;
    public Vector2 Goal = new Vector2(10, 10);
    public Vector2[][] DirectionVectors;

    public override void _Ready()
    {
        _lines = GetNode<Lines>("Lines");
        _rows = _height / _tileSize;
        _cols = _width / _tileSize;
    }

    public override void _Process(double delta)
    {
    }

    public void SetObstacleTile(Vector2 pos)
    {
        Vector2 wallPos = LocalToMap(pos);
        SetCell(1, new Vector2I((int)wallPos.X, (int)wallPos.Y), 1, new Vector2I());
    }

    public Vector2 GetMousePosition(Vector2 pos)
    {
        Vector2 tmp = LocalToMap(pos);
        return new Vector2(tmp.X * _tileSize, tmp.Y * _tileSize);
    }

    public void SetGoal(Vector2 pos)
    {
        Vector2 goalPos = LocalToMap(pos);
        Goal = goalPos;
        InitFlowField();
    }

    public bool IsInGrid(Vector2 pos)
    {
        return pos.X >= 0 && pos.X < _cols && pos.Y >= 0 && pos.Y < _rows;
    }

    public void InitFlowField()
    {
        _openList.Clear();
        _closeList.Clear();
        _initMapGridData(_rows, _cols);
        _generatorHeatmap();
        ComputeDirectionVectors();
        _lines.ShowDirLine(DirectionVectors, new Vector2(Goal.X * _tileSize, Goal.Y * _tileSize));
    }

    private void _initMapGridData(int rows, int cols)
    {
        MapGridData = new int[cols, rows];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // 可通行区域
                if (GetCellSourceId(0, new Vector2I(i, j)) == 0)
                {
                    MapGridData[i, j] = -1;
                }

                // 障碍物
                if (GetCellSourceId(1, new Vector2I(i, j)) == 1)
                {
                    MapGridData[i, j] = -10;
                }
            }
        }
    }

    private void _generatorHeatmap()
    {
        int index = (int)(Goal.X + Goal.Y * _cols);
        MapGridData[(int)Goal.X, (int)Goal.Y] = 0;

        _openList.Add(index);
        while (_openList.Count > 0)
        {
            int p = _openList[0];
            _closeList.Add(p);
            _openList.RemoveAt(0);
            int y = p / _cols;
            int x = p - y * _cols;
            _getNeighbor(new Vector2(x, y), MapGridData[x, y]);
        }
    }

    private void _getNeighbor(Vector2 position, int val)
    {
        Vector2[] neighbors = new Vector2[]
        {
            position + new Vector2(1, 0),
            position + new Vector2(-1, 0),
            position + new Vector2(0, 1),
            position + new Vector2(0, -1)
        };

        for (int i = 0; i < neighbors.GetLength(0); i++)
        {
            Vector2 current = neighbors[i];
            if (current.X >= 0 && current.X < _cols && current.Y >= 0 && current.Y < _rows)
            {
                if (MapGridData[(int)current.X, (int)current.Y] == -1)
                {
                    int index = (int)(current.X + current.Y * _cols);
                    if (!_openList.Contains(index) && !_closeList.Contains(index))
                    {
                        _openList.Add(index);
                        MapGridData[(int)current.X, (int)current.Y] = val + 1;
                    }
                }
            }
        }
    }

    private void ComputeDirectionVectors()
    {
        DirectionVectors = new Vector2[MapGridData.GetLength(0)][];
        for (int x = 0; x < MapGridData.GetLength(0); x++)
        {
            DirectionVectors[x] = new Vector2[MapGridData.GetLength(1)];
            for (int y = 0; y < MapGridData.GetLength(1); y++)
            {
                // 计算每个位置的方向向量
                Vector2 direction = CalculateFlowVector(new Vector2(x, y));
                DirectionVectors[x][y] = direction;
            }
        }
    }

    private Vector2 CalculateFlowVector(Vector2 position)
    {
        // 获取邻居块的列表
        List<Vector2> neighbors = GetNeighbors2(position);

        // 寻找下一个波数更小的邻居块
        int minWaveNumber = MapGridData[(int)position.X, (int)position.Y];
        Vector2 nextPosition = position;

        int limitNeighbor = 2;

        for (int i = 0; i < neighbors.Count; i++)
        {
            Vector2 neighbor = neighbors[i];

            int waveNumber = MapGridData[(int)neighbor.X, (int)neighbor.Y];

            // 过滤不可通行
            int a = GetObstacleCount(neighbor);
            if (a >= limitNeighbor)
            {
                continue;
            }

            // 如果邻居的波数更小，则更新流场向量
            if (waveNumber < minWaveNumber)
            {
                Vector2 diagonal1 = new Vector2(neighbor.X, position.Y);
                Vector2 diagonal2 = new Vector2(position.X, neighbor.Y);

                if (IsObstacleTile(diagonal1))
                {
                    minWaveNumber = MapGridData[(int)diagonal2.X, (int)diagonal2.Y];
                    ;
                    nextPosition = diagonal2;
                }
                else if (IsObstacleTile(diagonal2))
                {
                    minWaveNumber = MapGridData[(int)diagonal1.X, (int)diagonal1.Y];
                    ;
                    nextPosition = diagonal1;
                }
                else
                {
                    minWaveNumber = waveNumber;
                    nextPosition = neighbor;
                }
            }
        }

        // 计算流场向量，指向下一个块
        return (nextPosition - position).Normalized();
    }

    // 8个邻居
    private List<Vector2> GetNeighbors2(Vector2 position)
    {
        // 获取相邻块的位置
        List<Vector2> neighbors = new List<Vector2>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // 忽略当前块

                int x = (int)position.X + dx;
                int y = (int)position.Y + dy;

                // 检查是否越界 是否为障碍物
                if (x >= 0 && x < _cols && y >= 0 && y < _rows && MapGridData[x, y] != -10)
                {
                    neighbors.Add(new Vector2(x, y));
                }
            }
        }

        return neighbors;
    }

    private Vector2[] GetNeighbors(Vector2 position)
    {
        return new Vector2[]
        {
            position + new Vector2(1, 0),
            position + new Vector2(-1, 0),
            position + new Vector2(0, 1),
            position + new Vector2(0, -1)
        };
    }

    private int GetObstacleCount(Vector2 neighbor)
    {
        int c = 0;
        Vector2[] neighbors = GetNeighbors(neighbor);
        for (int i = 0; i < neighbors.Length; i++)
        {
            Vector2 n = neighbors[i];

            if (n.X >= 0 && n.X < _cols && n.Y >= 0 && n.Y < _rows)
            {
                if (MapGridData[(int)n.X, (int)n.Y] == -10)
                {
                    c += 1;
                }
            }
        }

        return c;
    }

    private bool IsObstacleTile(Vector2 position)
    {
        return MapGridData[(int)position.X, (int)position.Y] == -10;
    }
}
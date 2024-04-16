using System;
using System.Collections.Generic;
using Godot;

public partial class HexagonMap3 : TileMap
{
    private int _rows;
    private int _cols;

    public override void _Ready()
    {
        _rows = GetUsedRect().End.Y;
        _cols = GetUsedRect().End.X;
    }

    private List<Vector2> _mapToLocal(List<Vector2> pathList)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < pathList.Count; i++)
        {
            list.Add(MapToLocal(new Vector2I((int)pathList[i].X, (int)pathList[i].Y)));
        }

        return list;
    }

    private bool _isObstacle(Vector2 pos)
    {
        return GetCellSourceId(1, new Vector2I((int)pos.X, (int)pos.Y)) == 1;
    }

    private bool _checkTarget(Vector2 pos)
    {
        if (pos.X >= _cols || pos.Y >= _rows)
        {
            return true;
        }

        return false;
    }

    public List<Vector2> FindPath( Vector2 s, Vector2 e)
    {
        Vector2 start = LocalToMap(s);
        Vector2 end = LocalToMap(e);
        GD.Print("start: " + start + ", end: " + end);

        if (_checkTarget(end))
        {
            return null;
        }

        HexagonNode startNode = new HexagonNode(start);
        HexagonNode endNode = new HexagonNode(end);
        List<HexagonNode> openList = new List<HexagonNode>{startNode};
        List<HexagonNode> closedList = new List<HexagonNode>();

        startNode.G = 0;
        startNode.H = (int)Heuristic(startNode, endNode);
        
        while (openList.Count > 0)
        {
            HexagonNode currentNode = openList[0];

            // 寻找F值最小的节点
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Position == endNode.Position)
            {
                return ReconstructPath(currentNode);
            }

            List<HexagonNode> neighbors = GetNeighbourList(currentNode);

            foreach (HexagonNode neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }
                
                if (_isObstacle(neighbor.Position))
                {
                    closedList.Add(neighbor);
                    continue;
                }

                double tentativeG = currentNode.G + 1; // 假设每一步的代价是1

                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.Parent = currentNode;
                    neighbor.G =(int) tentativeG;
                    neighbor.H = (int)Heuristic(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // 如果找不到路径，则返回null
    }

    private List<Vector2> ReconstructPath(HexagonNode node)
    {
        List<HexagonNode> path = new List<HexagonNode>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }

        path.Reverse();

      // List<HexagonNode> s =  SmoothPath(path);

        List<Vector2> finalPath = new List<Vector2>(); 
        for (var i = 0; i < path.Count; i++)
        {
            finalPath.Add(path[i].Position);
        }
        
        return _mapToLocal(finalPath);
    }
    
    private List<HexagonNode> GetNeighbourList(HexagonNode currentNode)
    {
        List<HexagonNode> neighbourList = new List<HexagonNode>();

        bool oddRow = currentNode.y % 2 == 1;

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(new HexagonNode(currentNode.x - 1, currentNode.y));
        }

        if (currentNode.x + 1 < _cols)
        {
            // Right
            neighbourList.Add(new HexagonNode(currentNode.x + 1, currentNode.y));
        }

        if (currentNode.y - 1 >= 0)
        {
            // Down
            neighbourList.Add(new HexagonNode(currentNode.x, currentNode.y - 1));
        }

        if (currentNode.y + 1 < _rows)
        {
            // Up
            neighbourList.Add(new HexagonNode(currentNode.x, currentNode.y + 1));
        }

        if (oddRow)
        {
            if (currentNode.y + 1 < _rows && currentNode.x + 1 < _cols)
            {
                neighbourList.Add(new HexagonNode(currentNode.x + 1, currentNode.y + 1));
            }

            if (currentNode.y - 1 >= 0 && currentNode.x + 1 < _cols)
            {
                neighbourList.Add(new HexagonNode(currentNode.x + 1, currentNode.y - 1));
            }
        }
        else
        {
            if (currentNode.y + 1 < _rows && currentNode.x - 1 >= 0)
            {
                neighbourList.Add(new HexagonNode(currentNode.x - 1, currentNode.y + 1));
            }

            if (currentNode.y - 1 >= 0 && currentNode.x - 1 >= 0)
            {
                neighbourList.Add(new HexagonNode(currentNode.x - 1, currentNode.y - 1));
            }
        }

        return neighbourList;
    }

    static double Heuristic(HexagonNode node1, HexagonNode node2)
    {
        // 使用曼哈顿距离作为启发式估计
        return Math.Abs(node1.Position.X - node2.Position.X) + Math.Abs(node1.Position.Y - node2.Position.Y);
    }

}
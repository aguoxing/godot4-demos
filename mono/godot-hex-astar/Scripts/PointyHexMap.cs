using System;
using System.Collections.Generic;
using Godot;

public partial class PointyHexMap : TileMap
{
    private const int MOVE_STRAIGHT_COST = 1;
    private int _rows;
    private int _cols;

    public override void _Ready()
    {
        _rows = GetUsedRect().End.Y;
        _cols = GetUsedRect().End.X;
    }

    private bool _isObstacle(Vector2 pos)
    {
        return GetCellSourceId(1, new Vector2I((int)pos.X, (int)pos.Y)) == 1;
    }

    private bool _checkTarget(Vector2 pos)
    {
        if (pos.X < 0 || pos.X >= _cols || pos.Y < 0 || pos.Y >= _rows)
        {
            GD.Print("超出边界");
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
        startNode.H = CalculateHeuristicDistanceCost(startNode, endNode);

        while (openList.Count > 0)
        {
            HexagonNode currentNode = GetLowestFCostNode(openList);

            if (currentNode.Position == endNode.Position)
            {
                GD.Print("end");
                return ReconstructPath(currentNode);
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            
            foreach (HexagonNode neighbor in GetNeighbourList(currentNode))
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

                double tentativeG = currentNode.G + MOVE_STRAIGHT_COST;
                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
    
    private HexagonNode GetLowestFCostNode(List<HexagonNode> pathNodeList)
    {
        HexagonNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].F < lowestFCostNode.F)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
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

        List<Vector2> finalPath = new List<Vector2>(); 
        for (var i = 0; i < path.Count; i++)
        {
            finalPath.Add(MapToLocal(new Vector2I((int)path[i].Position.X, (int)path[i].Position.Y)));
        }
        
        return finalPath;
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

    private int CalculateHeuristicDistanceCost(HexagonNode a, HexagonNode b)
    {
        return Mathf.RoundToInt(MOVE_STRAIGHT_COST * a.Position.DistanceTo(b.Position));
        // int xDistance = (int)Mathf.Abs(a.Position.X - b.Position.X);
        // int yDistance = (int)Mathf.Abs(a.Position.Y - b.Position.Y);
        // int remaining = Mathf.Abs(xDistance - yDistance);
        // return MOVE_STRAIGHT_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        
        // return Math.Abs(a.Position.X - b.Position.X) + Math.Abs(a.Position.Y - b.Position.Y);
    }
    
    static double Heuristic(HexagonNode node1, HexagonNode node2)
    {
        // 使用曼哈顿距离作为启发式估计
        return Math.Abs(node1.Position.X - node2.Position.X) + Math.Abs(node1.Position.Y - node2.Position.Y);
    }
}
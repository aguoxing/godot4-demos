using System;
using Godot;
using System.Collections.Generic;
using RVO;
using Vector2 = RVO.Vector2;

public partial class Enemies : Node2D
{
    private Random _random;
    public FlowField flowField;
    private int _enemyCount = 0;
    private int[,] _mapGrid;

    public override void _Ready()
    {
        _random = new Random();
    }

    public override void _Process(double delta)
    {
        Simulator.Instance.doStep();
    }

    public void SpawnEnemy(Godot.Vector2 pos)
    {
        // todo 待完善
        int agentId = Simulator.Instance.addAgent(new Vector2(pos.X, pos.Y));

        Enemy enemy = GD.Load<PackedScene>("res://Scenes/Enemy.tscn").Instantiate<Enemy>();
        enemy.Id = agentId;
        enemy.Position = flowField.GetMousePosition(pos);
        enemy.flowField = flowField;
        AddChild(enemy);
    }

    public void InitRVO2()
    {
        int[,] mapGrid = flowField.MapGridData;
        _mapGrid = mapGrid;

        Simulator.Instance.setTimeStep(0.25f);
        Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 2.0f, 2.0f, new Vector2(0.0f, 0.0f));

        // 障碍物
        // for (int i = 0; i < mapGrid.GetLength(0); i++)
        // {
        //     for (int j = 0; j < mapGrid.GetLength(1); j++)
        //     {
        //         if (mapGrid[i, j] == -10)
        //         {
        //             // 多边形障碍物 逆时针
        //             IList<Vector2> obstacle1 = new List<Vector2>();
        //             obstacle1.Add(new Vector2((i + 1) * 16, j * 16));
        //             obstacle1.Add(new Vector2(i * 16, j * 1));
        //             obstacle1.Add(new Vector2(i * 16, (j + 1) * 16));
        //             obstacle1.Add(new Vector2((i + 1) * 16, (j + 1) * 16));
        //             Simulator.Instance.addObstacle(obstacle1);
        //         }
        //     }
        // }

        IList<Vector2> obstacle1 = new List<Vector2>();
        obstacle1.Add(new Vector2(10.0f, 80.0f));
        obstacle1.Add(new Vector2(40.0f, 80.0f));
        obstacle1.Add(new Vector2(40.0f, 20.0f));
        obstacle1.Add(new Vector2(10.0f, 20.0f));
        Simulator.Instance.addObstacle(obstacle1);

        Simulator.Instance.processObstacles();
    }
}
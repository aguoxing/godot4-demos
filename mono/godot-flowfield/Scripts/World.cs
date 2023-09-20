using Godot;
using System;
using RVO;

public partial class World : Node2D
{
    private FlowField _flowField;
    private Enemies _enemies;
    private int _selectedIdx = 0;

    public override void _Ready()
    {
        _flowField = GetNode<FlowField>("FlowField");
        _enemies = GetNode<Enemies>("Enemies");
        
        _flowField.InitFlowField();
        _enemies.flowField = _flowField;
        _enemies.InitRVO2();
    }

    public override void _Process(double delta)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("mouse_left"))
        {
            if (_selectedIdx == 0)
            {
                _flowField.SetObstacleTile(GetGlobalMousePosition());
                _flowField.InitFlowField();
                _enemies.flowField = _flowField;
            }

            if (_selectedIdx == 1)
            {
                _enemies.SpawnEnemy(GetGlobalMousePosition());
            }
        }

        if (@event.IsActionPressed("mouse_right"))
        {
            _flowField.SetGoal(GetGlobalMousePosition());
            _enemies.flowField = _flowField;
        }
    }
    
    private void _optionChange(int idx)
    {
        _selectedIdx = idx;
    }
}
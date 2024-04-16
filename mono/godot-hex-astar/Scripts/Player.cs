using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
    private List<Vector2> _pathList = new List<Vector2>();
    private int _currentPathIndex = 0;
    private float _maxSpeed = 80.0f;
    private float _friction = 500.0f;
    private float _acceleration = 500.0f;

    public override void _Ready()
    {
        // _pathList.Add(GlobalPosition);
        // _pathList.Add(GlobalPosition + Vector2.Down);
        // _pathList.Add(GlobalPosition + Vector2.Down);
        // _pathList.Add(GlobalPosition + Vector2.Down);
        // GD.Print(Position);
        // GD.Print(GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Vector2 velocity = Velocity;
        //
        // Vector2 inputDirection = new Vector2(
        //     Input.GetActionStrength("move_right") - Input.GetActionRawStrength("move_left"),
        //     Input.GetActionStrength("move_down") - Input.GetActionRawStrength("move_up")
        // );
        //
        // inputDirection = inputDirection.Normalized();
        //
        // if (inputDirection != Vector2.Zero)
        // {
        //     velocity = velocity.MoveToward(inputDirection * (int)_maxSpeed, (float)(_acceleration * delta));
        // }
        // else
        // {
        //     velocity = velocity.MoveToward(Vector2.Zero, (float)(_friction * delta));
        // }
        //
        // Velocity = velocity;
        // MoveAndSlide();
        
        
        if (_pathList != null && _currentPathIndex < _pathList.Count)
        {
            // Vector2 moveDirection = (targetPosition - GlobalPosition).Normalized();
            Vector2 moveDirection = GlobalPosition.DirectionTo(_pathList[_currentPathIndex]).Normalized();
            
            Vector2 velocity = Velocity;
            velocity = velocity.MoveToward(moveDirection * (int)_maxSpeed, (float)(_acceleration * delta));
            
            Velocity = velocity;
            MoveAndSlide();

            // 检查是否已到达目标节点
            float distanceToTarget = GlobalPosition.DistanceTo(_pathList[_currentPathIndex]);
            if (distanceToTarget < 1.0f)
            {
                // 移动到下一个路径节点
                _currentPathIndex++;
            }
        }
    }

    public void SetPath(List<Vector2> pathList)
    {
        _pathList.Clear();
        _pathList = pathList;
        _currentPathIndex = 0;
    }
    
}
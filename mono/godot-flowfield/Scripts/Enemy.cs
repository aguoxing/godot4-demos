using Godot;
using System;
using RVO;
using Vector2 = Godot.Vector2;

public partial class Enemy : CharacterBody2D
{
    private Random _random = new Random();
    public int Id = -1;
    private float _maxSpeed = 80.0f;
    private float _friction = 500.0f;
    private float _acceleration = 500.0f;
    private Vector2 _moveDir = Vector2.Zero;

    public FlowField flowField;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        _moveDir = Vector2.Zero;

        if (Id != -1)
        {
            // RVO.Vector2 pos = Simulator.Instance.getAgentPosition(Id);
            // RVO.Vector2 vel = Simulator.Instance.getAgentPrefVelocity(Id);
            // Position = new Vector2(pos.x(), pos.y());
            // if (Math.Abs(vel.x()) > 0.01f && Math.Abs(vel.y()) > 0.01f)
                // _moveDir = new Vector2(vel.x(),  vel.y()).Normalized();
            
            // RVO.Vector2 goalVector = new RVO.Vector2(flowField.Goal.X, flowField.Goal.Y) - Simulator.Instance.getAgentPosition(Id);
            RVO.Vector2 goalVector = new RVO.Vector2(GetGlobalMousePosition().X, GetGlobalMousePosition().Y) - Simulator.Instance.getAgentPosition(Id);

            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }

            Simulator.Instance.setAgentPrefVelocity(Id, goalVector);

            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
            float angle = (float)_random.NextDouble() * 2.0f * (float)Math.PI;
            float dist = (float)_random.NextDouble() * 0.0001f;

            Simulator.Instance.setAgentPrefVelocity(Id, Simulator.Instance.getAgentPrefVelocity(Id) +
                                                        dist * new RVO.Vector2((float)Math.Cos(angle),
                                                            (float)Math.Sin(angle)));
            
            // _moveDir = new Vector2(goalVector.x(), goalVector.y());
            // _moveDir = new Vector2(Simulator.Instance.getAgentPrefVelocity(Id).x(), Simulator.Instance.getAgentPrefVelocity(Id).y());
        }


        Vector2 enemyPos = flowField.LocalToMap(Position);
        if (flowField.IsInGrid(enemyPos))
        {
            _moveDir = flowField.DirectionVectors[(int)enemyPos.X][(int)enemyPos.Y];
        }

        Vector2 velocity = Velocity;
        if (_moveDir != Vector2.Zero)
        {
            float rotationAngle = Mathf.Atan2(_moveDir.Y, _moveDir.X);
            Rotation = rotationAngle;
            velocity = velocity.MoveToward(_moveDir * (int)_maxSpeed, (float)(_acceleration * delta));
        }
        else
        {
            velocity = velocity.MoveToward(Vector2.Zero, (float)(_friction * delta));
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
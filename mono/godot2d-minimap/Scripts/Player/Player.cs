using Godot;

public partial class Player : CharacterBody2D
{
    public const float MAXSPEED = 80.0f;
    public const float ROLLSPEED = 125.0f;

    // 摩擦
    public float FRICTION = 500.0f;

    // 加速度
    public float ACCELERATION = 500.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        Vector2 inputDirection = new Vector2(
            Input.GetActionStrength("right") - Input.GetActionRawStrength("left"),
            Input.GetActionStrength("down") - Input.GetActionRawStrength("up")
        );

        inputDirection = inputDirection.Normalized();

        if (inputDirection != Vector2.Zero)
        {
            velocity = velocity.MoveToward(inputDirection * (int)MAXSPEED, (float)(ACCELERATION * delta));
        }
        else
        {
            velocity = velocity.MoveToward(Vector2.Zero, (float)(FRICTION * delta));
        }

        Velocity = velocity;
        MoveAndSlide();
        
        GlobalData.Instance.PlayerPosition = Position;
    }
}
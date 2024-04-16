using Godot;

public partial class Player : CharacterBody2D
{
    private float _maxSpeed = 80.0f;
    private float _friction = 500.0f;
    private float _acceleration = 500.0f;

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
            velocity = velocity.MoveToward(inputDirection * (int)_maxSpeed, (float)(_acceleration * delta));
        }
        else
        {
            velocity = velocity.MoveToward(Vector2.Zero, (float)(_friction * delta));
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
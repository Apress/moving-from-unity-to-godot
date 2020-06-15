using Godot;
using System;

public class PlayerControl : KinematicBody2D
{
    [Export]
    public float MoveSpeed = 2f;

    private Vector2 InputDir = Vector2.Zero;
    private AnimatedSprite AnimSprite = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimSprite = GetNode("AnimatedSprite") as AnimatedSprite;
    }

    public override void _Process(float delta)
    {
        InputDir.x = -Input.GetActionStrength("Left") + Input.GetActionStrength("Right");
        InputDir.y = Input.GetActionStrength("Down") + -Input.GetActionStrength("Up");

        if(InputDir.x > 0) AnimSprite.FlipH = false;
        if(InputDir.x < 0) AnimSprite.FlipH = true;

        InputDir = InputDir * MoveSpeed;

        if(InputDir.LengthSquared() <= 0)
            AnimSprite.Play("Idle");
        else
            AnimSprite.Play("Walk");
    }

    public override void _PhysicsProcess(float delta)
    {
        MoveAndSlide(InputDir);
    }
}

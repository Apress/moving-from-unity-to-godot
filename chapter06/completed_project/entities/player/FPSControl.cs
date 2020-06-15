using Godot;
using System;

public class FPSControl : KinematicBody
{
    [Export]
    private string LeftAxis, RightAxis, UpAxis, DownAxis = string.Empty;

    private Vector3 MoveDirection = Vector3.Zero;
    private Vector3 FloorNormal = Vector3.Up;

    [Export]
    public float MoveSpeed = 10f;

    [Export]
    public Vector3 Gravity = Vector3.Zero;

    private bool Grounded = false;

    [Export]
    private float Acceleration = 8f;

    [Export]
    private float JumpPower = 15f;
    public Vector3 Velocity;

    [Export]
    public float FloorMaxAngle = 45f;

    private Vector3 Snap = Vector3.Down;

    private Vector2 MouseMove = Vector2.Zero;

    [Export]
    public float MouseSensitvity = 10f;

    [Export]
    public NodePath HeadPath;
    private Spatial HeadNode;

    [Export]
    public NodePath CamPath;
    private Spatial CamNode;

    [Export]
    public float HeadAmplitude, HeadFrequency = 1f;
    private float BobHeight = 0f;

    [Export]
    public float PitchLimit = 45f;

    [Export]
    public float RunMultiplier = 1f;
    private float SpeedMultiplier = 1f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HeadNode = GetNode(HeadPath) as Spatial;
        CamNode = GetNode(CamPath) as Spatial;
        Input.SetMouseMode(Input.MouseMode.Captured);

    }

    public override void _PhysicsProcess(float delta)
    {
        Vector3 TargetVel = Velocity;
        Grounded = IsOnFloor();

        TargetVel.x = MoveDirection.x * SpeedMultiplier * MoveSpeed;
        TargetVel.z = MoveDirection.z * SpeedMultiplier * MoveSpeed;
        
        Velocity = Velocity.LinearInterpolate(TargetVel, Acceleration * delta);
        Velocity += Gravity * delta;
        Velocity.y = MoveAndSlideWithSnap(Velocity, Snap, FloorNormal, true, 4, Mathf.Deg2Rad(FloorMaxAngle)).y;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float Vertical = -Input.GetActionStrength(DownAxis) + Input.GetActionStrength(UpAxis);
        float Horizontal = Input.GetActionStrength(LeftAxis) + -Input.GetActionStrength(RightAxis);

        MoveDirection = Vector3.Zero;
        MoveDirection = HeadNode.Transform.basis.z * Vertical + HeadNode.Transform.basis.x * Horizontal;
        MoveDirection = MoveDirection.Normalized();
        HeadBob(delta);

        if(Input.IsActionJustPressed("move_jump") && Grounded)
        {
            Velocity.y = JumpPower;
            Snap = Vector3.Zero;
        }
        else
            Snap = Vector3.Down;

        if(Input.IsActionPressed("move_sprint"))
            SpeedMultiplier = RunMultiplier;
        else
            SpeedMultiplier = 1f;

        if(MouseMove.Length() <= 0) return;

        Vector2 MouseResult = MouseMove * MouseSensitvity * delta;
        MouseResult = new Vector2(MouseResult.x, Mathf.Clamp(MouseResult.y, -PitchLimit, PitchLimit));
        HeadNode.Transform = HeadNode.Transform.Rotated(Vector3.Up, -Mathf.Deg2Rad(MouseResult.x));
        HeadNode.Transform = HeadNode.Transform.Rotated(HeadNode.Transform.basis.x, Mathf.Deg2Rad(MouseResult.y));
        HeadNode.RotationDegrees = new Vector3(Mathf.Clamp(HeadNode.RotationDegrees.x, -PitchLimit,PitchLimit),
                                               HeadNode.RotationDegrees.y, HeadNode.RotationDegrees.z);
        MouseMove = Vector2.Zero;
    }

    private void HeadBob(float delta)
    {
        Transform T = CamNode.Transform;

        if(MoveDirection.y != 0 && Grounded)
        {
            BobHeight += delta;
            BobHeight = Mathf.Wrap(BobHeight,0f,360f);
            float LerpedHeight = Mathf.Sin(BobHeight * HeadFrequency) * HeadAmplitude;
            T.origin.y = LerpedHeight;
            CamNode.Transform = T;
            return;
        }
        
        T.origin.y = Mathf.Lerp(T.origin.y, 0f, delta);
        CamNode.Transform = T;
    }

    public override void _Input(InputEvent motionUnknown)
    {
        InputEventMouseMotion motion = motionUnknown as InputEventMouseMotion;

        if (motion != null)
            MouseMove = motion.Relative;
    }
}

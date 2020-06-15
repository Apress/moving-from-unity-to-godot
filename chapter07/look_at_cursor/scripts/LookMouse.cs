using Godot;
using System;

public class LookMouse : Sprite
{
     private float StartRot = 0f;
      public override void _Ready()
      {
          StartRot = Rotation;
      }

    public override void _Process(float delta)
    {
          Vector2 CursorPos = GetLocalMousePosition();

          Rotation += CursorPos.Angle() + StartRot;
          Rotation = Mathf.Wrap(Rotation, Mathf.Deg2Rad(-360),Mathf.Deg2Rad(360));
    }
}

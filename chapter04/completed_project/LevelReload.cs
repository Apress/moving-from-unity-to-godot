using Godot;
using System;

public class LevelReload : Timer
{
    public void _on_Timer_timeout()
    {
        GetTree().ReloadCurrentScene();
    }
}

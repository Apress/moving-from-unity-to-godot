using Godot;
using System;

public class Pickup : Area2D
{
    public void _on_Pickup_body_entered(Node N)
    {
        if(!N.IsInGroup("Player"))return;

        //Check pickups remaining
        if(GetTree().GetNodesInGroup("Pickup").Count <= 1)
        {
            //You completed the level
            GD.Print("Level Completed");
        }

        CallDeferred("RemoveObject");
    }

    public void RemoveObject()
    {
        GetParent().RemoveChild(this);
    }
}

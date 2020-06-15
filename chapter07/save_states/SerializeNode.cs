using Godot;
using System;

public class SerializeNode : Sprite
{
    public Godot.Collections.Dictionary<string, object> Save()
    {
        return new Godot.Collections.Dictionary<string, object>()
        {
            {"Filename", Filename},
            {"Parent", GetParent().GetPath()},
            {"PositionX", Position.x},
            {"PositionY", Position.y},
            {"ModColor", Modulate.ToHtml(true)}
        };
    }

    public void Load(Godot.Collections.Dictionary<string, object> SavedData)
    {
        Position = new Vector2((float)SavedData["PositionX"],(float)SavedData["PositionY"]);
        Modulate = new Color((string)SavedData["ModColor"]);
    }
}

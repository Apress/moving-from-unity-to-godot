using Godot;
using System;

public class SaveState : Node
{
    //Name of file for saving and loading
    public string SGDName = "GameData.sav";
    public void Save()
    {
        //Open file for writing
        File SaveFile = new File();
        SaveFile.Open("user://"+SGDName, File.ModeFlags.Write);

        //Find all objects in scene to be saved
        Godot.Collections.Array Nodes = GetTree().GetNodesInGroup("persistent");

        //For each object, get its JSON representation
        foreach(Node N in Nodes)
        {
            if(N.Filename.Empty())
                continue;

            if(!N.HasMethod("Save"))
                continue;

            //Save JSON to file
            var SaveData = N.Call("Save");
            SaveFile.StoreLine(JSON.Print(SaveData));
        }

        //Close file
        SaveFile.Close();
    }

    public void Load()
    {
        File SaveFile = new File();

        if(!SaveFile.FileExists("user://"+SGDName))
            return;
        
        //Open file for reading data
        SaveFile.Open("user://"+SGDName, File.ModeFlags.Read);

        //Remove any duplicate objects already in scene
        Godot.Collections.Array Nodes = GetTree().GetNodesInGroup("persistent");
        foreach(Node N in Nodes)
            N.QueueFree();

        //Loop through file
        while(SaveFile.GetPosition() < SaveFile.GetLen())
        {
            string Line = SaveFile.GetLine();

            if(Line.Empty())break;

            Godot.Collections.Dictionary NodeData = (Godot.Collections.Dictionary)JSON.Parse(Line).Result;

            //Load objects back into scene
            PackedScene PS = (PackedScene)ResourceLoader.Load(NodeData["Filename"].ToString());
            Node NewScene = PS.Instance();
            Node ParentNode = GetNode(NodeData["Parent"].ToString())as Node;
            ParentNode.AddChild(NewScene);

            if(NewScene.HasMethod("Load"))
                NewScene.Call("Load", NodeData);
        }

        //Close file
        SaveFile.Close();
    }

    //Test functionality. Can be removed
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            //Pressing S will save
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.S)
            {
                Save();
                return;
            }
        
            //Pressing L will load
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.L)
            {
                Load();
                return;
            }
        }
    }
}

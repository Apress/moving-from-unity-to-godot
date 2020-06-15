using Godot;
using System;

public class ConfigLoader : Node
{
    private ConfigFile CF = new ConfigFile();
    public string CFName = "settings.cfg";

    public override void _Ready()
    {
        //GD.Print(OS.GetUserDataDir()+CFName);
        if(CF.Load("user://"+CFName)!=Error.Ok)
            SaveDefault();
    }

    public void SaveDefault()
    {
        CF.SetValue("Main","ResolutionWidth",1920);
        CF.Save("user://"+CFName); 
    }
}

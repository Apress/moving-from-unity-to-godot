using Godot;
using System;

public class TextureProgressMain : TextureProgress
{
    [Export]
    public string ScenePath;

    [Export]
    public uint MinimumTime = 2000;
    private uint TimeStart = 0;

    private Thread ThisThread = null;

    private void ThreadLoad(string ResPath)
    {
        ResourceInteractiveLoader RIL = ResourceLoader.LoadInteractive(ResPath);
        if(RIL==null)return;

        int StageCount = 0;

        while(true)
        {
            uint TimeElapsed = OS.GetTicksMsec() - TimeStart;
            float TimeProgress = (float)TimeElapsed/(float)(MinimumTime*1000f)*(float)MaxValue;
            
            if(StageCount < RIL.GetStageCount())
                StageCount = RIL.GetStage();

            float LoadProgress = (float)StageCount/(float)RIL.GetStageCount()*(float)MaxValue;
            CallDeferred("set_value", (TimeProgress<LoadProgress)?TimeProgress:LoadProgress);

            //Take a break
            OS.DelayMsec(100);
            
            if(Value>=MaxValue)
            {
                CallDeferred("ThreadDone", RIL.GetResource() as PackedScene);
                return;
            }

            if(StageCount >= RIL.GetStageCount())
                continue;

            //Poll current stats
            Error PollData = RIL.Poll();

            if(PollData == Error.FileEof)
            {
                StageCount = RIL.GetStageCount();
                continue;
            }

            if(PollData != Error.Ok)
            {
                GD.Print("Error Loading");
                return;
            }
        }
    }

    private void ThreadDone(PackedScene R)
    {
        ThisThread.WaitToFinish();
        SceneTree ST = GetTree();

        Node Root = R.Instance();

        ST.CurrentScene.Free();
        ST.CurrentScene = null;
        ST.Root.AddChild(Root);
        ST.CurrentScene = Root;
    }

    public override void _Ready()
    {
        TimeStart = OS.GetTicksMsec();
        ThisThread = new Thread();
        ThisThread.Start(this, "ThreadLoad", ScenePath);
    }
}

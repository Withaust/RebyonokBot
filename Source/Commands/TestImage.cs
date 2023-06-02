using Godot;
using System;

public class TestImage : INode<TestImage>
{
    Control CurrentControl;

    public async void Save()
    {
        await ToSignal(VisualServer.Singleton, "frame_post_draw");
        Image Image = GetViewport().GetTexture().GetData();
        Image.FlipY();
        Image.SavePng("test.png");
    }

    public override bool OnReady()
    {
        CurrentControl = GetNode<Control>("TestImage");
        OS.WindowSize = CurrentControl.RectSize;
        GetViewport().RenderTargetClearMode = Viewport.ClearMode.OnlyNextFrame;
        //CallDeferred(nameof(Save));
        return true;
    }

    public override bool OnProcess(float Delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        
        return true;
    }
}

using System.Threading.Tasks;
using Godot;

public class Scenes : INode<Scenes>
{
    private Vector2 DefaultWindowSize;

    public Control CurrentScene { get; private set; }

    public override bool OnReady()
    {
        DefaultWindowSize = OS.WindowSize;
        OS.WindowPosition = default;
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

    public void LoadScene(string ScenePath)
    {
        PackedScene NewScene = ResourceLoader.Load<PackedScene>("res://Scenes/" + ScenePath + ".tscn");
        CurrentScene = (Control)NewScene.Instance();
        AddChild(CurrentScene);
        OS.WindowSize = CurrentScene.RectSize;
        OS.WindowPosition = default;
    }

    public void UnloadScene()
    {
        if (CurrentScene == null)
        {
            return;
        }
        CurrentScene.QueueFree();
        CurrentScene = null;
        OS.WindowSize = DefaultWindowSize;
        OS.WindowPosition = default;
    }

    public async Task<Image> Render()
    {
        GetViewport().RenderTargetClearMode = Viewport.ClearMode.OnlyNextFrame;
        await ToSignal(VisualServer.Singleton, "frame_post_draw");
        Image Image = GetViewport().GetTexture().GetData();
        Image.FlipY();
        //OnRenderFinish.Invoke(Image);
        // We clear the event subscribers after firing so that there
        // wont be accidental callback firing to loaded but not currently
        // used nodes
        //OnRenderFinish.Clear();
        UnloadScene();
        return Image;
    }
}

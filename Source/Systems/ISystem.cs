using Godot;

public interface ISystemEvents
{
    bool OnReady();
    bool OnShutdown();
}

public abstract class ISystem<T> : Node, ISystemEvents where T : Node
{
    private static Node Root;

    public abstract bool OnReady();
    public abstract bool OnShutdown();
    
    public override void _Ready()
    {
        Core.Initialize();
        Core.Register(this);
        Root = GetTree().Root;
        if(!OnReady())
        {
            GD.PushError(GetType().Name + " failed to execute OnReady");
        }
    }

    public override void _ExitTree()
    {
        Core.Deinitialize();
    }

    public static T Get()
    {
        NodePath Path = typeof(T).Name;
        T Target = Root.GetNode<T>(Path);
        return Target;
    }
}

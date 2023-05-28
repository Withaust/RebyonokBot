using Godot;

public interface ISystemEvents
{
    void OnReady();
    void OnShutdown();
}

public abstract class ISystem<T> : Node, ISystemEvents where T : Node
{
    private static Node Root;

    public abstract void OnReady();
    public abstract void OnShutdown();
    
    public override void _Ready()
    {
        Core.Initialize();
        Core.Register(this);
        Root = GetTree().Root;
        OnReady();
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

using Godot;

public interface INodeEvents
{
    bool OnReady();
    bool OnProcess(float Delta);
    bool OnShutdown();
}

public abstract class INode<T> : Node, INodeEvents where T : Node
{
    public abstract bool OnReady();
    public abstract bool OnProcess(float Delta);
    public abstract bool OnShutdown();

    public static T Get()
    {
        NodePath Path = typeof(T).Name;
        T Target = Core.Instance.GetNode<T>(Path);
        return Target;
    }
}

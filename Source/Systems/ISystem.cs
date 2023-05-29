using Godot;

public interface ISystemEvents
{
    bool OnReady();
    bool OnProcess(float Delta);
    bool OnShutdown();
}

public abstract class ISystem<T> : Node, ISystemEvents where T : Node
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

using Godot;
using System;
using System.Collections.Generic;

public enum CoreExceptionType
{
    OnReady,
    OnProcess,
    OnShutdown
}

public delegate void OnCoreException(CoreExceptionType Type, Exception Exception);

public class Core : Node
{
    public static Core Instance { get; private set; }

    public Dictionary<Type, ISystemEvents> SystemsDict;
    public List<ISystemEvents> SystemsList;

    public event OnCoreException OnException;

    public override void _Ready()
    {
        Instance = this;
        try
        {
            SystemsDict = new Dictionary<Type, ISystemEvents>();
            SystemsList = new List<ISystemEvents>();
            AllSystems.Register();
        }
        catch (Exception Exception)
        {
            OnException?.Invoke(CoreExceptionType.OnReady, Exception);
        }
        Logger.Get().Log("RebyonokBot is now fully operational");
    }

    public override void _Process(float Delta)
    {
        try
        {
            for (int i = 0; i < SystemsList.Count; i++)
            {
                ISystemEvents Target = SystemsList[i];
                if (!Target.OnProcess(Delta))
                {
                    GD.PushError(Target.GetType().Name + " failed to execute OnDelta");
                }
            }
        }
        catch (Exception Exception)
        {
            OnException?.Invoke(CoreExceptionType.OnProcess, Exception);
        }
    }

    public override void _ExitTree()
    {
        try
        {
            for (int i = SystemsList.Count - 1; i >= 0; i--)
            {
                ISystemEvents Target = SystemsList[i];
                if (!Target.OnShutdown())
                {
                    GD.PushError(Target.GetType().Name + " failed to execute OnShutdown");
                }
                SystemsList.RemoveAt(i);
                SystemsDict.Remove(Target.GetType());
            }
        }
        catch (Exception Exception)
        {
            OnException?.Invoke(CoreExceptionType.OnShutdown, Exception);
        }
    }

    public void Register<T>(bool LoadScene = false) where T : Node, ISystemEvents, new()
    {
        T NewSystem = new T();
        SystemsDict[NewSystem.GetType()] = NewSystem;
        SystemsList.Add(NewSystem);
        NewSystem.Name = typeof(T).Name;
        AddChild(NewSystem);
        if (LoadScene)
        {
            PackedScene NewScene = ResourceLoader.Load<PackedScene>("res://Systems/" + typeof(T).Name + ".tscn");
            NewSystem.AddChild(NewScene.Instance());
        }
        if (!NewSystem.OnReady())
        {
            GD.PushError(typeof(T).Name + " failed to execute OnReady");
        }
    }
}

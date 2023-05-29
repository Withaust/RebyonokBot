using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using SysThread = System.Threading.Thread;

public enum CoreExceptionType
{
    OnReady,
    OnProcess,
    OnShutdown
}

public class Core : Node
{
    public static Core Instance { get; private set; }

    public Dictionary<Type, ISystemEvents> SystemsDict;
    public List<ISystemEvents> SystemsList;

    public Event<CoreExceptionType, Exception> OnException;

    public string[] WakeUp = { "Да я здесь!", "Да всем здесь я привет!", "Я проснулся!", "Да я проснулся да!", "Я здесь!", "Всем привет!", "Сука встал!" };


    public override void _Ready()
    {
        Instance = this;

        SystemsDict = new Dictionary<Type, ISystemEvents>();
        SystemsList = new List<ISystemEvents>();
        OnException = new Event<CoreExceptionType, Exception>();

        bool gotCapcha = false;
        bool registered = false;

        do
        {
            try
            {
                if (!registered)
                {
                    AllSystems.Register();
                }
                registered = true;
                Logger.Get().Log("RebyonokBot is now fully operational");
                Vk.Get().SendMessage(WakeUp[(int)GD.RandRange(0, WakeUp.Length)]);
            }
            catch (Exception Exception)
            {
                if (Exception is VkNet.Exception.CaptchaNeededException)
                {
                    int SleepingTime = (int)GD.RandRange(5000.0, 15000.0);
                    GD.Print("Got capcha, sleeping for " + SleepingTime + "ms and retrying");
                    gotCapcha = true;
                    SysThread.Sleep(SleepingTime);
                    return;
                }
                OnException.Invoke(CoreExceptionType.OnReady, Exception);
            }
            gotCapcha = false;
        }
        while (gotCapcha);
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
            if (Exception is VkNet.Exception.CaptchaNeededException)
            {
                GD.Print("Got capcha, restarting...");
                GetTree().Quit(1);
                return;
            }
            OnException.Invoke(CoreExceptionType.OnProcess, Exception);
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
            OnException.Invoke(CoreExceptionType.OnShutdown, Exception);
        }
        if (OS.ExitCode != 0)
        {
            return;
        }
        Vk.Get().SendMessage("В пизду, срать пора.");
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

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

    public Dictionary<Type, INodeEvents> SystemsDict;
    public List<INodeEvents> SystemsList;

    private int ExceptionCounter = 0;
    private int MaxExceptionCount = 3;
    public Event<CoreExceptionType, Exception> OnException  { get; private set; }

    public string[] Wakeup = { "Да я здесь!", "Да всем здесь я привет!", "Я проснулся!", "Да я проснулся да!", "Я здесь!", "Всем привет!", "Сука встал!" };
    public string[] Shutdown = { "Gracefully уснул...", "Всё я сплю...", "Пошёл в макдоналдс и уснул...", "Я сплю...", "Zzzz...", "Всем пока...", "В пизду, срать пора..." };

    public override void _Ready()
    {
        Instance = this;

        SystemsDict = new Dictionary<Type, INodeEvents>();
        SystemsList = new List<INodeEvents>();
        OnException = new Event<CoreExceptionType, Exception>();

        bool gotCapcha = false;
        bool registered = false;

        do
        {
            try
            {
                if (!registered)
                {
                    Registration.AddAll();
                }
                registered = true;
                Logger.Get().Log("RebyonokBot is now fully operational");
                MessageSender.Get().SendMessage(Wakeup[(int)GD.RandRange(0, Wakeup.Length)]);
            }
            catch (Exception Exception)
            {
                if (Exception is VkNet.Exception.CaptchaNeededException || Exception is VkNet.Exception.RateLimitReachedException)
                {
                    int SleepingTime = (int)GD.RandRange(5000.0, 15000.0);
                    GD.Print("Got rated, sleeping for " + SleepingTime + "ms and retrying");
                    gotCapcha = true;
                    SysThread.Sleep(SleepingTime);
                    return;
                }
                else
                {
                    ExceptionCounter++;
                    if (ExceptionCounter > MaxExceptionCount)
                    {
                        GetTree().Quit(1);
                    }
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
                INodeEvents Target = SystemsList[i];
                if (!Target.OnProcess(Delta))
                {
                    GD.PushError(Target.GetType().Name + " failed to execute OnDelta");
                }
            }
        }
        catch (Exception Exception)
        {
            if (Exception is VkNet.Exception.CaptchaNeededException || Exception is VkNet.Exception.RateLimitReachedException)
            {
                GD.Print("Got rated, restarting...");
                GetTree().Quit(1);
                return;
            }
            else
            {
                ExceptionCounter++;
                if (ExceptionCounter > MaxExceptionCount)
                {
                    GetTree().Quit(1);
                }
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
                INodeEvents Target = SystemsList[i];
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
            if (ExceptionCounter < MaxExceptionCount)
            {
                OnException.Invoke(CoreExceptionType.OnShutdown, Exception);
            }
        }
        if (OS.ExitCode != 0)
        {
            return;
        }
        MessageSender.Get().SendMessage(Shutdown[(int)GD.RandRange(0, Shutdown.Length)]);
    }

    public void Register<T>() where T : Node, INodeEvents, new()
    {
        T NewSystem = new T();
        SystemsDict[NewSystem.GetType()] = NewSystem;
        SystemsList.Add(NewSystem);
        NewSystem.Name = typeof(T).Name;
        AddChild(NewSystem);
        if (!NewSystem.OnReady())
        {
            GD.PushError(typeof(T).Name + " failed to execute OnReady");
        }
    }

    public INodeEvents Get(Type Target)
    {
        if (!SystemsDict.ContainsKey(Target))
        {
            GD.PushError("Core failed to find node by type of " + Target.Name);
            return null;
        }
        return SystemsDict[Target];
    }
}

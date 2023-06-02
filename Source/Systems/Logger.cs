using Godot;
using System;
using System.Diagnostics;

public class Logger : INode<Logger>
{
    public Event<string> OnError;

    private DateTime Launch;

    private string GetLoggerTime()
    {
        return (DateTime.Now - Launch).ToString("hh':'mm':'ss");
    }

    public void Log(string Text)
    {
        GD.Print("[" + GetLoggerTime() + "] " + Text);
    }

    public void Warning(string Text)
    {
        GD.PushWarning("[" + GetLoggerTime() + "] " + Text);
    }

    public void Error(string Text, bool Blocking = false, bool Quit = false)
    {
        GD.PushError("[" + GetLoggerTime() + "] " + Text);
        OnError.Invoke(Text);
        if (Blocking)
        {
            using (Process cmd = new Process())
            {
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Arguments = "/k msg %username% /w " + Text + " & exit";
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = true;
                cmd.Start();
                cmd.WaitForExit();
            }
        }
        if (Quit)
        {
            GetTree().Quit(1);
        }
    }

    public bool OnCoreException(CoreExceptionType Type, Exception Exception)
    {
        string Result;
        switch (Type)
        {
            case CoreExceptionType.OnReady:
                {
                    Result = "OnReady got an exception:\n";
                    break;
                }
            default:
            case CoreExceptionType.OnProcess:
                {
                    Result = "OnProcess got an exception:\n";
                    break;
                }
            case CoreExceptionType.OnShutdown:
                {
                    Result = "OnShutdown got an exception:\n";
                    break;
                }
        }
        Result += Exception.Message + "\n" + Exception.StackTrace;
        Error(Result);
        return true;
    }

    public override bool OnReady()
    {
        OnError = new Event<string>();
        Launch = DateTime.Now;
        
        Core.Instance.OnException.Register(OnCoreException);

        Log("Starter RebyonokBot successfully");
        return true;
    }

    public override bool OnProcess(float Delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        Log("Closed RebyonokBot successfully");
        return true;
    }
}

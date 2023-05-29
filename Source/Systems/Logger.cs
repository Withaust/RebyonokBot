using Godot;
using System;
using System.Diagnostics;

public delegate void OnError(string Text);

public class Logger : ISystem<Logger>
{
    public event OnError OnError;

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
        OnError?.Invoke(Text);
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

    public override bool OnReady()
    {
        Launch = DateTime.Now;

        Log("Starter RebyonokBot successfully");
        return true;
    }

    public override bool OnShutdown()
    {
        Log("Closed RebyonokBot successfully");
        return true;
    }
}

using Godot;
using System;
using System.Diagnostics;

public delegate void OnError(string Text);

public class Logger : ISystem<Logger>
{
	public event OnError OnError;

	public void Log(string Text)
	{
		GD.Print("[" + Time.GetTimeStringFromSystem() + "] " + Text);
	}

	public void Warning(string Text)
	{
		GD.PushWarning("[" + Time.GetTimeStringFromSystem() + "] " + Text);
	}

	public void Error(string Text, bool Blocking = false, bool Quit = false)
	{
		GD.PushError("[" + Time.GetTimeStringFromSystem() + "] " + Text);
		OnError?.Invoke(Text);
		if (Blocking)
		{
			Process cmd = new Process();
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.Arguments = "/k msg %username% /w " + Text + " & exit";
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = true;
			cmd.Start();
			cmd.WaitForExit();
		}
		if(Quit)
		{
			GetTree().Quit(1);
		}
	}

	public override void OnReady()
	{
		Log("Starter RebyonokBot successfully");
	}

	public override void OnShutdown()
	{
		Log("Closed RebyonokBot successfully");
	}
}

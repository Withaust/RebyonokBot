using Godot;
using System.Diagnostics;

public class Version : INode<Version>
{
    public string Commit { get; private set; }

    string RunEvaluate()
    {
        string Result;
        using (Process cmd = new Process())
        {
            cmd.StartInfo.FileName = "git.exe";
            cmd.StartInfo.Arguments = "show --name-status";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();
            Result = cmd.StandardOutput.ReadToEndAsync().Result;
            cmd.WaitForExit();
        }
        return Result;
    }

    public override bool OnReady()
    {
        string VersionText = RunEvaluate();
        Commit = VersionText.Split("\n")[0].Split(" ")[1];
        return true;
    }

    public override bool OnProcess(float Delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        return true;
    }
}

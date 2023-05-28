using Godot;
using System.Diagnostics;

public class Version : ISystem<Version>
{
    public string VersionText { get; private set; }
    public string Commit { get; private set; }

    string RunEvaluate()
    {
        string Result;
        using (Process cmd = new Process())
        {
            cmd.StartInfo.FileName = "git.exe";
            cmd.StartInfo.Arguments = "show --summary";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();
            Result = cmd.StandardOutput.ReadToEndAsync().Result;
            cmd.WaitForExit();
        }
        return Result;
    }

    public override void OnReady()
    {
        VersionText = RunEvaluate();
        Commit = VersionText.Split(" ")[1];
    }

    public override void OnShutdown()
    {

    }
}

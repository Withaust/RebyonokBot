
class Help : INode<Eblan>
{
    [Cmd("help")]
    [CmdArgs(null)]
    [CmdShort("команды", "cmd", "h")]
    [CmdHelp("Список команд")]
    public string Execute()
    {
        string Result = default;
        foreach(var Entry in Commander.Get().Descriptions)
        {
            Result += "." + Entry.Key + ": " + Entry.Value + "\n";
        }
        return Result;
    }

    public override bool OnReady()
    {
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

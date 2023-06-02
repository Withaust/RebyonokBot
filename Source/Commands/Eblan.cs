
class Eblan : INode<Eblan>
{
    [Cmd("eblan")]
    [CmdArgs(null)]
    // vvv Different languages vvv
    [CmdShort("e", "е")]
    [CmdHelp("Генератор ебланов")]
    public string Execute()
    {
        return "Еблан .. TODO";
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

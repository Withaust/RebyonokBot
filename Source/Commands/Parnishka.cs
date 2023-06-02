
class Parnishka : INode<Eblan>
{
    [Cmd("parniska")]
    [CmdArgs(null)]
    [CmdShort("p", "парнишка", "ребёнок")]
    [CmdHelp("Генератор парнишек")]
    public SentMessage Execute()
    {
        return new SentMessage() { Text = "Лови", Attachment = Photos.Get().GroupChat.Upload("icon.png") };
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

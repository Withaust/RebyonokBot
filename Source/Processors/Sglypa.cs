
public class Sglypa : INode<Commander>
{
    private long SglypaId;

    public override bool OnReady()
    {
        MessageReciever.Get().OnMessage.Register(OnMessage);
        return true;
    }

    public override bool OnProcess(float delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        return true;
    }

    public bool OnMessage(RecievedMessage Message)
    {
        if (Message.FromId != Credentials.Get().Fields.SglypaId)
        {
            return true;
        }

        if (Message.Text.Contains("."))
        {
            MessageSender.Get().SendMessage("Сглыпа иди нахуй");
        }
        else if (Message.Text.Contains("@"))
        {
            MessageSender.Get().SendMessage("Как ты заебал маму свою пингани сглыпа ебаная");
        }
        else if (Message.Text.ToLower().Contains("dg"))
        {
            MessageSender.Get().SendMessage("Чтоб ты сдох");
        }

        return false;
    }
}

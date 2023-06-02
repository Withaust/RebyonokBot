using Godot;
using System;
using System.Collections.ObjectModel;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

public class RecievedMessage
{
    public long FromId;
    public string Text;
    public ReadOnlyCollection<Attachment> Attachments;
}

public class MessageReciever : INode<MessageReciever>
{
    private VkApi Api;
    private LongPollServerResponse PollServer;

    public Event<RecievedMessage> OnMessage { get; private set; }

    public override bool OnReady()
    {
        Api = Vk.Get().Api;
        PollServer = Api.Messages.GetLongPollServer(true);
        OnMessage = new Event<RecievedMessage>();

        return true;
    }

    public override bool OnProcess(float Delta)
    {
        LongPollHistoryResponse Poll = Api.Messages.GetLongPollHistory(
        new MessagesGetLongPollHistoryParams()
        {
            Ts = Convert.ToUInt64(PollServer.Ts),
            Pts = PollServer.Pts,
            PreviewLength = 1000
        });

        PollServer.Pts = Poll?.NewPts;

        if (Poll?.Messages == null || Poll?.Messages.Count == 0)
        {
            return true;
        }

        foreach (var Message in Poll?.Messages)
        {
            long MessageFromId = Message.FromId.GetValueOrDefault();
            // Prevent from dispatching our own messages into the infinite loop
            if (MessageFromId == Credentials.Get().Fields.UserId)
            {
                continue;
            }
            OnMessage.Invoke(new RecievedMessage
            {
                FromId = MessageFromId,
                Text = Message.Text,
                Attachments = Message.Attachments
            });
            Logger.Get().Log(MessageFromId + ": " + Message.Text);
        }

        return true;
    }

    public override bool OnShutdown()
    {
        return true;
    }
}

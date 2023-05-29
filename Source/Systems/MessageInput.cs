using Godot;
using System;
using System.Collections.ObjectModel;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

public class MessageInput : ISystem<MessageInput>
{
    private VkApi Api;
    private LongPollServerResponse PollServer;

    public Event<long, string, ReadOnlyCollection<Attachment>> OnMessage;

    public override bool OnReady()
    {
        Api = Vk.Get().Api;
        PollServer = Api.Messages.GetLongPollServer(true);
        OnMessage = new Event<long, string, ReadOnlyCollection<Attachment>>();

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

        foreach(var Message in Poll?.Messages)
        {
            // Prevent from dispatching our own messages into the infinite loop
            if(Message.FromId.GetValueOrDefault() == Convert.ToInt64((string)Credentials.Get().Fields["user_id"]))
            {
                continue;
            }
            OnMessage.Invoke(Message.FromId.GetValueOrDefault(), Message.Text, Message.Attachments);
            Vk.Get().SendMessage(Message.FromId.ToString() + ": " + Message.Text);
            Logger.Get().Log(Message.Text);
        }

        return true;
    }

    public override bool OnShutdown()
    {
        return true;
    }
}

using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

public class SentMessage
{
    public string Text = null;
    public MediaAttachment Attachment = null;
    public ReadOnlyCollection<MediaAttachment> Attachments = null;
}

public class MessageSender : INode<MessageSender>
{
    private VkApi Api;
    private MessagesSendParams sendParams;

    public override bool OnReady()
    {
        Api = Vk.Get().Api;
        Logger.Get().OnError.Register(OnError);
        Commander.Get().OnParseFail.Register(OnCommandError);
        //TODO: Move into a separete util Random.cs class
        GD.Randomize();
        CheckVersion();
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

    public bool OnError(string Text)
    {
        SendMessage("Я обосрался!\n" + Text);
        return true;
    }

    public bool OnCommandError(string Text)
    {
        SendMessage(Text);
        return true;
    }

    public void CheckVersion()
    {
        string Message = GetLastSelfMessage();

        if (Message == null || Message != Version.Get().Commit)
        {
            SendMessageSelf(Version.Get().Commit);
            SendMessage("Я обновился!\n" +
            "https://github.com/Withaust/RebyonokBot/commit/" + Version.Get().Commit);
        }
    }

    private string ParseMessage(string Message)
    {
        if (Message == null || string.IsNullOrEmpty(Message) || string.IsNullOrWhiteSpace(Message))
        {
            return "�";
        }
        else if (Message.Length > 2048)
        {
            return Message.Substring(0, 2048);
        }
        else
        {
            return Message;
        }
    }

    public string GetLastSelfMessage()
    {
        var getHistory = Api.Messages.GetHistory(new MessagesGetHistoryParams
        {
            UserId = Credentials.Get().Fields.UserId,
            Count = 1,
        });
        if (getHistory.Messages.Count() != 1)
        {
            return null;
        }
        return getHistory.Messages.First().Text;
    }

    public void SendMessage(string Text)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = Credentials.Get().Fields.ChatId,
            Message = ParseMessage(Text),
            RandomId = GD.Randi()
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessageSelf(string Text)
    {
        sendParams = new MessagesSendParams()
        {
            UserId = Credentials.Get().Fields.UserId,
            Message = ParseMessage(Text),
            RandomId = GD.Randi()
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(MediaAttachment Attachment)
    {
        sendParams = new MessagesSendParams()
        {
            UserId = Credentials.Get().Fields.ChatId,
            RandomId = GD.Randi(),
            Attachments = new MediaAttachment[] { Attachment }
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(SentMessage Message)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = Credentials.Get().Fields.ChatId,
            RandomId = GD.Randi(),
        };

        if (Message.Text != null)
        {
            sendParams.Message = ParseMessage(Message.Text);
        }
        if (Message.Attachment != null)
        {
            sendParams.Attachments = new MediaAttachment[] { Message.Attachment };
        }
        if (Message.Attachments != null)
        {
            sendParams.Attachments = Message.Attachments;
        }
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(string Text, MediaAttachment Attachment)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = Credentials.Get().Fields.ChatId,
            Message = ParseMessage(Text),
            RandomId = GD.Randi(),
            Attachments = new MediaAttachment[] { Attachment }
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(string Text, IEnumerable<MediaAttachment> Attachments = null)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = Credentials.Get().Fields.ChatId,
            Message = ParseMessage(Text),
            RandomId = GD.Randi(),
            Attachments = Attachments,
        };
        Api.Messages.Send(sendParams);
    }
}

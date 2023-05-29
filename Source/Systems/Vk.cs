using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

public class Vk : ISystem<Vk>
{
    public bool MyRemoteCertificateValidationCallback(System.Object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public VkApi Api { get; private set; }
    private MessagesSendParams sendParams;
    private bool Authorized = false;

    public override bool OnReady()
    {
        Logger.Get().OnError.Register(OnError);
        GD.Randomize();
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        Api = new VkApi();

        Api.Authorize(new ApiAuthParams
        {
            AccessToken = (string)Credentials.Get().Fields["token"]
        });
        Authorized = true;

        CheckVersion();
        return true;
    }

    public override bool OnProcess(float Delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        //SendMessage("Delicious\nBottom text");
        return true;
    }

    public bool OnError(string Text)
    {
        SendMessage("Я обосрался!\n" + Text);
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
        else if (Message.Length > 1000)
        {
            return Message.Substring(0, 1000);
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
            UserId = Convert.ToInt64((string)Credentials.Get().Fields["user_id"]),
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
            ChatId = Convert.ToInt64((string)Credentials.Get().Fields["chat_id"]),
            Message = ParseMessage(Text),
            RandomId = GD.Randi()
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessageSelf(string Text)
    {
        sendParams = new MessagesSendParams()
        {
            UserId = Convert.ToInt64((string)Credentials.Get().Fields["user_id"]),
            Message = ParseMessage(Text),
            RandomId = GD.Randi()
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(MediaAttachment Attachment)
    {
        sendParams = new MessagesSendParams()
        {
            UserId = Convert.ToInt64((string)Credentials.Get().Fields["user_id"]),
            RandomId = GD.Randi(),
            Attachments = new MediaAttachment[] { Attachment }
        };
        Api.Messages.Send(sendParams);
    }

    public void SendMessage(string Text, MediaAttachment Attachment)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = Convert.ToInt64((string)Credentials.Get().Fields["chat_id"]),
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
            ChatId = Convert.ToInt64((string)Credentials.Get().Fields["chat_id"]),
            Message = ParseMessage(Text),
            RandomId = GD.Randi(),
            Attachments = Attachments,
        };
        Api.Messages.Send(sendParams);
    }
}

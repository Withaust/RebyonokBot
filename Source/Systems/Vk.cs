using Godot;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

public class Vk : ISystem<Vk>
{
    public bool MyRemoteCertificateValidationCallback(System.Object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private VkApi api;
    private MessagesSendParams sendParams;
    private bool Authorized = false;

    public override bool OnReady()
    {
        GD.Randomize();
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        api = new VkApi();

        api.Authorize(new ApiAuthParams
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

    public void CheckVersion()
    {
        var getHistory = api.Messages.GetHistory(new MessagesGetHistoryParams
        {
            UserId = Convert.ToInt64((string)Credentials.Get().Fields["user_id"]),
            Count = 1,
        });

        if (getHistory.Messages.Count() == 0 || getHistory.Messages.First().Text != Version.Get().Commit)
        {
            SendMessageSelf(Version.Get().Commit);
            SendMessage("Я обновился!\n" + 
            "https://github.com/Withaust/RebyonokBot/commit/" + Version.Get().Commit);
        }
    }

    public void SendMessage(string Text)
    {
        sendParams = new MessagesSendParams()
        {
            ChatId = 1,
            Message = Text,
            RandomId = GD.Randi()
        };
        api.Messages.Send(sendParams);
    }

    public void SendMessageSelf(string Text)
    {
        sendParams = new MessagesSendParams()
        {
            UserId = Convert.ToInt64((string)Credentials.Get().Fields["user_id"]),
            Message = Text,
            RandomId = GD.Randi()
        };
        api.Messages.Send(sendParams);
    }
}

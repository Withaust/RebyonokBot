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

public class Vk : INode<Vk>
{
    public bool MyRemoteCertificateValidationCallback(System.Object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public VkApi Api { get; private set; }
    public bool Authorized { get; private set; } = false;

    public override bool OnReady()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        Api = new VkApi();

        Api.Authorize(new ApiAuthParams
        {
            AccessToken = Credentials.Get().Fields.Token
        });
        Authorized = true;

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

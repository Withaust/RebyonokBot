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

	public override void OnReady()
	{
		GD.Randomize();
		ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

		api = new VkApi();

		api.Authorize(new ApiAuthParams
		{
			AccessToken = (string)Credentials.Get().Fields["token"]
		});
		//Console.WriteLine(api.Token);

		sendParams = new MessagesSendParams()
		{
			ChatId = 1,
			Message = "Delicious\nBottom text",
			RandomId = GD.Randi()
		};

		var group = api.Groups.GetById(null, (string)Credentials.Get().Fields["group_id"], GroupsFields.All).FirstOrDefault();
		if (group == null)
		{
			return;
		}

		//sendParams.Message = group.Name;
	}

	public override void OnShutdown()
	{
		api.Messages.Send(sendParams);
	}
}

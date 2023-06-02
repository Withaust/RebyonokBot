using System;
using System.Collections.Generic;
using VkNet;
using VkNet.Model.RequestParams;

public class Photos : INode<Photos>
{
    private VkApi Api;

    public AlbumManager GroupChat { get; private set; }

    public override bool OnReady()
    {
        Api = Vk.Get().Api;
        var Albums = Api.Photo.GetAlbums(new PhotoGetAlbumsParams()
        {
            //Why, VK, why?
            OwnerId = -Credentials.Get().Fields.GroupId
        });

        GroupChat = new AlbumManager("GroupChat", Api, Albums);

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
}

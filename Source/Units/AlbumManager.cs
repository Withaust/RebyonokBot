using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using VkNet;
using VkNet.Model.Attachments;

public class AlbumManager
{
    private int MaxPhotoCount = 10000;
    private long IndexCounter = 0;
    private long PhotoCounter = 0;
    public long SelectedId { get; private set; } = 0;
    private string Prefix;
    private VkApi Api;

    public AlbumManager(string NewPrefix, VkApi NewApi, VkNet.Utils.VkCollection<VkNet.Model.PhotoAlbum> NewCurrentAlbums)
    {
        Prefix = NewPrefix;
        Api = NewApi;

        // Add all albums in initial pass
        foreach (var Album in NewCurrentAlbums)
        {
            string Title = Album.Title;
            if (!Title.StartsWith(Prefix))
            {
                continue;
            }

            long NewIndexCounter = Convert.ToInt64(Title.Split('_').Last());

            if (NewIndexCounter >= IndexCounter)
            {
                IndexCounter = NewIndexCounter;
                PhotoCounter = Album.Size.GetValueOrDefault();
                SelectedId = Album.Id;
            }

        }

        // If we managed to get here, we either found somewhat suitable target album,
        // or we dont have albums at all, indicating by SelectedId still being 0
        if (SelectedId == 0)
        {
            var Album = Api.Photo.CreateAlbum(new VkNet.Model.RequestParams.PhotoCreateAlbumParams()
            {
                Title = Prefix + "_0",
                GroupId = Credentials.Get().Fields.GroupId
            });

            IndexCounter = 0;
            PhotoCounter = 0;
            SelectedId = Album.Id;
            return;
        }

        // Sanity check if our selected highest indexing album has reached its limit
        if (PhotoCounter <= MaxPhotoCount)
        {
            // We are fine with our current selection
            return;
        }
        else
        {
            // Our selected album has reached its limit, have to create a new one
            CreateNew();
        }
    }

    private void CreateNew()
    {
        IndexCounter++;
        var Album = Api.Photo.CreateAlbum(new VkNet.Model.RequestParams.PhotoCreateAlbumParams()
        {
            Title = Prefix + "_" + IndexCounter,
            GroupId = Credentials.Get().Fields.GroupId
        });

        PhotoCounter = 0;
        SelectedId = Album.Id;
    }

    public Photo Upload(string FileName)
    {
        PhotoCounter++;
        // Our selected album has reached its limit, have to create a new one
        if (PhotoCounter >= MaxPhotoCount)
        {
            CreateNew();
        }

        Photo Result;
        var Server = Api.Photo.GetUploadServer(SelectedId, Credentials.Get().Fields.GroupId);
        using (var wc = new WebClient())
        {
            string SendResult = Encoding.ASCII.GetString(wc.UploadFile(Server.UploadUrl, FileName));
            Result = Api.Photo.Save(new VkNet.Model.RequestParams.PhotoSaveParams()
            {
                AlbumId = SelectedId,
                GroupId = Credentials.Get().Fields.GroupId,
                SaveFileResponse = SendResult
            }).First();
        }
        return Result;
    }
}
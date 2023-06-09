using Godot;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class RimNotif : INode<TestImage>
{
    [Cmd("rimnotif")]
    [CmdArgs(typeof(string))]
    [CmdShort("rn")]
    [CmdHelp("Генератор уведомлений из Рима")]
    public async Task<SentMessage> Execute(string Text)
    {
        Scenes Scenes = Scenes.Get();
        Scenes.LoadScene("RimNotif");
        RichTextLabel Label = Scenes.CurrentScene.GetNode<RichTextLabel>("Text");
        Label.BbcodeText = Text;
        Label.FilterEmotes();
        Image Image = await Scenes.Render();
        Image.SavePng("test.png");
        return new SentMessage{ Text = "Лови", Attachment = Photos.Get().GroupChat.Upload("test.png")};
    }

    public override bool OnReady()
    {
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

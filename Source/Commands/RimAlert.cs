using Godot;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class RimAlert : INode<TestImage>
{
    [Cmd("rimalert")]
    [CmdArgs(typeof(string))]
    [CmdShort("ra")]
    [CmdHelp("Генератор алертов из Рима")]
    public async Task<SentMessage> Execute(string Text)
    {
        if (Text.Length > 24)
        {
            return new SentMessage { Text = "Слишком жирный еблан" };
        }
        Scenes Scenes = Scenes.Get();
        Scenes.LoadScene("RimAlert");
        RichTextLabel Label = Scenes.CurrentScene.GetNode<RichTextLabel>("Text");
        Label.BbcodeText = Text;
        Label.FilterEmotes();
        Label.BbcodeText = "[right]" + Label.BbcodeText + "[/right]";
        int SpaceCount = 11 - (Text.Length / 4);
        Label.BbcodeText = Label.BbcodeText.AppendEnd(" ", SpaceCount);
        Image Image = await Scenes.Render();
        Image.SavePng("test.png");
        return new SentMessage { Text = "Лови", Attachment = Photos.Get().GroupChat.Upload("test.png") };
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

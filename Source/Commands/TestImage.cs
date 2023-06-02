using Godot;
using System.Threading.Tasks;

public class TestImage : INode<TestImage>
{
    [Cmd("test")]
    [CmdArgs(null)]
    [CmdShort("t")]
    [CmdHelp("Генератор тестов")]
    public async Task<SentMessage> Execute()
    {
        Scenes Scenes = Scenes.Get();
        Scenes.LoadScene("TestImage");
        Label TopLabel = Scenes.CurrentScene.GetNode<Label>("TextureRect/VBoxContainer/Label");
        TopLabel.Text = GD.Randf().ToString();
        Label BottomLabel = Scenes.CurrentScene.GetNode<Label>("TextureRect/VBoxContainer/Label2");
        BottomLabel.Text = GD.Randf().ToString();
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

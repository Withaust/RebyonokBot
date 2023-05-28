using Godot;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Closer : ISystem<Logger>
{
    public int Port = 49898;

    private TcpListener Listener;
    private Task ListenTask;
    private CancellationTokenSource Token;

    private async Task Listen()
    {
        var client = await Listener.AcceptTcpClientAsync();
        client.Close();
        Logger.Get().Log("Recieved graceful request to close!");
        GetTree().Quit(0);
    }

    public override void OnReady()
    {
        Token = new CancellationTokenSource();
        Listener = new TcpListener(IPAddress.Any, Port);
        Listener.Start();
        Logger.Get().Log("Listening for graceful TCP shutdown at port " + Port);
        ListenTask = Task.Run((Func<Task>)Listen, Token.Token);
    }

    public override void OnShutdown()
    {
        Token.Cancel();
    }
}

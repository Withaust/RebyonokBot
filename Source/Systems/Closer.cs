using Godot;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Closer : INode<Closer>
{
    public static int Port = 49898;

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

    public override bool OnReady()
    {
        try
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();
        }
        catch (Exception e)
        {
            Logger.Get().Error("Failed to create Closer listener. Is another RebyonokBot instance running?. " +
            e.Message + "\n" + e.StackTrace);
            return false;
        }
        Logger.Get().Log("Listening for graceful TCP shutdown at port " + Port);
        Token = new CancellationTokenSource();
        ListenTask = Task.Run((Func<Task>)Listen, Token.Token);
        return true;
    }

    public override bool OnProcess(float Delta)
    {
        return true;
    }

    public override bool OnShutdown()
    {
        Token.Cancel();
        return true;
    }
}

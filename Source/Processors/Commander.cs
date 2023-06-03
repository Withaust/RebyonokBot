using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkNet.Model;

public class Cmd : Attribute
{
    public string Name;

    public Cmd(string NewName)
    {
        Name = NewName;
    }
}

public class CmdArgs : Attribute
{
    public Type[] ArgTypes;

    public CmdArgs(params Type[] Types)
    {
        ArgTypes = Types;
    }
}

public class CmdHelp : Attribute
{
    public string Help;

    public CmdHelp(string NewHelp)
    {
        Help = NewHelp;
    }
}

public class CmdShort : Attribute
{
    public string[] Shortcuts;

    public CmdShort(params string[] NewShortcuts)
    {
        Shortcuts = NewShortcuts;
    }
}

public class Commander : INode<Commander>
{
    private struct CommmandEntry
    {
        public Type Invokator;
        public MethodInfo Base;
    }

    private Regex CommandRegex;
    private Dictionary<string, Tuple<CommmandEntry, List<Type>>> Commands;
    private Dictionary<string, string> Shortcuts;
    public Dictionary<string, string> Descriptions { get; private set; }
    public Event<string> OnParseFail { get; private set; }

    public override bool OnReady()
    {
        CommandRegex = new Regex("^\\.(.*?) (.*?)$", RegexOptions.Singleline);
        Commands = new Dictionary<string, Tuple<CommmandEntry, List<Type>>>();
        Shortcuts = new Dictionary<string, string>();
        Descriptions = new Dictionary<string, string>();
        OnParseFail = new Event<string>();

        Load();

        MessageReciever.Get().OnMessage.Register(OnMessage);

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

    private void Load()
    {
        Reflector Reflector = Reflector.Get();

        foreach (var Method in Reflector.GetAttributedMethods(typeof(Cmd)))
        {
            if (Method.Item2.ReturnType != typeof(SentMessage) && Method.Item2.ReturnType != typeof(Task<SentMessage>)) { continue; }
            Cmd Cmd = (Cmd)Method.Item2.GetCustomAttribute(typeof(Cmd)); if (Cmd == null) { continue; }
            CmdArgs Args = (CmdArgs)Method.Item2.GetCustomAttribute(typeof(CmdArgs)); if (Args == null) { continue; }
            CmdShort Short = (CmdShort)Method.Item2.GetCustomAttribute(typeof(CmdShort)); if (Short == null) { continue; }
            CmdHelp Help = (CmdHelp)Method.Item2.GetCustomAttribute(typeof(CmdHelp)); if (Help == null) { continue; }

            if (Commands.ContainsKey(Cmd.Name)) { continue; }

            List<Type> NewArgs;

            if (Args.ArgTypes == null)
            {
                NewArgs = null;
            }
            else
            {
                NewArgs = Args.ArgTypes.ToList();
            }

            Commands[Cmd.Name.ToLower()] = new Tuple<CommmandEntry, List<Type>>(new CommmandEntry()
            { Base = Method.Item2, Invokator = Method.Item1 }, NewArgs);
            Descriptions[Cmd.Name.ToLower()] = Help.Help;

            foreach (var Shortcut in Short.Shortcuts)
            {
                if (!Shortcuts.ContainsKey(Shortcut))
                {
                    Shortcuts[Shortcut.ToLower()] = Cmd.Name.ToLower();
                }
            }
        }
    }

    private List<Tuple<string, bool>> ParseText(string Text)
    {
        Text += ' ';
        List<Tuple<string, bool>> Result = new List<Tuple<string, bool>>();
        string TempVariable = null;
        bool String = false;

        for (int i = 0; i < Text.Length; ++i)
        {
            if (Text[i] == ' ')
            {
                if (String)
                {
                    TempVariable += ' ';
                }
                else
                {
                    if (TempVariable != null)
                    {
                        Result.Add(new Tuple<string, bool>(TempVariable, false));
                        TempVariable = null;
                    }
                }
            }
            else if (Text[i] == '"')
            {
                if (!String)
                {
                    String = true;
                }
                else
                {
                    String = false;

                    if (TempVariable != null)
                    {
                        Result.Add(new Tuple<string, bool>(TempVariable, true));
                        TempVariable = null;
                    }
                }
            }
            else
            {
                TempVariable += Text[i];
            }
        }

        return Result;
    }

    private void PrintError(Tuple<CommmandEntry, List<Type>> Entry, string Command)
    {
        string Args = default;

        for (int i = 0; i < Entry.Item2.Count; ++i)
        {
            Args += Entry.Item2[i].Name;

            if (i != Entry.Item2.Count - 1) { Args += ", "; }
        }

        OnParseFail.Invoke("Неверные типы аргументов для команды " + Command + "\nСписок аргументов: " + Args);
    }

    public bool OnMessage(RecievedMessage Message)
    {
        if (Message.FromId < 0) { return true; }
        if (!Message.Text.StartsWith(".") || Message.Text.Length < 2) { return true; }

        string Command;
        List<Tuple<string, bool>> Arguments = null;
        if (Message.Text.Contains(' '))
        {
            Match NewMatch = CommandRegex.Match(Message.Text);
            Command = NewMatch.Groups[1].Value;
            Arguments = ParseText(NewMatch.Groups[2].Value);
        }
        else
        {
            Command = Message.Text.Substring(1);
        }

        Command = Command.ToLower();

        if (Shortcuts.ContainsKey(Command)) { Command = Shortcuts[Command]; }
        if (!Commands.ContainsKey(Command)) { return true; }

        Tuple<CommmandEntry, List<Type>> Entry = Commands[Command];

        List<object> ParsedObjects = new List<object>();

        if (Entry.Item2 == null)
        {
            object Result = Entry.Item1.Base.Invoke(Core.Instance.Get(Entry.Item1.Invokator), null);
            if (Result != null)
            {
                if (Result is Task<SentMessage> Async)
                {
                    Async.ContinueWith((i) => { MessageSender.Get().SendMessage(i.Result); });
                }
                else if (Result is SentMessage Blocking)
                {
                    MessageSender.Get().SendMessage(Blocking);
                }
            }
            return false;
        }

        if (Arguments == null || Arguments.Count != Entry.Item2.Count)
        {
            PrintError(Entry, Command);
            return false;
        }

        for (int i = 0; i < Arguments.Count; ++i)
        {
            if (Entry.Item2[i] == typeof(string))
            {
                if (Arguments[i].Item2)
                {
                    ParsedObjects.Add(Arguments[i].Item1);
                }
            }
            else
            {
                try
                {
                    object ResultObject = JsonConvert.DeserializeObject(Arguments[i].Item1, Entry.Item2[i]);
                    ParsedObjects.Add(ResultObject);
                }
                catch (Exception Exception)
                {
                    int Compiler = Exception.Message.Length;
                    Compiler++;
                    PrintError(Entry, Command);
                    return false;
                }
            }
        }

        if (Entry.Item2.Count != ParsedObjects.Count)
        {
            PrintError(Entry, Command);
            return false;
        }

        bool Valid = true;

        for (int i = 0; i < ParsedObjects.Count; ++i)
        {
            if (Entry.Item2[i] != ParsedObjects[i].GetType())
            {
                Valid = false;
            }
        }

        if (Valid)
        {
            object Result = Entry.Item1.Base.Invoke(Core.Instance.Get(Entry.Item1.Invokator), ParsedObjects.ToArray());
            if (Result != null)
            {
                if (Result is Task<SentMessage> Async)
                {
                    Async.ContinueWith((i) => { MessageSender.Get().SendMessage(i.Result); });
                }
                else if (Result is SentMessage Blocking)
                {
                    MessageSender.Get().SendMessage(Blocking);
                }
            }
        }
        else
        {
            PrintError(Entry, Command);
        }

        return true;
    }
}

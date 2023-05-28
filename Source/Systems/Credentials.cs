using Godot;
using System;
using System.IO;
using SysFile = System.IO.File;

public class Credentials : ISystem<Credentials>
{
	public string CredentialsFile = "CREDENTIALS.json";
	public Godot.Collections.Dictionary Fields { get; private set; }

	public override void OnReady()
	{
		if (!SysFile.Exists(CredentialsFile))
		{
			Logger.Get().Error(CredentialsFile + " is not found. RebyonokBot cant launch.", true, true);
			return;
		}

		string CredentialsContents = SysFile.ReadAllText(CredentialsFile);
		JSONParseResult ParseResult = JSON.Parse(CredentialsContents);
		if (ParseResult.Error != Error.Ok)
		{
			Logger.Get().Error(CredentialsFile + " failed to get parsed. Error \"" + ParseResult.ErrorString + "\" at line " + ParseResult.ErrorLine, true, true);
			return;
		}

		Fields = (Godot.Collections.Dictionary)ParseResult.Result;
	}

	public override void OnShutdown()
	{

	}
}

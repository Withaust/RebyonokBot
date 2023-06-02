using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using SysFile = System.IO.File;

public class CredentialsFields
{
	public string Token;
	public long GroupId;
	public long UserId;
	public long ChatId;
	public HashSet<long> Admins;
	public int Fps;
	public long SglypaId;
}

public class Credentials : INode<Credentials>
{
	public string CredentialsFile = "CREDENTIALS.json";
	private Godot.Collections.Dictionary GodotFields;
	public CredentialsFields Fields { get; private set; }

	public void Deserialize()
	{
		Fields = new CredentialsFields()
		{
			Token = (string)GodotFields["Token"],
			GroupId = Convert.ToInt64((string)GodotFields["GroupId"]),
			UserId = Convert.ToInt64((string)GodotFields["UserId"]),
			ChatId = Convert.ToInt64((string)GodotFields["ChatId"]),
			Fps = Convert.ToInt32((string)GodotFields["Fps"]),
			SglypaId = Convert.ToInt64((string)GodotFields["SglypaId"]),
		};
		Fields.Admins = new HashSet<long>();

		foreach(var Admin in (Godot.Collections.Array)GodotFields["Admins"])
		{
			Fields.Admins.Add(Convert.ToInt64((string)Admin));
		}
	}

	public override bool OnReady()
	{
		if (!SysFile.Exists(CredentialsFile))
		{
			Logger.Get().Error(CredentialsFile + " is not found. RebyonokBot cant launch.", true, true);
			return false;
		}

		string CredentialsContents = SysFile.ReadAllText(CredentialsFile);
		JSONParseResult ParseResult = JSON.Parse(CredentialsContents);
		if (ParseResult.Error != Error.Ok)
		{
			Logger.Get().Error(CredentialsFile + " failed to get parsed. Error \"" + ParseResult.ErrorString + "\" at line " + ParseResult.ErrorLine, true, true);
			return false;
		}

		GodotFields = (Godot.Collections.Dictionary)ParseResult.Result;
		Deserialize();
		Engine.TargetFps = Fields.Fps;
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

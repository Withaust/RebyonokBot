using Godot;
using System;
using System.Collections.Generic;

public class Core : Node
{
	private TextureRect Rebyonok;
	private float RebyonokCounter;

	public static Dictionary<Type, ISystemEvents> SystemsDict;
	public static List<ISystemEvents> SystemsList;

	public static bool Initialized = false;
	public static bool Deinitialized = false;
	public static void Initialize()
	{
		if (Initialized)
		{
			return;
		}
		Initialized = true;
		SystemsDict = new Dictionary<Type, ISystemEvents>();
		SystemsList = new List<ISystemEvents>();
	}

	public static void Deinitialize()
	{
		if(Deinitialized)
		{
			return;
		}
		Deinitialized = true;

		for(int i = SystemsList.Count - 1; i >= 0; i--)
		{
			ISystemEvents Target = SystemsList[i];
			Target.OnShutdown();
			SystemsList.RemoveAt(i);
			SystemsDict.Remove(Target.GetType());
		}
	}

	public override void _Ready()
	{
		Rebyonok = GetNode<TextureRect>("Rebyonok");
	}

	public override void _Process(float delta)
	{
		RebyonokCounter += delta;
		if(RebyonokCounter < 1.0f)
		{
			return;
		}
		RebyonokCounter = 0.0f;
		Rebyonok.FlipH = !Rebyonok.FlipH;
	}

	public override void _ExitTree()
	{
		Deinitialize();
	}

	public static void Register(ISystemEvents System)
	{
		SystemsDict[System.GetType()] = System;
		SystemsList.Add(System);
	}
}

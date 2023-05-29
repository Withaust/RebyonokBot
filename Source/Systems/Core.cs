using Godot;
using System;
using System.Collections.Generic;

public class Core : Node
{
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
			if(!Target.OnShutdown())
			{
				GD.PushError(Target.GetType().Name + " failed to execute OnShutdown");
			}
			SystemsList.RemoveAt(i);
			SystemsDict.Remove(Target.GetType());
		}
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

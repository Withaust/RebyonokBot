using System;
using System.Collections.Generic;
using System.Reflection;

public class Reflector : ISystem<Reflector>
{
    private Dictionary<string, Assembly> Assemblies;
    private Dictionary<string, Type> NamedTypes;
    private Dictionary<Type, string> ActualTypes;
    private Dictionary<Type, List<Tuple<Type, MethodInfo>>> AttributedMethods;

    public override bool OnReady()
    {
        Assemblies = new Dictionary<string, Assembly>();

        NamedTypes = new Dictionary<string, Type>();
        ActualTypes = new Dictionary<Type, string>();

        AttributedMethods = new Dictionary<Type, List<Tuple<Type, MethodInfo>>>();

        Assembly[] NewAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var Entry in NewAssemblies)
        {
            string CurrentName = Entry.GetName().Name;

            if (CurrentName.StartsWith("System")
            || CurrentName.StartsWith("mscorlib")
            || CurrentName.StartsWith("RCNG_6"))
            {
                Assemblies[CurrentName] = Entry;
                AddTypes(Assemblies[CurrentName]);
            }
        }

        AddType("System.Int64[]", typeof(long[]));

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

    public Assembly GetAssembly(string PluginID)
    {
        if (Assemblies.ContainsKey(PluginID))
        {
            return Assemblies[PluginID];
        }
        else
        {
            Logger.Get().Error("Tried to access not registered assembly " + PluginID);
            return null;
        }
    }

    public string GetType(Type Target)
    {
        if (ActualTypes.ContainsKey(Target))
        {
            return ActualTypes[Target];
        }
        else
        {
            Logger.Get().Error("Tried to access not registered type " + Target.FullName);
            return null;
        }
    }

    public Type GetType(string Target)
    {
        if (NamedTypes.ContainsKey(Target))
        {
            return NamedTypes[Target];
        }
        else
        {
            Logger.Get().Error("Tried to access not registered type " + Target);
            return null;
        }
    }

    public List<Tuple<Type, MethodInfo>> GetAttributedMethods(Type AttributeType)
    {
        if (AttributedMethods.ContainsKey(AttributeType))
        {
            return AttributedMethods[AttributeType];
        }
        else
        {
            return null;
        }
    }

    private void AddType(string Key, Type Type)
    {
        if (!NamedTypes.ContainsKey(Key))
        {
            NamedTypes[Key] = Type;
        }

        if (!ActualTypes.ContainsKey(Type))
        {
            ActualTypes[Type] = Key;
        }

        foreach (var Method in Type.GetMethods())
        {
            foreach (var Attribute in Method.GetCustomAttributes())
            {
                Type AttributeType = Attribute.GetType();

                if (!AttributedMethods.ContainsKey(AttributeType))
                {
                    AttributedMethods[AttributeType] = new List<Tuple<Type, MethodInfo>>();
                }

                AttributedMethods[AttributeType].Add(new Tuple<Type, MethodInfo>(Type, Method));
            }
        }
    }

    private void AddTypes(Assembly TargetAssembly)
    {
        foreach (var Type in TargetAssembly.GetTypes())
        {
            if (Type.Namespace != null && (Type.IsClass || (Type.IsValueType && !Type.IsEnum)))
            {
                string Key = Type.FullName;
                AddType(Key, Type);
            }
        }
    }
}
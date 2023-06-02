using System;
using System.Collections.Generic;
using System.Reflection;

public class Reflector : INode<Reflector>
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

            //if (CurrentName.StartsWith("System")
            //|| CurrentName.StartsWith("mscorlib")
            //|| CurrentName.StartsWith("RebyonokBot"))
            {
                Assemblies[CurrentName] = Entry;
                AddTypes(Assemblies[CurrentName]);
            }
        }

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

    public Assembly GetAssembly(string AssemblyName)
    {
        if (!Assemblies.ContainsKey(AssemblyName))
        {
            Logger.Get().Error("Tried to access not registered assembly " + AssemblyName);
            return null;
        }
        return Assemblies[AssemblyName];
    }

    public string GetType(Type Target)
    {
        if (!ActualTypes.ContainsKey(Target))
        {
            Logger.Get().Error("Tried to access not registered type " + Target.FullName);
            return null;
        }
        return ActualTypes[Target];
    }

    public Type GetType(string Target)
    {
        if (!NamedTypes.ContainsKey(Target))
        {
            Logger.Get().Error("Tried to access not registered type " + Target);
            return null;
        }
        return NamedTypes[Target];
    }

    public List<Tuple<Type, MethodInfo>> GetAttributedMethods(Type AttributeType)
    {
        if (!AttributedMethods.ContainsKey(AttributeType))
        {
            return null;
        }
        return AttributedMethods[AttributeType];
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
            if (Type.IsClass || (Type.IsValueType && !Type.IsEnum))
            {
                AddType(Type.FullName, Type);
            }
        }
    }
}

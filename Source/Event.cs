using System;
using System.Collections.Generic;

public class Event<X1>
{
    private List<Func<X1, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, bool>>();
    }

    public void Register(Func<X1, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

public class Event<X1, X2>
{
    private List<Func<X1, X2, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, X2, bool>>();
    }

    public void Register(Func<X1, X2, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1, X2 Param2)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1, Param2))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

public class Event<X1, X2, X3>
{
    private List<Func<X1, X2, X3, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, X2, X3, bool>>();
    }

    public void Register(Func<X1, X2, X3, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1, X2 Param2, X3 Param3)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1, Param2, Param3))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

public class Event<X1, X2, X3, X4>
{
    private List<Func<X1, X2, X3, X4, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, X2, X3, X4, bool>>();
    }

    public void Register(Func<X1, X2, X3, X4, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1, X2 Param2, X3 Param3, X4 Param4)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1, Param2, Param3, Param4))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

public class Event<X1, X2, X3, X4, X5>
{
    private List<Func<X1, X2, X3, X4, X5, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, X2, X3, X4, X5, bool>>();
    }

    public void Register(Func<X1, X2, X3, X4, X5, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1, X2 Param2, X3 Param3, X4 Param4, X5 Param5)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1, Param2, Param3, Param4, Param5))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

public class Event<X1, X2, X3, X4, X5, X6>
{
    private List<Func<X1, X2, X3, X4, X5, X6, bool>> Subscriptions;

    public Event()
    {
        Subscriptions = new List<Func<X1, X2, X3, X4, X5, X6, bool>>();
    }

    public void Register(Func<X1, X2, X3, X4, X5, X6, bool> Function)
    {
        Subscriptions.Add(Function);
    }

    public void Invoke(X1 Param1, X2 Param2, X3 Param3, X4 Param4, X5 Param5, X6 Param6)
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            if (!Subscriptions[i].Invoke(Param1, Param2, Param3, Param4, Param5, Param6))
            {
                break;
            }
        }
    }

    ~Event()
    {
        for (int i = 0; i < Subscriptions.Count; i++)
        {
            Subscriptions.RemoveAt(i);
        }
    }
}

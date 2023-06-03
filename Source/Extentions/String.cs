using System;

public static class StringExtention
{
    public static string AppendEnd(this string Target, string Append, int Count = 1)
    {
        for(int i = 0; i < Count; i++)
        {
            Target += Append;
        }
        return Target;
    }
}
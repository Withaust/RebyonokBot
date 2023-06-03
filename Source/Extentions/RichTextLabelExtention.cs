
using System.Text.RegularExpressions;
using Godot;

public static class RichTextLabelExtention
{
    public static void FilterEmotes(this RichTextLabel Target)
    {
        foreach (Match match in Regex.Matches(Target.BbcodeText, "\\\\em\\((.*?)\\)"))
        {
            string Emote = match.Groups[1].Value;
            if (!ResourceLoader.Exists("res://Textures/Icons/" + Emote + ".png"))
            {
                continue;
            }
            Target.BbcodeText = Regex.Replace(Target.BbcodeText, "\\\\em\\((.*?)\\)", "[img]res://Textures/Icons/$1.png[/img]");
        }
    }
}
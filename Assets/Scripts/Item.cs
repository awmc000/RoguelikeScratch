using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public Dictionary<string, int> Stats;
    public Dictionary<string, bool> Flags;

    public Item(string name, string description, Sprite icon)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Stats = new Dictionary<string, int>();
        Flags = new Dictionary<string, bool>();
    }
}

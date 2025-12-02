using UnityEngine;

public enum ItemType
{
    Weapon,
    Potion,
    Material,
    Etc
}

public class Item : ScriptableObject
{
    public ItemType Type;
    public Sprite Icon;
    public string ItemName;
    public int MaxAmount;
}
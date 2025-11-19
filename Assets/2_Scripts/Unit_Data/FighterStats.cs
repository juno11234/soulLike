using UnityEngine;

[CreateAssetMenu(fileName = "FighterStats", menuName = "Combat/Fighter Stats", order = 0)]
public class FighterStats : ScriptableObject
{
    public int MaxHealth = 100;
    public float MaxStamina = 100f;
    public int Defense = 5;
}
using UnityEngine;

[CreateAssetMenu(fileName = "FighterStats", menuName = "Combat/Fighter Stats", order = 0)]
public class FighterStats : ScriptableObject
{
    public int MaxHealth = 100;
    public int AttackPower = 10;
    public int Defense = 5;
    
}
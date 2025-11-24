using UnityEngine;
[CreateAssetMenu(fileName = "New Attack",menuName = "AI/Attack Data")]
public class AttackData : ScriptableObject
{
    public string aniName;
    public float damage;
    
    public float windupTime;
    public float activeTime;
    public float recoveryTime;
    
    public float probability;
    public string animTrigger;
}

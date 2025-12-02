using UnityEngine;

public interface IAttackAble
{
    public void AttackForCollEnable();
    public void AttackForCollDisEnable();
}

public interface ISkillAble
{
    public void UseSkillFirstTiming(int skillIndex);
    public void UseSkillSecondTiming(int skillIndex);
}
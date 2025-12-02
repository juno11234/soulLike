using UnityEngine;

public class BossState : EnemyState, ISkillAble
{
    [SerializeField] private GameObject secondPhaseParticle;
    [SerializeField] private GameObject[] skillPrefabs;

    protected override void Dead()
    {
        base.Dead();
        secondPhaseParticle.SetActive(false);
    }

    protected override void Initialized()
    {
        if (enemyAI is BossMonsterAI bossAI)
        {
            fighterView.OnSecondPhase += bossAI.SecondPhase;
        }

        fighterView.OnSecondPhase += SecondParticleOn;
        secondPhaseParticle.SetActive(false);
    }

    private void SecondParticleOn()
    {
        secondPhaseParticle.gameObject.SetActive(true);
    }

    public void UseSkillFirstTiming(int skillIndex)
    {
        if (skillIndex == -1)
        {
            return;
        }

        Instantiate(skillPrefabs[skillIndex], transform.position, transform.rotation);
    }

    public void UseSkillSecondTiming(int skillIndex)
    {
        if (skillIndex == -1)
        {
            return;
        }

        Instantiate(skillPrefabs[skillIndex], transform.position, transform.rotation);
    }
}
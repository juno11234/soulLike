public static class CombatCalculator
{
    public static int CalculateDamage(int attackPower, FighterStats defender)
    {
        // 기본 대미지 계산 (추후 더 복잡한 공식으로 확장 가능)
        int damage = attackPower - defender.Defense;

        if (damage < 1)
        {
            damage = 1; // 최소 1의 대미지 보장
        }

        return damage;
    }
    
}
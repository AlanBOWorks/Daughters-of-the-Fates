using System;
using ___ProjectExclusive.Characters;
using UnityEngine;

namespace CombatSystem
{
    public static class DamageUtils
    {
        public static float CalculateDamageOfUser(CharacterCombatStatsHolder user, float damageModifier)
        {
            IOffensiveStats baseStats = user.MainStats;
            IOffensiveStats buffStats = user.BuffStats;
            IOffensiveStats burstStats = user.BurstStats;


            return baseStats.AttackPower * buffStats.AttackPower * burstStats.AttackPower * damageModifier;

            
        }
    }

    public static class VitalityUtils
    {
        public static float CalculateFinalDamage(CharacterCombatStatsHolder target, float damageDealt)
        {
            IVitalityStats baseStats = target.MainStats;
            IVitalityStats buffStats = target.BuffStats;
            IVitalityStats burstStats = target.BurstStats;
            float reduction = baseStats.DamageReduction;
            reduction += buffStats.DamageReduction;
            reduction *= 1 + burstStats.DamageReduction;
            // Reduction = (base + buff) * (1 + burst)

            return Mathf.Lerp(damageDealt, 0, reduction);
        }
        
        
        /// <returns>If dead or not</returns>
        public static bool DoDamageOnTarget(CharacterCombatStatsHolder target, float damageDealt)
        {
            ICombatTemporalStats stats = target.MainStats;
            stats.ShieldAmount = DoCalculation(stats.ShieldAmount);
            if (damageDealt <= 0) return false;

            stats.HealthPoints = DoCalculation(stats.HealthPoints);
            if (damageDealt <= 0) return false;

            stats.MortalityPoints = DoCalculation(stats.MortalityPoints);

            return stats.MortalityPoints >= 0;



            float DoCalculation(float stat)
            {
                stat -= damageDealt;
                if (stat >= 0) //if the damage exchanged is lower than the initial stat
                {
                    damageDealt = 0;
                    return stat;
                }
                damageDealt = -stat;
                return 0;
            }
        }

    }
}

using ___ProjectExclusive.Characters;
using CombatSystem;
using UnityEngine;

namespace CardSystem
{
    [CreateAssetMenu(fileName = "Damage - N [Card Effect]",
        menuName = "Card System/Effect/Damage")]
    public class SDamageEffect : SCardEffect
    {
        public override void DoEffect(CharacterCombatStatsHolder user, CharacterCombatStatsHolder target,
            float effectModifier = 1)
        {
            float userDamageOutput = DamageUtils.CalculateDamageOfUser(user, effectModifier);
            userDamageOutput = VitalityUtils.CalculateFinalDamage(target, userDamageOutput);

            VitalityUtils.DoDamageOnTarget(target, userDamageOutput);
        }
    }
}

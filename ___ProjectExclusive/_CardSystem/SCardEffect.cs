using ___ProjectExclusive.Characters;
using UnityEngine;

namespace CardSystem
{
    public abstract class SCardEffect : ScriptableObject, ICardEffect
    {
        public float CardPower = 1f;


        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="effectModifier">This modifier is meant to be used externally. <br></br>
        ///     <example>(eg: a field modifier for battle;
        ///         a character's special passive skill and/or stat;
        ///         buffs and debuffs</example></param>
        public abstract void DoEffect(CharacterCombatStatsHolder user, CharacterCombatStatsHolder target,
            float effectModifier = 1);
    }


}

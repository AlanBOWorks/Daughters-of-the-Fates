using System;
using System.Collections.Generic;
using CombatSystem;
using UnityEngine;

namespace CardSystem
{
    public static class CardUtils 
    {
        public static List<CombatSystemCharacter> GetCardsTarget(CombatSystemCharacter user, ICardData card)
        {
            CardTargets.TargetType targetType = card.GetTargetType();

            switch (targetType)
            {
                default:
                    return user.SelfAgent;
                case CardTargets.TargetType.Allies:
                    return user.Allies;
                case CardTargets.TargetType.Allies & CardTargets.TargetType.Self:
                    return user.Team.Members;
                case CardTargets.TargetType.Enemies:
                    return user.EnemyTeam.Members;
                case CardTargets.TargetType.All:
                    return CardCombatSystemSingleton.Instance.Entity.CurrentCharacters.ListCharactersInCombat;
            }
            
        }
    }
}

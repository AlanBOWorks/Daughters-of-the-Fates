using System.Collections.Generic;
using CardSystem;
using UnityEngine;

namespace CombatSystem
{
    public class CharacterTeam
    {
        public CharacterTeam(List<CombatSystemCharacter> members)
        {
            Members = members;
        }


        public List<CombatSystemCharacter> Members { get; private set; }

        public CharacterTeam EnemyTeam;



        public static List<CombatSystemCharacter> GetTarget(CombatSystemCharacter user, CardArchetypeBase.CardArchetype archetype)
        {
            //TODO if(!multiTarget)
            switch (archetype)
            {
                default:
                case CardArchetypeBase.CardArchetype.Buffer:
                case CardArchetypeBase.CardArchetype.Protector:
                    return user.Team.Members;
                case CardArchetypeBase.CardArchetype.Attacker:
                    return user.EnemyTeam.Members;
                
            }
        }

        public static List<CombatSystemCharacter> GetTarget(CombatSystemCharacter user, ICardData card) =>
            GetTarget(user, card.GetArchetype());

    }

}

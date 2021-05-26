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

        public void RemoveMember(CombatSystemCharacter member)
        {
            if(!Members.Contains(member)) return;
            Members.Remove(member);
            foreach (CombatSystemCharacter character in Members)
            {
                character.Allies.Remove(member);
            }
        }

    }

}

using CardSystem;
using SharedLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    [CreateAssetMenu(fileName = "Enemy Character Entity - N [Variable]",
        menuName = "Variable/Entity/Enemy Character")]
    public class EnemyEntityVariable : CharacterEntityVariableBase
    {

        [SerializeField] 
        private CharacterCombatStatsBase _combatStats = new CharacterCombatStatsBase();
        [SerializeField]
        private CharacterDeck _deck = new CharacterDeck();

        public override CharacterCombatStatsHolder GenerateCombatStats()
        {
            return new CharacterCombatStatsHolder(entityName,
                new CharacterCombatMainStats(_combatStats), 
                new CharacterBuffStats(), 
                new CharacterBuffStats());
        }

        public override IDeckCollection GetDeck()
        {
            return _deck;
        }

    }
}

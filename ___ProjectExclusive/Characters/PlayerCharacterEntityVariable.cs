using ___ProjectExclusive;
using ___ProjectExclusive.Characters;
using CardSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Player
{
    [CreateAssetMenu(fileName = "Player Character Entity - N [Variable]",
        menuName = "Variable/Entity/Player Character")]
    public class PlayerCharacterEntityVariable : CharacterEntityVariableBase,
        IAgentName
    {
        [Title("Data")] 
        [SerializeField] private string _characterName = "NULL";


        [Title("Variables")]
        [SerializeField] private SPlayerCharacterCombatStatsVariable _combatStatsVariable = null;
        [SerializeField] private SCharacterDeck _characterDeck = null;

        [Title("Prefabs")] 
        [SerializeField] private GameObject _characterPrefab = null;

        public GameObject InstantiateGameObject(Vector3 position, Quaternion rotation)
        {
            return GameObject.Instantiate(_characterPrefab, position, rotation);
        }


        public string GetAgentName() => _characterName;

        public SPlayerCharacterCombatStatsVariable GetCombatStats() => _combatStatsVariable;

        public override CharacterCombatStatsHolder GenerateCombatStats()
        {
            CharacterCombatMainStats mainStats = new CharacterCombatMainStats(
                _combatStatsVariable.BaseStats(), _combatStatsVariable.CharacterAffinities(), _combatStatsVariable);
            return new CharacterCombatStatsHolder(mainStats,
                new CharacterBuffStats(), 
                new CharacterBuffStats());
        }

        public override IDeckCollection GetDeck()
        {
            return _characterDeck;
        }
    }

}

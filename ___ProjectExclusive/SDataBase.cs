using System;
using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using _Player;
using CardSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive
{
    [CreateAssetMenu(fileName = "DataBase",
        menuName = "Singleton/DataBase")]
    public class SDataBase : ScriptableObject
    {
        [TabGroup("Player Characters")]
        public PlayerDataBase PlayerDataBase = DataBaseSingleton.Instance.PlayerDataBase;

        private void OnEnable()
        {
            PlayerDataBase.OnAfterDeserialize();
        }
    }

    [Serializable]
    public class PlayerDataBase
    {
        [TabGroup("Injector")] 
        public List<PlayerCharacterEntityVariable> PlayerCharacters 
            = new List<PlayerCharacterEntityVariable>(5);

        [TabGroup("Data Base"), PropertyOrder(-10)] 
        public List<CharacterData> CharactersData;

        [Serializable]
        public class CharacterData
        {
            [Title("Variable")] 
            public PlayerCharacterEntityVariable EntityVariable;

            [Title("Stats")] 
            [GUIColor(.4f,.8f,1f)]
            public string CharacterName;

            [DisableInEditorMode,DisableInPlayMode]
            public SPlayerCharacterCombatStatsVariable StatsVariable;
            public CharacterCombatStatsBase CombatStats;
            public CharacterStatsModifiers Grow;
            public CharacterCombatStatsBase Affinity;

            [Title("Deck")] 
            public SCharacterDeck DeckVariable = null;
            public CharacterDeck Deck = null;

            public CharacterData(PlayerCharacterEntityVariable variable)
            {
                EntityVariable = variable;
                UpdateCombatData();
                UpdateDeckData();
            }

            private void UpdateCombatData()
            {

                CharacterName = EntityVariable.GetAgentName();
                SPlayerCharacterCombatStatsVariable combatStats = EntityVariable.GetCombatStats();

                if (combatStats is null) return;
                StatsVariable = combatStats;

                CombatStats = StatsVariable.MainStats();
                Grow = StatsVariable.CharacterGrow();
                Affinity = StatsVariable.CharacterAffinities();
            }

            private void UpdateDeckData()
            {
                IDeckCollection deckCollection = EntityVariable.GetDeck();
                if (deckCollection is SCharacterDeck deckVariable)
                    DeckVariable = deckVariable;
                else
                {
                    return;
                }

                Deck = deckVariable.Data;
            }
        }

        [Button]
        public void OnAfterDeserialize()
        {
            CharactersData = new List<CharacterData>(PlayerCharacters.Count);
            foreach (PlayerCharacterEntityVariable variable in PlayerCharacters)
            {
                if(variable == null) continue;
                    CharactersData.Add(new CharacterData(variable));
            }
        }
    }

    public sealed class DataBaseSingleton
    {
        static DataBaseSingleton() { }
        private DataBaseSingleton() { }
        public static DataBaseSingleton Instance { get; } = new DataBaseSingleton();

        [SerializeField]
        public PlayerDataBase PlayerDataBase = new PlayerDataBase();
    }
}

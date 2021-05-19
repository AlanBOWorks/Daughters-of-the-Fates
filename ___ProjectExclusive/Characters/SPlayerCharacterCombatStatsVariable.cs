using System;
using CardSystem;
using SharedLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    [CreateAssetMenu(fileName = "Player Character Combat Stats - N [Variable]",
        menuName = "Variable/Combat/Player Character Stats")]
    public class SPlayerCharacterCombatStatsVariable : ScriptableObject, ISerializationCallbackReceiver,
        ICharacterUpgradable
    {
        [Title("Base")] 
        [SerializeField,TabGroup("Real")] 
        private CharacterCombatStatsBase _combatStatsBase = new CharacterCombatStatsBase();
        public CharacterCombatStatsBase MainStats() => _combatStatsBase;

#if UNITY_EDITOR
        [TabGroup("Debug"), ShowInInspector,DisableInEditorMode,DisableInPlayMode] 
        private CharacterCombatStatsBase _debugStats;
#endif

        [Title("Modifiers")]
        [InfoBox("Final value (excluding Deck) > Base + Affinity * Grow"),GUIColor(.2f,.8f,.4f)]
        [SerializeField]
        private CharacterStatsModifiers _characterGrow = new CharacterStatsModifiers(1);
        public CharacterStatsModifiers CharacterGrow() => _characterGrow;

        [SerializeField]
        private CharacterCombatStatsBase _characterAffinities = new CharacterCombatStatsBase();
        public CharacterCombatStatsBase CharacterAffinities() => _characterAffinities;

        public float OffensivePower
        {
            get => _characterGrow.OffensivePower;
            set => _characterGrow.OffensivePower = value;
        }
        public float SupportPower
        {
            get => _characterGrow.SupportPower;
            set => _characterGrow.SupportPower = value;
        }

        public float VitalityAmount
        {
            get => _characterGrow.VitalityAmount;
            set => _characterGrow.VitalityAmount = value;
        }
        public float CriticalChance
        {
            get => _characterGrow.CriticalChance;
            set => _characterGrow.CriticalChance = value;
        }
        public int DeckSize
        {
            get => _characterGrow.DeckSize;
            set => _characterGrow.DeckSize = value;
        }

        public ICharacterCombatStatsBase BaseStats()
        {
            return _combatStatsBase;
        }

        public int HandSize {
            get=> _combatStatsBase.HandSize;
            set => _combatStatsBase.HandSize = value;
        }

        public float Enlightenment
        {
            get => _characterGrow.Enlightenment;
            set => _characterGrow.Enlightenment = value;
        }


        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            UpdateDebug();
#endif        
        }
#if UNITY_EDITOR
        [Button]
        private void UpdateDebug()
        {
            _debugStats = new CharacterCombatStatsBase(_combatStatsBase,_characterAffinities, this);

        } 
#endif
    }

    [Serializable]
    public class CharacterStatsModifiers : IStatsUpgradable
    {
        [SerializeField,SuffixLabel("%")] private float _offensivePower = 1;
        [SerializeField, SuffixLabel("%")] private float _supportPower = 1;
        [SerializeField, SuffixLabel("%")] private float _vitalityAmount = 1;
        [SerializeField, SuffixLabel("%")] private float _criticalChance = .1f;
        [SerializeField, SuffixLabel("%")] private float _enlightenment = 1;
        [SerializeField, SuffixLabel("units")] private int _deckSize = 0;

        public CharacterStatsModifiers()
        {}

        public CharacterStatsModifiers(float initialValue)
        {
            _offensivePower = initialValue;
            _supportPower = initialValue;
            _vitalityAmount = initialValue;
            _criticalChance = initialValue;
            _enlightenment = initialValue;
            _deckSize = (int) initialValue * 100;
        }

        public float OffensivePower
        {
            get => _offensivePower;
            set => _offensivePower = value;
        }

        public float SupportPower
        {
            get => _supportPower;
            set => _supportPower = value;
        }

        public float VitalityAmount
        {
            get => _vitalityAmount;
            set => _vitalityAmount = value;
        }

        public float CriticalChance
        {
            get => _criticalChance;
            set => _criticalChance = value;
        }

        public float Enlightenment
        {
            get => _enlightenment;
            set => _enlightenment = value;
        }

        public int DeckSize
        {
            get => _deckSize;
            set => _deckSize = value;
        }
    }
}

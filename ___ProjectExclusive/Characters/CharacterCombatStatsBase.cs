using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    [Serializable]
    public class CharacterCombatStatsBase : ICharacterCombatStatsBase
    {
        [TitleGroup("Offensive")]
        [SerializeField, SuffixLabel("Units")] private float _attackPower = 10;
        [TitleGroup("Offensive")]
        [SerializeField, SuffixLabel("%")] private float _deBuffPower = 1;

        [TitleGroup("Support")]
        [SerializeField, SuffixLabel("Units")] private float _healPower = 10;
        [TitleGroup("Support")]
        [SerializeField, SuffixLabel("%")] private float _buffPower = 1;


        [TitleGroup("Vitality")]
        [SerializeField, SuffixLabel("%")] private float _maxHealth = 100;
        [TitleGroup("Vitality")]
        [SerializeField, SuffixLabel("%")] private float _mortalityPoints = 1000;
        [TitleGroup("Vitality")]
        [SerializeField, SuffixLabel("%")] private float _damageReduction = 0;

        
        [TitleGroup("Others")]
        [SerializeField, SuffixLabel("%")] private float _criticalChance = 0;

        [TitleGroup("Others")]
        [SerializeField, SuffixLabel("Cards")] private int _handSize = 5;

        public CharacterCombatStatsBase()
        {}

        public CharacterCombatStatsBase(ICharacterCombatStatsBase baseStats, ICharacterCombatStatsBase affinities,
            IStatsUpgradable upgradedStats)
        {
            AttackPower = baseStats.AttackPower + affinities.AttackPower * upgradedStats.OffensivePower;
            DeBuffPower = baseStats.DeBuffPower + affinities.DeBuffPower * upgradedStats.OffensivePower;

            HealPower = baseStats.HealPower + affinities.HealPower * upgradedStats.SupportPower;
            BuffPower = baseStats.BuffPower + affinities.BuffPower * upgradedStats.SupportPower;

            MaxHealth = baseStats.MaxHealth + affinities.MaxHealth * upgradedStats.VitalityAmount;
            MortalityPoints = baseStats.MortalityPoints + affinities.MortalityPoints * upgradedStats.VitalityAmount;
            DamageReduction = baseStats.DamageReduction;

            CriticalChance = baseStats.CriticalChance + affinities.CriticalChance * upgradedStats.CriticalChance;
            HandSize = baseStats.HandSize + affinities.HandSize;

        }

        protected CharacterCombatStatsBase(ICharacterCombatStatsBase baseStats)
        {
            AttackPower = baseStats.AttackPower;
            DeBuffPower = baseStats.DeBuffPower;

            HealPower = baseStats.HealPower;
            BuffPower = baseStats.BuffPower;

            MaxHealth = baseStats.MaxHealth;
            MortalityPoints = baseStats.MortalityPoints;
            DamageReduction = baseStats.DamageReduction;

            CriticalChance = baseStats.CriticalChance;
            HandSize = baseStats.HandSize;
        }

        public CharacterCombatStatsBase(bool zeroAll)
        {
            _attackPower = 0;
            _deBuffPower = 0;
            _healPower = 0;
            _buffPower = 0;
            _maxHealth = 0;
            _mortalityPoints = 0;
            _damageReduction = 0;
            _criticalChance = 0;
            _handSize = 0;
        }

        public float AttackPower { 
            get => _attackPower;
            set => _attackPower = value;
        }
        public float DeBuffPower {
            get => _deBuffPower;
            set => _deBuffPower = value;
        }
        public float HealPower {
            get => _healPower;
            set => _healPower = value;
        }
        public float BuffPower {
            get => _buffPower;
            set => _buffPower = value;
        }

        public float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }
        public float MortalityPoints
        {
            get => _mortalityPoints;
            set => _mortalityPoints = value;
        }
        public float DamageReduction
        {
            get => _damageReduction;
            set => _damageReduction = value;
        }

        public float CriticalChance
        {
            get => _criticalChance;
            set => _criticalChance = value;
        }

        public int HandSize { 
            get=> _handSize;
            set => _handSize = value;
        }


       
    }


    public class CharacterBuffStats : CharacterCombatStatsBase
    {
        public CharacterBuffStats() : base(true)
        {
            
        }
    }

    [Serializable]
    public class CharacterCombatMainStats : CharacterCombatStatsBase, ICharacterCombatStats, ISerializationCallbackReceiver
    {
        [ShowInInspector]
        public float HealthPoints { get; set; }
        [ShowInInspector]
        public float ShieldAmount { get; set; }
        [ShowInInspector]
        public float HarmonyAmount { get; set; }

        public CharacterCombatMainStats() : base()
        {}

        public CharacterCombatMainStats(ICharacterCombatStatsBase baseStats) : base(baseStats)
        {
            
        }
        public CharacterCombatMainStats(ICharacterCombatStatsBase baseStats, ICharacterCombatStatsBase affinities,
            IStatsUpgradable upgradedStats)
        : base (baseStats,affinities,upgradedStats)
        {
            HealthPoints = MaxHealth;
            ShieldAmount = 0;
            HarmonyAmount = 0;
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            HealthPoints = MaxHealth;
        }
    }

    public class CharacterCombatStatsHolder
    {
        public string CharacterName;
        public readonly ICharacterCombatStats MainStats;
        public readonly CharacterBuffStats BuffStats;
        public readonly CharacterBuffStats BurstStats;

        public CharacterCombatStatsHolder(string characterName,
            ICharacterCombatStats mainStats, CharacterBuffStats buffStats,
            CharacterBuffStats burstStats)
        {
            CharacterName = characterName;
            MainStats = mainStats;
            BuffStats = buffStats;
            BurstStats = burstStats;
        }

    }

    public interface ICharacterUpgradable
        : IStatsUpgradable
    {
        ICharacterCombatStatsBase BaseStats();
    }

    public interface ICharacterCombatStats : ICharacterCombatStatsBase,
        ICombatTemporalStats
    { }

    public interface ICharacterCombatStatsBase : 
        IOffensiveStats, ISupportStats, IVitalityStats,
        ISpecialStats, ICardStats
    { }
    public interface ICardStats
    {
        int HandSize { get; }
    }

    public interface IOffensiveStats
    {
        float AttackPower { get; set; }
        float DeBuffPower { get; set; }
    }

    public interface ISupportStats
    {
        float HealPower { get; set; }
        float BuffPower { get; set; }
    }

    public interface IVitalityStats 
    {
        float MaxHealth { get; set; }
        float MortalityPoints { set; get; }
        float DamageReduction { set; get; }
    }


    public interface ICombatTemporalStats
    {
        float HealthPoints { get; set; }
        float ShieldAmount { set; get; }
        float MortalityPoints { set; get; }
        float HarmonyAmount { get; set; }
    }


    public interface ISpecialStats
    {
        float CriticalChance { get; set; }
    }

    public interface IStatsUpgradable
    {
        float OffensivePower { get; set; }
        float SupportPower { get; set; }
        float VitalityAmount { get; set; }
        float CriticalChance { get; set; }
        float Enlightenment { get; set; }
        int DeckSize { get; set; }
    }
}

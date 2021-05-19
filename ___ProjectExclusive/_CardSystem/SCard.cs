using System;
using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CardSystem
{
    [CreateAssetMenu(fileName = "Card - N",
        menuName = "Card System/Card")]
    public class SCard : ScriptableObject, ICardData, ISerializationCallbackReceiver
    {
        [SerializeField] private string _cardName;
        [SerializeField,TextArea] private string _description = "NULL";
        [InfoBox("Negative value: Card will be not removed from the Deck")]
        [SerializeField,SuffixLabel("cards Per Use")] private int _cost = -1;
        [SerializeField, SuffixLabel("%")] private float _cardPower = 1;
        [SerializeField] private Sprite _cardImage = null;
        [SerializeField] private CardArchetypeBase.CardArchetype _archetype = CardArchetypeBase.CardArchetype.Others;

        public string CardName => _cardName;
        public string Description => _description;
        public int Cost => _cost;
        public float CardPower => _cardPower;
        public Sprite CardImage => _cardImage;
        public CardArchetypeBase.CardArchetype GetArchetype() => _archetype;

        

        [SerializeField] 
        private SCardEffect[] _serializedEffects = new SCardEffect[1];

        [ShowInInspector,DisableInEditorMode]
        private List<ICardEffect> _secondaryEffects;

        [Button]
        public void AddEffect(ICardEffect effect)
        {
            _secondaryEffects.Add(effect);
        }
        [Button]
        public void RemoveEffect(ICardEffect effect)
        {
            _secondaryEffects.Remove(effect);
        }

        private void OnEnable()
        {
            if (_cardName.Length < 1)
                _cardName = name;
        }

        public void DoEffect(CharacterCombatStatsHolder user, CharacterCombatStatsHolder target, float cardModifier = 1)
        {
            foreach (SCardEffect effect in _serializedEffects)
            {
                effect.DoEffect(user,target,CardPower * cardModifier);
            }

            foreach (ICardEffect effect in _secondaryEffects)
            {
                effect.DoEffect(user,target,CardPower * cardModifier);
            }
        }

        public void OnBeforeSerialize()
        {
            if (_secondaryEffects is null)
                _secondaryEffects = new List<ICardEffect>(0);
        }

        public void OnAfterDeserialize()
        {
            
        }
    }

    public abstract class CardArchetypeBase
    {
        /// <summary>
        /// It means to be use as an approximation of the final role of the card.<br></br>
        /// Cards could be not 100% of only one archetype
        /// <example>(eg: guard with a heal support effect)</example>, so this
        /// is enum could give an overall view of the card by showing the player a visual/description
        /// representation of what's the most likely use of this <see cref="ICardData"/>.<br></br>
        /// Also could be used for drop chances for some specific classes so they can receive the most
        /// fitting cards.
        /// </summary>
        public enum CardArchetype
        {

            Attacker = AttackerIndex,
            /// <summary>
            /// Cards relates to enable the ally/self by increasing the effectiveness of the targeting
            /// agent. In other words, they're meant to increase the chance of success in the battle
            /// </summary>
            Buffer = BufferIndex,
            /// <summary>
            /// Cards related to increase the survival chances for the self/ally. They're usually recovers, prevent or
            /// stall damage in some sort of way.
            /// </summary>
            Protector = ProtectorIndex,
            Others = OthersIndex
        }


        public static T GetValue<T>(ICardArchetype<T> archetype, CardArchetype type)
        {
            switch (type)
            {
                default:
                    return archetype.Others;
                case CardArchetype.Attacker:
                    return archetype.Attacker;
                case CardArchetype.Buffer:
                    return archetype.Buffer;
                case CardArchetype.Protector:
                    return archetype.Protector;
            }
        }


        public const int AttackerIndex = 0;
        public const int BufferIndex = AttackerIndex + 1;
        public const int ProtectorIndex = BufferIndex + 1;
        public const int OthersIndex = ProtectorIndex + 1;
        public const int ArchetypesAmount = OthersIndex + 1;
    }

    public interface ICardArchetype<out T>
    {
        T Attacker { get; }
        T Buffer { get; }
        T Protector { get; }
        T Others { get; }


        T GetArchetypeValue(CardArchetypeBase.CardArchetype archetype);
    }

    public class SerializableCardArchetype<T> : CardArchetypeBase, ICardArchetype<T>
    {
        [SerializeField] private T _attacker;
        [SerializeField] private T _buffer;
        [SerializeField] private T _protector;
        [SerializeField] private T _others;

        public T Attacker => _attacker;
        public T Buffer => _buffer;
        public T Protector => _protector;
        public T Others => _others;
        public T GetArchetypeValue(CardArchetype archetype)
        {
            return GetValue(this, archetype);
        }

    }

    public class CardArchetypeBase<T> : CardArchetypeBase, ICardArchetype<T>
    {
        public T Attacker => SerializedArchetypes[AttackerIndex];
        public T Buffer => SerializedArchetypes[BufferIndex];
        public T Protector => SerializedArchetypes[ProtectorIndex];
        public T Others => SerializedArchetypes[OthersIndex];
        public T[] SerializedArchetypes { get; }

        public CardArchetypeBase(ICardArchetype<T> fromArchetype)
        {
            SerializedArchetypes = new[]
            {
                fromArchetype.Attacker,
                fromArchetype.Buffer,
                fromArchetype.Protector,
                fromArchetype.Others
            };

        }

        public T GetArchetypeValue(CardArchetype archetype)
        {
            return GetValue(this, archetype);
        }
    }
}

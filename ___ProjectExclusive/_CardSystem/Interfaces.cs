using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using UnityEngine;

namespace CardSystem
{
    public interface ICardData : ICardEffect
    {
        string CardName { get; }
        string Description { get; }
        int Cost { get; }
        float CardPower { get; }
        Sprite CardImage { get; }

        CardArchetypeBase.CardArchetype GetArchetype();

    }

    public interface ICardEffect
    {
        void DoEffect(CharacterCombatStatsHolder user, CharacterCombatStatsHolder target,
            float effectModifier = 1);
    }

    public interface IDrawableCards
    {
        Queue<ICardData> DrawCards(int amount);
    }

    public interface IDrawableDeck : IDeckCollection, IDrawableCards
    {
        /// <summary>
        /// Unlike <seealso cref="IDeckCollection.RemoveCard"/> (which is a direct Dictionary remove)
        /// the <see cref="UsedCardDiscardOrReturn"/> uses Game Logic to determinate the removing method.
        /// <br></br>
        /// <example>(Eg: if <seealso cref="ICardData.Cost"/> less
        /// than 0 will be not removed from the <see cref="IDeckCollection"/> but just reduced
        /// on the <seealso cref="IDrawableCards.DrawCards"/>)</example>.
        /// </summary>
        void UsedCardDiscardOrReturn(ICardData card);
    }

    public interface IDeckCollection
    {
        /// <summary>
        /// int is the amount of card of the <see cref="ICardData"/> in the Deck
        /// </summary>
        Dictionary<ICardData, int> GenerateDeck();

        void AddCard(ICardData card, int amount);
        void RemoveCard(ICardData card, int amount);
        void ModifyAmount(ICardData card, int amount);
    }

    public interface ICardsPile
    {
        void Add(GameObject card, int index = -1);
        void Remove(GameObject card);
        void RemoveAt(int index);
        void RemoveAll();
        void UpdatePositions(bool animatePositions = true);

    }
}

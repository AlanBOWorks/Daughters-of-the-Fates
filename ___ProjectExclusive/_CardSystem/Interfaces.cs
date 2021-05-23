using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using CombatSystem;
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

    public interface ICardUser
    {
        ICardData Card { get; }
        CombatSystemCharacter User { get; }
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

    public interface IItemPile<in T>
    {
        /// <param name="animatePosition">Does animation on call (and will do calculations as a consequence).<br></br>
        /// False is recommendable if there's a lot of additions that can wait to be animated after all is added, then 
        /// <seealso cref="UpdatePositions"/>can be used to animate/update the positions (and doing the calculations
        /// just once)</param>
        void Add(T item, PileAnimation.Type animationType = PileAnimation.Type.None, int index = -1);
        void Remove(T item);
        void RemoveAt(int index);
        void RemoveAll();
        void UpdatePositions(bool animatePositions = true);

    }
}

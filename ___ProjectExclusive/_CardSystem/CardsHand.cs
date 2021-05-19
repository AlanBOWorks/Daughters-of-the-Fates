using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CardSystem
{
    public class CardsHand : IDrawableCards
    {
        [ShowInInspector,HideInEditorMode]
        public readonly List<ICardData> CardsInHand;
        [ShowInInspector,HideInEditorMode]
        public readonly CombatDeck UsingDeck;
        [ShowInInspector,HideInEditorMode]
        public readonly ICardStats CardStats;

        public CardsHand(CombatDeck usingDeck, ICardStats cardStats)
        {
            UsingDeck = usingDeck;
            CardStats = cardStats;
            this.CardsInHand = new List<ICardData>(cardStats.HandSize);
        }

        [Button,DisableInEditorMode]
        public Queue<ICardData> DrawAllCardPossible()
        {
            return DrawCards(CardStats.HandSize - CardsInHand.Count);
        }

        public Queue<ICardData> DrawCards(int amount = 1)
        {
            Queue<ICardData> drawnCards = UsingDeck.DrawCards(amount);
            foreach (ICardData card in drawnCards)
            {
                CardsInHand.Add(card);
            }

            return drawnCards;
        }

        public void OnUseSubtractCard(int handIndex)
        {
            CardsInHand.RemoveAt(handIndex);
        }

    }
}

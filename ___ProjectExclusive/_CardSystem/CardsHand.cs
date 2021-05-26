using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CardSystem
{
    public class CardsHand : IDrawableCards
    {
        [ShowInInspector, HideInEditorMode] 
        public readonly CombatSystemCharacter User;
        [ShowInInspector,HideInEditorMode]
        public readonly List<ICardData> CardsInHand;
        public readonly Dictionary<ICardData, int> AmountOfCards;

        [ShowInInspector,HideInEditorMode]
        public readonly CombatDeck UsingDeck;
        [ShowInInspector,HideInEditorMode]
        public readonly ICardStats CardStats;

        public readonly CardsDrawHandler DrawHandler;

        public CardsHand(CombatSystemCharacter user, CombatDeck usingDeck, ICardStats cardStats)
        {

            User = user;
            UsingDeck = usingDeck;
            CardStats = cardStats;
            this.CardsInHand = new List<ICardData>(cardStats.HandSize);
            this.AmountOfCards = new Dictionary<ICardData, int>(cardStats.HandSize);

            DrawHandler = new CardsDrawHandler(this);
        }

        public int GetAmountInHand(ICardData card) => AmountOfCards[card];

        [Button,DisableInEditorMode]
        public Queue<ICardData> DrawAllCardPossible()
        {
            return DrawCards(CardStats.HandSize - CardsInHand.Count);
        }

        public Queue<ICardData> DrawCards(int amount = 1)
        {
            return DrawHandler.DrawCards(amount);
        }

    }

    public class CardsDrawHandler : IDrawableCards
    {
        private readonly CardsHand _hand;

        public CardsDrawHandler(CardsHand hand)
        {
            _hand = hand;
        }

        public Queue<ICardData> DrawCards(int amount)
        {
            Queue<ICardData> drawnCards = _hand.UsingDeck.DrawCards(amount);
            List<ICardData> cardsInHand = _hand.CardsInHand;
            Dictionary<ICardData, int> amountOfCards = _hand.AmountOfCards;
            foreach (ICardData card in drawnCards)
            {
                cardsInHand.Add(card);
                if (amountOfCards.ContainsKey(card))
                    amountOfCards[card]++;
                else
                {
                    amountOfCards.Add(card, 1);
                }
            }
            return drawnCards;
        }
    }

}

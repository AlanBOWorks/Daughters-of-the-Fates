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
        public readonly CardsHandPusher HandPusher;

        public CardsHand(CombatSystemCharacter user, CombatDeck usingDeck, ICardStats cardStats)
        {

            User = user;
            UsingDeck = usingDeck;
            CardStats = cardStats;
            this.CardsInHand = new List<ICardData>(cardStats.HandSize);
            this.AmountOfCards = new Dictionary<ICardData, int>(cardStats.HandSize);

            DrawHandler = new CardsDrawHandler(this);
            HandPusher = new CardsHandPusher(this);
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

    public class CardsHandPusher
    {
        private readonly CardsHand _hand;

        private readonly Dictionary<ICardUser, CombatSystemCharacter> _chosenCards;
        private const int PredictedAmountOfCards = 4;

        public CardsHandPusher(CardsHand hand)
        {
            _hand = hand;
            _chosenCards = new Dictionary<ICardUser, CombatSystemCharacter>(PredictedAmountOfCards);
        }


        public void PrepareCardForPlay(ICardUser card, CombatSystemCharacter target)
        {
            _chosenCards.Add(card, target);
        }

        public void RemovePreparedCard(ICardUser card)
        {
            _chosenCards.Remove(card);
        }

        [Button, HideInEditorMode]
        public void PushCardToPreparedCardsPhase()
        {
            Dictionary<ICardData, int> amountOfCards = _hand.AmountOfCards;
            PrepareCardsPhase prepareCardsPhase =
                CardCombatSystemSingleton.Instance.Entity.GetCombatSection().GetPrepareCardsPhase();
            foreach (KeyValuePair<ICardUser, CombatSystemCharacter> chosenCard in _chosenCards)
            {
                ICardData card = chosenCard.Key.Card;
                CombatSystemCharacter user = chosenCard.Key.User;
                prepareCardsPhase.PrepareCard(new PreparedCard(
                    card,
                    user,
                    chosenCard.Value));

                amountOfCards[card]--;
                if (amountOfCards[card] <= 0)
                {
                    amountOfCards.Remove(card);
                }
            }
            _chosenCards.Clear();
        }
    }
}

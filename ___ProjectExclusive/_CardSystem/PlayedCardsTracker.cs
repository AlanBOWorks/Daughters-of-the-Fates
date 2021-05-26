using System.Collections;
using System.Collections.Generic;
using CombatSystem;
using UnityEngine;

namespace CardSystem
{
    public class PlayedCardsTracker 
    {
        private readonly Dictionary<CombatSystemCharacter, int> _characterOrder;
        public readonly List<PreparedCard>[] lastRoundCards;
        public readonly List<PreparedCard>[] playedCards;
        private const int PredictionOfAmountOfPlays = 2 * 2; //2 cards for "Harmony" * 2 (through 2 rounds) 

        private int _requestIndex;
        public PlayedCardsTracker()
        {
            _characterOrder = CardCombatSystemSingleton.Instance.Entity.CharacterRoundOrder;
            int amountOfCharacters = _characterOrder.Count;

            lastRoundCards = new List<PreparedCard>[amountOfCharacters];
            playedCards = new List<PreparedCard>[amountOfCharacters];
            PushToLastRoundListeners = new List<IOnPushPlayedCardsListener>(amountOfCharacters);

            for (int i = 0; i < amountOfCharacters; i++)
            {
                lastRoundCards[i] = new List<PreparedCard>(PredictionOfAmountOfPlays);
            }

            _requestIndex = 0;
        }

        public void ToInitialState()
        {
            _requestIndex = 0;
            PushPlayedToLastRound();
        }

        public List<PreparedCard> GetPreviousCardsToCurrent()
        {
            return lastRoundCards[_requestIndex];
        }

        public List<PreparedCard> GetNextCardsToCurrent()
        {
            int targetIndex = _requestIndex + 1;
            return targetIndex < lastRoundCards.Length 
                ? lastRoundCards[targetIndex]
                // if there's no next lastRoundCards, that means the next list is from the next round
                // which will be the first index of the actual played cards (this will be shown
                // when the last playable request for a card play)
                : playedCards[0];
        }


        public readonly List<IOnPushPlayedCardsListener> PushToLastRoundListeners;
        public void PushPlayedToLastRound()
        {
            for (int i = 0; i < lastRoundCards.Length; i++)
            {
                foreach (IOnPushPlayedCardsListener listener in PushToLastRoundListeners)
                {
                    listener.OnPushToLastRound(lastRoundCards[i], playedCards[i]);
                }

                lastRoundCards[i] = playedCards[i];

                if (playedCards[i].Count <= 0) continue;
                playedCards[i] = new List<PreparedCard>(PredictionOfAmountOfPlays);
            }
        }


        public void AddToPlayed(PreparedCard card)
        {
            int orderIndex = _characterOrder[card.User];
            playedCards[orderIndex].Add(card);
        }

        /// <summary>
        /// Used for "Quick Cards" that are meant to be played before all previous cards;
        /// </summary>
        public void AddToBeforeLastRounds(PreparedCard card)
        {
            int orderIndex = _characterOrder[card.User];
            lastRoundCards[orderIndex].Insert(0, card);
        }

    }

    public interface IOnPushPlayedCardsListener
    {
        void OnPushToLastRound(List<PreparedCard> removingCards, List<PreparedCard> nextLastCards);
    }

    public struct PreparedCard
    {
        public readonly ICardData Card;
        public readonly CombatSystemCharacter User;
        public readonly CombatSystemCharacter Target;

        public PreparedCard(ICardData card, CombatSystemCharacter user, CombatSystemCharacter target)
        {
            Card = card;
            User = user;
            Target = target;
        }

        public void DoEffect(float modifier = 1)
        {
            Card.DoEffect(User.Stats, Target.Stats, modifier);
        }

        public void DiscardFromDeck()
        {
            User.Deck.UsedCardDiscardOrReturn(Card);
        }
    }
}

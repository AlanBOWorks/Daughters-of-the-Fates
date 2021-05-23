using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardSystem
{
    public class CombatDeck : IDrawableDeck
    {
        public readonly Dictionary<ICardData, int> Deck;
        public bool RemoveCardOnEmpty;
        public int CurrentAmountOfCardInDeck { get; private set; }

        private CombatDeck(int defaultAmountOfDraws, bool removeCardOnEmpty)
        {
            DrawnCards = new Queue<ICardData>(defaultAmountOfDraws);
            RemoveCardOnEmpty = removeCardOnEmpty;
        }

        public CombatDeck(Dictionary<ICardData, int> deck,int defaultAmountOfDraws = 4, bool removeCardOnEmpty = true)
        : this(defaultAmountOfDraws,removeCardOnEmpty)
        {
            CurrentAmountOfCardInDeck = 0;
            //Clone the Deck so the original doesn't lose their amounts
            Deck = new Dictionary<ICardData, int>(deck.Count);
            foreach (KeyValuePair<ICardData, int> pair in deck)
            {
                Deck.Add(pair.Key,pair.Value);
                CurrentAmountOfCardInDeck += pair.Value;
            }

        }

        public CombatDeck(Dictionary<ICardData, int>[] decks, int defaultAmountOfDraws = 4, bool removeCardOnEmpty = true)
        : this(defaultAmountOfDraws,removeCardOnEmpty)
        {
            CurrentAmountOfCardInDeck = 0;
            int length = 0;
            for (int i = 0; i < decks.Length; i++)
            {
                length += decks[i].Count;
            }

            Deck = new Dictionary<ICardData, int>(length);
            for (int i = 0; i < decks.Length; i++)
            {
                Dictionary<ICardData, int> deck = decks[i];
                foreach (KeyValuePair<ICardData, int> pair in deck)
                {
                    if (Deck.ContainsKey(pair.Key))
                    {
                        Deck[pair.Key] += pair.Value;
                    }
                    else
                    {
                        Deck.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        public Dictionary<ICardData, int> GenerateDeck()
        {
            Dictionary<ICardData, int> generatedDeck 
                = new Dictionary<ICardData, int>(Deck.Count);
            foreach (KeyValuePair<ICardData, int> pair in Deck)
            {
                generatedDeck.Add(pair.Key, pair.Value);
            }

            return generatedDeck;
        }

        public int GetAmountOfCard(ICardData card)
        {
            return Deck[card];
        }

        public void AddCard(ICardData card, int amount = 1)
        {
            if(!Deck.ContainsKey(card)) return;
            Deck[card] += amount;
            CurrentAmountOfCardInDeck += amount;
        }

        public void RemoveCard(ICardData card, int amount = 1)
        {
            if(!Deck.ContainsKey(card)) return;

            int cardAmount = Deck[card];
            cardAmount -= amount;
            if (cardAmount <= 0 )
            {
                if(RemoveCardOnEmpty)
                    Deck.Remove(card);
                else
                {
                    cardAmount = 0;
                }
            }

            Deck[card] = cardAmount;
            CurrentAmountOfCardInDeck -= amount;
        }

        public void ModifyAmount(ICardData card, int amount)
        {
            if(!Deck.ContainsKey(card)) return;
            Deck[card] = amount;
        }


        //To avoid GC allocation
        [ShowInInspector,HideInEditorMode,DisableInPlayMode]
        public readonly Queue<ICardData> DrawnCards;
        public Queue<ICardData> DrawCards(int amount)
        {
            amount = Mathf.Min(amount,CurrentAmountOfCardInDeck); //To avoid drawing more cards that it allow
            DrawnCards.Clear();
            for (int i = 0; i < amount; i++)
            {
                DrawnCards.Enqueue(DrawSingle());
            }

            return DrawnCards;

            ICardData DrawSingle()
            {
                int randomPick = Random.Range(1, CurrentAmountOfCardInDeck + 1); //The min amount of cards is 1
                int loopCardAmount = 0;
                foreach (KeyValuePair<ICardData, int> pair in Deck)
                {
                    loopCardAmount += pair.Value;
                    /* if pair.Value == 0 in first check: this will be true since the random.Min is 1;

                     In next checks: some previous should do return
                     or this also will be skipped (since loopCardAmount didn't change);
                    */
                    if (loopCardAmount < randomPick) continue; 

                    Deck[pair.Key] = Mathf.Max(0,pair.Value -1); //negative will subtract the loopCardAmount
                    CurrentAmountOfCardInDeck--;
                    return pair.Key;
                }

                return null;
            }
        }

        /// <summary>
        /// Checks if the cards has a <see cref="ICardData.Cost"/> and decides if the card
        /// is should be removed or inserted in de <see cref="Deck"/>.
        /// </summary>
        public void UsedCardDiscardOrReturn(ICardData card)
        {
            // Cost means how many cards are removed from the Play (Hand and/or Deck);
            // 0 or lower means limitless uses, there for should be re-added to the deck after use
            // 1 is just the card in hand; Removing nor adding is necessary
            // +1: extra cards will removed from the Deck
            
            if(card.Cost == 1)
                return;

            if (card.Cost <= 0)
            {
                AddCard(card);
                return;
            }

            RemoveCard(card, card.Cost -1);
        }
    }
}

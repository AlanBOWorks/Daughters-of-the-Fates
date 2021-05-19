using System;
using System.Collections.Generic;
using SharedLibrary;
using UnityEngine;

namespace CardSystem
{
    [CreateAssetMenu(fileName = "CharacterDeck - N",
        menuName = "Card System/Deck/Character Deck")]
    public class SCharacterDeck : ScriptableVariable<CharacterDeck>, IDeckCollection
    {
        public Dictionary<ICardData, int> GenerateDeck()
        {
            return Data.GenerateDeck();
        }

        public void AddCard(ICardData card, int amount)
        {
            Data.AddCard(card,amount);
        }

        public void RemoveCard(ICardData card, int amount)
        {
            Data.RemoveCard(card,amount);
        }

        public void ModifyAmount(ICardData card, int amount)
        {
            Data.ModifyAmount(card,amount);
        }
    }

    [Serializable]
    public class CharacterDeck : IDeckCollection
    {
        [SerializeField]
        private List<CharacterDeckCard> _characterDeck = new List<CharacterDeckCard>(0);

        /// <summary>
        /// Generates a <seealso cref="Dictionary{TKey,TValue}"/> of Cards; This is generated in
        /// every call since it will be invoked for a manager of cards
        /// </summary>
        public Dictionary<ICardData, int> GenerateDeck()
        {
            Dictionary<ICardData, int> generatedDeck = new Dictionary<ICardData, int>(_characterDeck.Count);
            foreach (CharacterDeckCard deckCard in _characterDeck)
            {
                generatedDeck.Add(deckCard.CardReference, deckCard.AmountInDeck);
            }


            return generatedDeck;
        }

        private CharacterDeckCard GetCard(ICardData card)
        {
            foreach (CharacterDeckCard deckCard in _characterDeck)
            {
                if (deckCard.CardReference.Equals(card))
                {
                    return deckCard;
                }
            }

            return null;
        }

        public void AddCard(ICardData card, int amount = 1)
        {
            CharacterDeckCard deckCard = GetCard(card);
            if (deckCard is null) return;

            GetCard(card).AmountInDeck += amount;
        }

        public void RemoveCard(ICardData card, int amount = 1)
        {
            CharacterDeckCard deckCard = GetCard(card);
            if (deckCard is null) return;


            deckCard.AmountInDeck -= amount;
            if (deckCard.AmountInDeck <= 0)
                _characterDeck.Remove(deckCard);
        }

        public void ModifyAmount(ICardData card, int amount)
        {
            CharacterDeckCard deckCard = GetCard(card);
            if (deckCard is null) return;

            deckCard.AmountInDeck = amount;
        }
    }


    [Serializable]
    public class CharacterDeckCard
    {
        public SCard CardReference;
        public int AmountInDeck;

        public CharacterDeckCard(SCard card, int amount)
        {
            CardReference = card;
            AmountInDeck = amount;
        }
    }
}

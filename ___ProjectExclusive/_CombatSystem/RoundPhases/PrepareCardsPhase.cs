using System;
using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using CardSystem;
using MEC;
using Sirenix.OdinInspector;
using TurnSystem;
using Random = UnityEngine.Random;

namespace CombatSystem
{
    public class PrepareCardsPhase : ITurnPhase
    {

        private readonly List<CombatSystemCharacter> _systemCharacters;
        private readonly Dictionary<CombatSystemCharacter, int> _characterOrder;
        private readonly List<int> _randomPicks;

        private readonly List<PreparedCard>[] _onWaitCards;
        public readonly List<PreparedCard>[] PreparedCards;



        private const int PredictionOfAmountOfPlays = 2 * 2; //2 cards for "Harmony" * 2 (through 2 turns) 
        private int CalculateListLength() => PredictionOfAmountOfPlays * _systemCharacters.Count;
        public PrepareCardsPhase(CombatCharactersHolder charactersHolder)
        {
            _systemCharacters = charactersHolder.CharactersInCombat;
            int amountOfCharacters = _systemCharacters.Count;

            _characterOrder = new Dictionary<CombatSystemCharacter, int>(amountOfCharacters);
            _randomPicks = new List<int>(amountOfCharacters);

            _onWaitCards = new List<PreparedCard>[amountOfCharacters];
            PreparedCards = new List<PreparedCard>[amountOfCharacters];



            int amountOfList = CalculateListLength();
            for (int i = 0; i < amountOfCharacters; i++)
            {
                _onWaitCards[i] = new List<PreparedCard>(amountOfList);
            }

            _checkIsReady = IsReadyToPush;
        }




        private bool _isReadyToPush; 
        private bool IsReadyToPush() => _isReadyToPush;
        private readonly Func<bool> _checkIsReady;

        public IEnumerator<float> _DoStep()
        {
            _isReadyToPush = false;
            PrepareCharactersOrder();

            PreparePreviousCards();
            //TODO call for showUI (Card hands)
            yield return Timing.WaitUntilTrue(_checkIsReady);
            PushWaitingToPrepared();
        }

        private void PrepareCharactersOrder()
        {
            _characterOrder.Clear();
            _randomPicks.Clear();

            int amountOfCurrentCharacters = _systemCharacters.Count; //Characters can die and be removed of the initial total
            for (int i = 0; i < amountOfCurrentCharacters; i++)
            {
                _randomPicks.Add(i);
            }

            for (int i = 0; i < amountOfCurrentCharacters; i++)
            {
                int pick = Random.Range(0, _randomPicks.Count);
                _characterOrder.Add(_systemCharacters[i],_randomPicks[pick]);
            }
        }

        private void PreparePreviousCards()
        {
            int listLength = CalculateListLength();
            for (int i = 0; i < _onWaitCards.Length; i++)
            {
                PreparedCards[i] = _onWaitCards[i];

                if (_onWaitCards[i].Count <= 0) continue;
                _onWaitCards[i] = new List<PreparedCard>(listLength);
            }
        }

        private void PushWaitingToPrepared()
        {
            for (int i = 0; i < _onWaitCards.Length; i++)
            {
                foreach (PreparedCard card in _onWaitCards[i])
                {
                    PreparedCards[i].Add(card);
                }
            }
        }

        public void PrepareCard(PreparedCard card)
        {
            int orderIndex = _characterOrder[card.User];
            _onWaitCards[orderIndex].Add(card);
        }

        public void PrepareCardInFirst(PreparedCard card)
        {
            int orderIndex = _characterOrder[card.User];
            _onWaitCards[orderIndex].Insert(0,card);
        }


        public void RemovePreparedCard(CombatSystemCharacter user, int handIndex)
        {
            int orderIndex = _characterOrder[user];
            // Uses list instead or caching through a Dictionary all used cards mainly because
            // the list normally aren't that big (generally 1 card; being 4 the highest)
            List<PreparedCard> cards = _onWaitCards[orderIndex]; 
            for (int i = 0; i < cards.Count; i++)
            {
                PreparedCard iCard = cards[i];
                if (iCard.HandIndex != handIndex) continue;

                cards.RemoveAt(i);
                break;
            }
            
        }

        [Button]
        public void ConfirmPlayAllCards()
        {
            _isReadyToPush = true;
        }

    }


    public struct PreparedCard
    {
        public readonly ICardData Card;
        public readonly int HandIndex;
        public readonly CombatSystemCharacter User;
        public readonly CombatSystemCharacter Target;

        public PreparedCard(ICardData card,int handIndex, CombatSystemCharacter user, CombatSystemCharacter target)
        {
            Card = card;
            HandIndex = handIndex;
            User = user;
            Target = target;
        }

        public void DoEffect(float modifier = 1)
        {
            Card.DoEffect(User.CharacterCombatStats,Target.CharacterCombatStats,modifier);
        }

        public void DiscardFromHand()
        {
            User.Hand.OnUseSubtractCard(HandIndex);
        }

        public void DiscardFromDeck()
        {
            User.Deck.UsedCardDiscardOrReturn(Card);
        }
    }
}

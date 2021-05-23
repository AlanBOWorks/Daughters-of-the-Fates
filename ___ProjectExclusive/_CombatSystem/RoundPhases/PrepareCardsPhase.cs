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

        public readonly List<IOnPreparedCharactersOrder> CharactersOrdersListeners;
        private const int PredictedAmountOfListeners = 4;

        public PrepareCardsPhase(CombatCharactersHolder charactersHolder)
        {
            _systemCharacters = charactersHolder.ListCharactersInCombat;
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

            CharactersOrdersListeners = new List<IOnPreparedCharactersOrder>(PredictedAmountOfListeners);
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

            foreach (IOnPreparedCharactersOrder listener in CharactersOrdersListeners)
            {
                listener.UpdateOrder(_characterOrder);
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



        [Button]
        public void ConfirmPlayAllCards()
        {
            _isReadyToPush = true;
        }

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
            Card.DoEffect(User.CharacterCombatStats,Target.CharacterCombatStats,modifier);
        }

        public void DiscardFromDeck()
        {
            User.Deck.UsedCardDiscardOrReturn(Card);
        }
    }

    public interface IOnPreparedCharactersOrder
    {
        Dictionary<CombatSystemCharacter, int> CharactersOrder { get; }
        void UpdateOrder(Dictionary<CombatSystemCharacter, int> charactersOrder);
    }
}

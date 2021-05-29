
using System.Collections.Generic;
using CombatSystem;
using UnityEngine;

namespace CardSystem
{

    public abstract class PlayCardsRequest : ICardPlayRequest
    {
        protected readonly CombatSystemCharacter user;
        protected readonly List<PreparedCard> chosenCards;
        private const int PredictedMaxCards = 4;
        private bool _isFinish;
        public int maxCardPerPlay;

        protected PlayCardsRequest(CombatSystemCharacter character)
        {
            user = character;
            chosenCards = new List<PreparedCard>(PredictedMaxCards);
            maxCardPerPlay = 1;
        }

        public void RequestForPlay()
        {
            _isFinish = false;
            DoRequestForPlay();
        }

        protected abstract void DoRequestForPlay();

        public virtual void FinishRequests()
        {
            _isFinish = true;
        }

        public bool IsFinishPlaying()
        {
            return _isFinish;
        }

        public void PushPlay()
        {
            CardCombatSystemEntity entity = CardCombatSystemSingleton.Instance.Entity;
            PlayedCardsTracker tracker = entity.playedCardsTracker;
            foreach (PreparedCard chosenCard in chosenCards)
            {
                tracker.AddToPlayed(chosenCard);
            }
        }

        public void PrepareCard(ICardData card, CombatSystemCharacter target)
        {
            PreparedCard preparedCard = new PreparedCard(card, user,target);
            chosenCards.Add(preparedCard);
        }

        public void RemoveCard(ICardData card)
        {
            for (int i = 0; i < chosenCards.Count; i++)
            {
                if (card != chosenCards[i].Card) continue;
                chosenCards.RemoveAt(i);
                break;
            }
        }

        
    }

    public class PlayerPlayCardsRequest : PlayCardsRequest
    {
        private readonly PlayerCharacterEntity _characterEntity;
        public PlayerPlayCardsRequest(CombatSystemCharacter character) : base(character)
        {
            PlayerCombatSystemEntity entity=
                CardCombatSystemSingleton.Instance.PlayerEntity;
            _characterEntity = entity.CharacterEntityDictionary[character];

        }

        protected override void DoRequestForPlay()
        {
            _characterEntity.StatusHandler.EnableAllCards();
        } 
        public override void FinishRequests()
        {
            base.FinishRequests();
            DisableCards();
        }

        private void DisableCards()
        {
            _characterEntity.StatusHandler.DisableAllCards();
        }
    }

    public class AIPlayCardsRequest : PlayCardsRequest
    {
        private readonly CardsHand _hand;
        public AIPlayCardsRequest(CombatSystemCharacter character) : base(character)
        {
            _hand = character.Hand;
        }

        protected override void DoRequestForPlay()
        {
            PlayRandomCard();
            FinishRequests();
        }

        //TODO temporal; Switch with an actual AI that selects a card
        private void PlayRandomCard()
        {
            List<ICardData> cardsInHand = _hand.CardsInHand;
            int randomPick = Random.Range(0, cardsInHand.Count);
            ICardData pickCard = _hand.CardsInHand[randomPick];

            List<CombatSystemCharacter> cardsTarget = CardUtils.GetCardsTarget(user,pickCard);
            randomPick = Random.Range(0, cardsTarget.Count);
            CombatSystemCharacter targetCharacter = cardsTarget[randomPick];

            PrepareCard(pickCard,targetCharacter);
        }
    }
}

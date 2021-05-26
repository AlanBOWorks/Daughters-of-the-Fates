
using System.Collections.Generic;
using CombatSystem;
using UnityEngine;

namespace CardSystem
{

    public abstract class PlayCardsRequest : ICardPlayRequest
    {
        protected readonly CombatSystemCharacter user;
        protected readonly Dictionary<ICardData, CombatSystemCharacter> chosenCards;
        private const int PredictedMaxCards = 4;
        private bool _isFinish;
        public int maxCardPerPlay;

        protected PlayCardsRequest(CombatSystemCharacter character)
        {
            user = character;
            chosenCards = new Dictionary<ICardData, CombatSystemCharacter>(PredictedMaxCards);
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
            foreach (KeyValuePair<ICardData, CombatSystemCharacter> chosenCard in chosenCards)
            {
                PreparedCard preparedCard = new PreparedCard(
                    chosenCard.Key, user, chosenCard.Value);
                tracker.AddToPlayed(preparedCard);
            }
        }

        public void PrepareCard(ICardData card, CombatSystemCharacter target)
        {
            chosenCards.Add(card, target);
        }

        public void RemoveCard(ICardData card)
        {
            chosenCards.Remove(card);
        }
    }

    public class PlayerPlayCardsRequest : PlayCardsRequest
    {

        public PlayerPlayCardsRequest(CombatSystemCharacter character) : base(character)
        {

            UCardPilesManager handPilesManager =
                CardCombatSystemSingleton.Instance.PlayerEntity.cardPilesManager;
            _handPile = handPilesManager.characterPiles[character];

        }

        private readonly IItemPile<UCardHolder> _handPile;
        protected override void DoRequestForPlay()
        {
            // Initially all cards are Inactive so the player can't play cards before hand.
            SwitchPileState(CardsStatesManager.HandlerStates.Idle);
        } 
        public override void FinishRequests()
        {
            base.FinishRequests();
            SwitchPileState(CardsStatesManager.HandlerStates.Inactive);
        }

        private void SwitchPileState(CardsStatesManager.HandlerStates targetState)
        {
            CardsStatesManager manager = CardCombatSystemSingleton.Instance.PlayerEntity.cardsStatesManager;
            ICardStateHandler targetHandler = manager.GetState(targetState);

            foreach (UCardHolder cardHolder in _handPile.Items)
            {
                cardHolder.SwitchStateHandler(targetHandler);
            }
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

            base.chosenCards.Add(pickCard,targetCharacter);
        }
    }
}

using System.Collections.Generic;
using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CardSystem
{
    public class CharacterCardStatusHolder
    {
        public readonly CombatSystemCharacter user;
        public readonly IItemPile<UCardHolder> cards;

        /// <summary>
        /// holds the prepared cards and their respective initial position on hand
        /// </summary>
        public readonly Dictionary<UCardHolder, int> preparedCards;
        public readonly UCardPilesManager pilesManager;

        public CharacterCardStatusHolder(CombatSystemCharacter user)
        {
            this.user = user;
            PlayerCombatSystemEntity entity = CardCombatSystemSingleton.Instance.PlayerEntity;
            pilesManager = entity.cardPilesManager;

            cards = pilesManager.characterPiles[user];
            preparedCards = new Dictionary<UCardHolder, int>(cards.Items.Capacity);

        }

        public List<UCardHolder> GetHandHolders() => cards.Items;
    }

    public class ActiveCardsStatusHandler : ICardStateHandler
    {
        private readonly CharacterCardStatusHolder _statusHolder;
        private readonly CardsStatesManager _statesManager;

        public ActiveCardsStatusHandler(CharacterCardStatusHolder holder)
        {
            _statusHolder = holder;
            _statesManager = CardCombatSystemSingleton.Instance.PlayerEntity.cardsStatesManager;
        }

        public void EnableAllCards()
        {
            IItemPile<UCardHolder> cards = _statusHolder.cards;
            foreach (UCardHolder card in cards.Items)
            {
                card.SwitchStateHandler(this);
            }
        }

        public void DisableAllCards()
        {
            CardsStatesManager.HandlerStates targetState =
                CardsStatesManager.HandlerStates.Inactive;

            CardsStatesManager manager = CardCombatSystemSingleton.Instance.PlayerEntity.cardsStatesManager;
            ICardStateHandler targetHandler = manager.GetState(targetState);

            foreach (UCardHolder cardHolder in _statusHolder.cards.Items)
            {
                cardHolder.SwitchStateHandler(targetHandler);
            }
        }

        [ShowInInspector,DisableInPlayMode]
        public UCardHolder CurrentActiveCard { get; private set; }
        public void OnSwitchState(UCardHolder onHolder)
        {
            var idleState = _statesManager.IdleState;
            foreach (UCardHolder cardHolder in _statusHolder.cards.Items)
            {
                idleState.OnSwitchState(cardHolder);
            }
        }

        public void OnClick(UCardHolder onHolder)
        {
            if(CurrentActiveCard != null) 
            {
                OnCancel(CurrentActiveCard);
                _statesManager.TargetState.OnCancel(CurrentActiveCard);

                if (onHolder == CurrentActiveCard) //this means the current selected card was clicked again
                {
                    CurrentActiveCard = null;
                    return;
                }
            }

            _statesManager.TargetState.OnSwitchState(onHolder);
            CurrentActiveCard = onHolder;
        }

        public void OnCancel(UCardHolder onHolder)
        {
            Dictionary<UCardHolder, int> preparedCards = _statusHolder.preparedCards;
            if (!preparedCards.ContainsKey(onHolder)) return;

            IItemPile<UCardHolder> pile = _statusHolder.cards;
            pile.Add(onHolder,PileAnimation.Type.Animated,preparedCards[onHolder]);
            preparedCards.Remove(onHolder);

            _statesManager.IdleState.OnSwitchState(onHolder);
        }

        /// <summary>
        /// Submits the target holder to prepared cards
        /// </summary>
        public void OnSubmit(UCardHolder onHolder)
        {
            CombatSystemCharacter user = _statusHolder.user;
            IItemPile<UCardHolder> pile = _statusHolder.cards;

            _statusHolder.pilesManager.AddCardToPreparation(
                onHolder,user,PileAnimation.Type.Animated);
            int index = pile.Items.IndexOf(onHolder);
            _statusHolder.preparedCards.Add(onHolder,index);

            CurrentActiveCard = null;
        }

        public void OnPointerEnter(UCardHolder onHolder)
        {
        }

        public void OnPointerExit(UCardHolder onHolder)
        {
        }
    }

    
}

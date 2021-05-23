using System;
using System.Collections.Generic;
using ___ProjectExclusive;
using CombatSystem;
using MEC;
using MPUIKIT;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CardSystem
{
    public class UCardHolder : MonoBehaviour, ICardInjection, IPointerClickHandler, ICardUser
    {
        [ShowInInspector, HideInEditorMode]
        public CombatSystemCharacter User { get; private set; }
        [ShowInInspector, HideInEditorMode]
        public ICardData Card { get; private set; }

        [FormerlySerializedAs("_visualFeedback")] [SerializeField,HideInPlayMode]
        private CardVisualsHolder visualsHolder = new CardVisualsHolder();

        public IdleCardStateHandler IdleStateHandler { get; private set; }
        public CardTypeHandler CardTypeHandler { get; private set; }

        private void Awake()
        {
            CardTypeHandler = new CardTypeHandler(this);
            IdleStateHandler = new IdleCardStateHandler(this);
        }

        public void InjectCard(CombatSystemCharacter user, ICardData card, bool isPlayer)
        {
            User = user;
            Card = card;
            visualsHolder.InjectCard(user, card,isPlayer);
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            CardStates.States currentState = IdleStateHandler.CurrentState;
            switch (currentState)
            {
                case CardStates.States.Idle:
                case CardStates.States.Hover:
                    IdleStateHandler.DoSwitchState(CardStates.States.Selected);
                    break;
                default:
                    IdleStateHandler.DoSwitchState(CardStates.States.Idle);
                    break;
            }
        }


    }

    public interface ICardInjection
    {
        void InjectCard(CombatSystemCharacter user, ICardData card, bool isPlayer);
    }
    
    /// <summary>
    /// Can be a Card in the player's hand (UI), prepared card (UI) or something special that can
    /// be interacted with
    /// </summary>
    public interface ICardStates
    {
        void DoSwitchState(CardStates.States targetState);
    }

    public static class CardStates
    {
        public enum States
        {
            Idle,
            Hover,
            Selected,
            Prepared
        }
    }

    public class IdleCardStateHandler : ICardStates
    {
        public readonly UCardHolder CardHolder;
        public CardStates.States CurrentState;

        public IdleCardStateHandler(UCardHolder cardHolder)
        {
            CardHolder = cardHolder;
        }


        public void DoSwitchState(CardStates.States targetState)
        {
            if(targetState == CurrentState) return;

            switch (targetState)
            {
                
                case CardStates.States.Idle:
                    RemoveSelectedState();
                    break;
                case CardStates.States.Hover:
                    //TODO animation hover
                    break;
                case CardStates.States.Selected:
                    CallTargetHandler();
                    break;
                case CardStates.States.Prepared:
                    PushCardToPreparedHand();
                    break;
            }
            CurrentState = targetState;
        }

        private void CallTargetHandler()
        {
            CardHolder.CardTypeHandler.CallTargetHandler();
            CurrentState = CardStates.States.Selected;
        }

        private void PushCardToPreparedHand()
        {
            //TODO Push card animation
            //TODO add card in pile of prepared cards (UI)
            CurrentState = CardStates.States.Idle;
        }

        private void RemoveSelectedState()
        {
            if (CurrentState == CardStates.States.Prepared)
            {
                //TODO removing selection (animation)
                CardHolder.User.Hand.HandPusher.RemovePreparedCard(CardHolder);
            }
            CurrentState = CardStates.States.Idle;

        }
    }

    public class CardTypeHandler
    {

        private readonly UCardHolder _cardHolder;

        public CardTypeHandler(UCardHolder cardHolder)
        {
            _cardHolder = cardHolder;
        }

        public void CallTargetHandler()
        {
            //TODO check if card is singleTarget
            bool isSingleTarget = false;
            if (isSingleTarget)
            {

                return;
            }

            //TODO else
            ICardTargetHandler targetHandler =
                CardCombatSystemSingleton.Instance.PlayerEntity.cardSelectorsManager;
            targetHandler.EnableSelectors(_cardHolder.User, _cardHolder.Card, _cardHolder.IdleStateHandler);
            //TODO do animation
        }

    }

    [Serializable]
    public class CardVisualsHolder : ISerializationCallbackReceiver, ICardInjection
    {
        [SerializeField] private MPImage _background = null;
        [SerializeField] private TextMeshProUGUI[] _mainColorText = new TextMeshProUGUI[4];

        [SerializeField] private MPImage _mainIcon = null;
        [SerializeField] private MPImage _iconBorder = null;
        [SerializeField] private Image _typeIcon = null;
        [SerializeField] private Image _typeSecondaryIcon = null;
        [SerializeField] private MPImage _gradientBorder = null;
        [SerializeField] private MPImage _line = null;

        [SerializeField] private TextMeshProUGUI _cardName = null;
        [SerializeField] private TextMeshProUGUI _costText = null;
        [SerializeField] private TextMeshProUGUI _costValue = null;
        [SerializeField] private TextMeshProUGUI _cardsAmount = null;

        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        private TextMeshProUGUI[] _texts;

        public void InjectCard(CombatSystemCharacter user, ICardData card, bool isPlayer)
        {
            CombatDeck deck = user.Deck;
            CardsHand hand = user.Hand;
            CardArchetypeBase.CardArchetype cardArchetype = card.GetArchetype();
            CharacterColors characterColors = GameThemeSingleton.GetArchetypeColor(
                GetControllableArchetype(isPlayer));
            Color cardColor = GameThemeSingleton.GetArchetypeColor(cardArchetype);
            Color neutralColor = characterColors.Neutral;

            Sprite cardTypeIcon = GameThemeSingleton.GetArchetypeIcon(cardArchetype);

            UpdateColors(cardColor);
            UpdateColors(characterColors);
            UpdateTypeIcon(cardTypeIcon);
            UpdateGradients(cardColor,neutralColor);

            _cardName.text = card.CardName;
            _mainIcon.sprite = card.CardImage;

            int amountOfCardsInTotal = deck.GetAmountOfCard(card) + hand.GetAmountInHand(card);
            _cardsAmount.text = amountOfCardsInTotal.ToString();
        }

        private static ControllableArchetypesBase.ControllableArchetypes GetControllableArchetype(bool isPlayer)
        {
            return (isPlayer)
                ? ControllableArchetypesBase.ControllableArchetypes.Player
                : ControllableArchetypesBase.ControllableArchetypes.Enemy;
        }

        private void UpdateColors(Color targetColor)
        {
            _mainIcon.color = targetColor;
            _typeIcon.color = targetColor;
            _line.color = targetColor;
            _iconBorder.color = targetColor;

            foreach (TextMeshProUGUI text in _texts)
            {
                text.color = targetColor;
            }
        }

        private void UpdateColors(CharacterColors colors)
        {
            _background.color = colors.MainColor;
            for (int i = 0; i < _mainColorText.Length; i++)
            {
                _mainColorText[i].color = colors.Text;
            }
        }

        private void UpdateGradients(Color mainColor, Color secondary)
        {
            Color[] borderGradient = _gradientBorder.GradientEffect.CornerGradientColors;
            borderGradient[0] = mainColor;
            borderGradient[2] = mainColor;

            borderGradient[1] = secondary;
            borderGradient[3] = secondary;
        }

        private void UpdateTypeIcon(Sprite sprite)
        {
            _typeIcon.sprite = sprite;
            _typeSecondaryIcon.sprite = sprite;
        }

        public void OnBeforeSerialize()
        {}

        public void OnAfterDeserialize()
        {
            _texts = new[]
            {
                _costText,
                _costValue
            };
        }
    }
}

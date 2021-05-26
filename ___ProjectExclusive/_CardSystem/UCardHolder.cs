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
    public class UCardHolder : MonoBehaviour, ICardInjection, ICardUser,
        ICardStateHandlerMono
    {
        [ShowInInspector, HideInEditorMode]
        public CombatSystemCharacter User { get; private set; }
        [ShowInInspector, HideInEditorMode]
        public ICardData Card { get; private set; }

        [FormerlySerializedAs("_visualFeedback")] [SerializeField,HideInPlayMode]
        private CardVisualsHolder visualsHolder = new CardVisualsHolder();

        [Title("State")]
        [ShowInInspector, HideInEditorMode, DisableInPlayMode]
        private ICardStateHandler _currentStateHandler;

        public void InjectCard(CombatSystemCharacter user, ICardData card, bool isPlayer)
        {
            User = user;
            Card = card;
            visualsHolder.InjectCard(user, card,isPlayer);
            SwitchStateHandler(CardsStatesManager.HandlerStates.Inactive);
        }
        public void SwitchStateHandler(ICardStateHandler handler)
        {
            _currentStateHandler = handler;
            _currentStateHandler.OnSwitchState(this);
        }
        private void SwitchStateHandler(CardsStatesManager.HandlerStates target)
        {
            CardsStatesManager manager = CardCombatSystemSingleton.Instance.PlayerEntity.cardsStatesManager;
            _currentStateHandler = manager.GetState(target);
            _currentStateHandler.OnSwitchState(this);
        }


        public void OnPointerClick(PointerEventData eventData) => OnSubmit();
        public void OnSubmit(BaseEventData eventData) => OnSubmit();
        public void OnSubmit()
        {
            _currentStateHandler.OnClick(this);
        }

        public void OnCancel(BaseEventData eventData) => OnCancel();
        public void OnCancel()
        {
            _currentStateHandler.OnCancel(this);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            _currentStateHandler.OnPointerEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _currentStateHandler.OnPointerExit(this);
        }


    }

    public interface ICardInjection
    {
        void InjectCard(CombatSystemCharacter user, ICardData card, bool isPlayer);
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

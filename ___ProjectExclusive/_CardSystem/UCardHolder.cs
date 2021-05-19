using ___ProjectExclusive;
using CombatSystem;
using MPUIKIT;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardSystem
{
    public class UCardHolder : MonoBehaviour, ISerializationCallbackReceiver
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

        public void InjectCard(ICardData data, bool isPlayer)
        {
            CardArchetypeBase.CardArchetype cardArchetype = data.GetArchetype();
            CharacterColors characterColors = GameThemeSingleton.GetArchetypeColor(
                GetControllableArchetype(isPlayer));
            Color cardColor = GameThemeSingleton.GetArchetypeColor(cardArchetype);
            Color neutralColor = characterColors.Neutral;

            Sprite cardTypeIcon = GameThemeSingleton.GetArchetypeIcon(cardArchetype);

            UpdateColors(cardColor);
            UpdateColors(characterColors);
            UpdateTypeIcon(cardTypeIcon);
            UpdateGradients(cardColor,neutralColor);

            _cardName.text = data.CardName;
            _mainIcon.sprite = data.CardImage;
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

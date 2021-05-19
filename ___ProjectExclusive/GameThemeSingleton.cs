using System;
using CardSystem;
using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive
{
    public sealed class GameThemeSingleton
    {
        static GameThemeSingleton() { }
        private GameThemeSingleton() { }
        public static GameThemeSingleton Instance { get; } = new GameThemeSingleton();

        public ColorThemeEntity ColorTheme => _entity.ColorTheme;
        public IconThemeEntity IconTheme => _entity.IconTheme;
        [SerializeField]
        private GameThemeEntity _entity = new GameThemeEntity();
        public GameThemeEntity Entity => _entity;

        public void Injection(SGameThemeSingleton variable)
        {
            _entity.Variable = variable;
            _entity.ColorTheme = variable.ColorTheme;
            _entity.IconTheme = variable.IconTheme;
        }

        public static CharacterColors GetArchetypeColor(ControllableArchetypesBase.ControllableArchetypes type)
        {
            return Instance.ColorTheme.ControllableColors.GetArchetypeValue(type);
        }
        public static Color GetArchetypeColor(CardArchetypeBase.CardArchetype type)
        {
            return Instance.ColorTheme.CardColors.GetArchetypeValue(type);
        }

        public static Sprite GetArchetypeIcon(CardArchetypeBase.CardArchetype type)
        {
            return Instance.IconTheme.IconArchetypes.GetArchetypeValue(type);
        }
    }

    [Serializable]
    public class GameThemeEntity
    {
        [SerializeReference] 
        public SGameThemeSingleton Variable = null;

        [SerializeReference]
        public ColorThemeEntity ColorTheme = null;
        [SerializeReference]
        public IconThemeEntity IconTheme = null;


    }

    [Serializable]
    public class ColorThemeEntity
    {
        [SerializeField]
        private ControllableColorArchetypes _controllableColor = new ControllableColorArchetypes();
        [SerializeField]
        private CardColorArchetypes _cardColors = new CardColorArchetypes();

        public ControllableColorArchetypes ControllableColors => _controllableColor;
        public CardColorArchetypes CardColors => _cardColors;
    }

    [Serializable]
    public class ControllableColorArchetypes : SerializableControllableArchetypes<CharacterColors>
    { }

    [Serializable]
    public class CardColorArchetypes : SerializableCardArchetype<Color>
    { }

    [Serializable]
    public struct CharacterColors
    {
        public Color MainColor;
        public Color Neutral;
        public Color Text;
    }

    [Serializable]
    public class IconThemeEntity
    {
        [SerializeField]
        private CardIconArchetypes _iconArchetypes = new CardIconArchetypes();

        public CardIconArchetypes IconArchetypes => _iconArchetypes;
    }

    [Serializable]
    public class CardIconArchetypes : SerializableCardArchetype<Sprite> 
    { }
}

using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive
{
    [CreateAssetMenu(fileName = "Game Theme [Singleton]",
        menuName = "Singleton/Game Theme")]
    public class SGameThemeSingleton : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private GameThemeEntity _singleton = GameThemeSingleton.Instance.Entity;
#endif

        [TitleGroup("Colors")]
        public ColorThemeEntity ColorTheme 
            = new ColorThemeEntity();
        [TitleGroup("Icons")]
        public IconThemeEntity IconTheme
            = new IconThemeEntity();

        [Button]
        public void InjectInSingleton()
        {
            GameThemeSingleton.Instance.Injection(this);
        }
    }
}

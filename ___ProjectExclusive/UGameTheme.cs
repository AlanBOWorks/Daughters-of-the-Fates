using UnityEngine;

namespace ___ProjectExclusive
{
    public class UGameTheme : MonoBehaviour
    {
        [SerializeField] 
        private SGameThemeSingleton _theme = null;

        private void Awake()
        {
            GameThemeSingleton.Instance.Injection(_theme);
        }

        private void Start()
        {
            Destroy(gameObject);
        }
    }
}

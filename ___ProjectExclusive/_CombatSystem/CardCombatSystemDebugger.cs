using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatSystem
{
    public class CardCombatSystemDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        [ShowInInspector, DisableInEditorMode]
        private CardCombatSystemEntity _combatSystem = CardCombatSystemSingleton.Instance.Entity;

#endif

    }
}


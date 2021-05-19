using Sirenix.OdinInspector;
using UnityEngine;

namespace _Player
{
    public class PlayerEntityDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        [ShowInInspector, DisableInEditorMode]
        private PlayerEntity _combatSystem = PlayerEntitySingleton.Instance.Entity;

#endif
    }
}

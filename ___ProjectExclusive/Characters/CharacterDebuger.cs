using _Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    public class CharacterDebuger : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField,HideInPlayMode]
        private PlayerCharacterEntityVariable _variable;
        [ShowInInspector,HideInEditorMode]
        private CharacterEntity _entity;

        private void Start()
        {
            _entity = _variable.Data;
            _variable = null;
        }

#endif    
    }
}

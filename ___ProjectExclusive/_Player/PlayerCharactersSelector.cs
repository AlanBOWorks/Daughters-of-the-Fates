using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Player
{
    public class PlayerCharactersSelector : MonoBehaviour
    {
        public PlayerCharacterEntityVariable VanguardRole = null;
        public PlayerCharacterEntityVariable BackGuardRole = null;

        private void Start()
        {
            if(VanguardRole != null && BackGuardRole != null)
                ConfirmCharacters();
        }

        [Button,DisableInEditorMode]
        public void ConfirmCharacters()
        {
            if(VanguardRole is null || BackGuardRole is null)
            {
#if UNITY_EDITOR
                Debug.Log("Null characters");
#endif

                return;
            }

            List<PlayerCharacterEntityVariable> playersCharacters =
            PlayerEntitySingleton.Instance.Entity.ControllingCharacters;

            playersCharacters.Clear();
            playersCharacters.Add(VanguardRole);
            playersCharacters.Add(BackGuardRole);

            Destroy(this);
        }
    }
}

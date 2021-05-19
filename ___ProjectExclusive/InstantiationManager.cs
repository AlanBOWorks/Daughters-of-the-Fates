using System.Collections.Generic;
using _Player;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ___ProjectExclusive
{
    public class InstantiationManager : MonoBehaviour
    {
        private void Awake()
        {
            Timing.RunCoroutine(_InstantiatePlayerCharacters());
        }

        private IEnumerator<float> _InstantiatePlayerCharacters()
        {
            yield return Timing.WaitUntilTrue(PlayerEntitySingleton.HasEnoughCharacters);
            Vector3 position = transform.position;
            for (var i = 0; i < PlayerEntitySingleton.Instance.Entity.ControllingCharacters.Count; i++)
            {
                PlayerCharacterEntityVariable entityVariable =
                    PlayerEntitySingleton.Instance.Entity.ControllingCharacters[i];
                entityVariable.InstantiateGameObject(position + Vector3.right * i, Quaternion.identity);
            }
        }
    }
}

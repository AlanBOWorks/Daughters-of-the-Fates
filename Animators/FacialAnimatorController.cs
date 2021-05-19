using Animancer;
using CharacterCreator;
using SharedLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Animators
{
    public class FacialAnimatorController : MonoBehaviour
    {
        [Title("Layers")]
        [SerializeField]
        private FacialLayersAnimator _layers = new FacialLayersAnimator();

        public void Awake()
        {
            _layers.Awake();
        }
    }

}

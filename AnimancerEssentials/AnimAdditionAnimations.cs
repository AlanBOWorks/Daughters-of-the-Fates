using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimancerEssentials
{
    public class AnimAdditionAnimations : MonoBehaviour
    {
        [SerializeReference] 
        private AnimancerComponent _animancer = null;
        [SerializeReference] 
        private MixerTransition2D _additionalAnimations = null;
        [SerializeField, 
         InfoBox("This layer will become an Additive layer as a result",InfoMessageType.Warning)] 
        private int _targetLayer = 1;
        public AnimancerState State;

        private void Awake()
        {
            State = _animancer.Layers[_targetLayer].GetOrCreateState(_additionalAnimations);
            _animancer.Layers[_targetLayer].IsAdditive = true;
            _animancer.Play(State);
        }
    }
}

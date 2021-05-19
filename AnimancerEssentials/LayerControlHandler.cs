using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimancerEssentials
{
    public class LayerControlHandler : MonoBehaviour
    {
        [SerializeReference,DisableInPlayMode] 
        private AnimancerComponent _animancer = null;
        private AnimancerLayer _layer = null;

        [SerializeField]
        private int _targetLayer = 0;
        [SerializeReference, DisableInPlayMode] 
        private AvatarMask _mask = null;

        public AnimancerLayer ControllingLayer()
        {
            return _layer;
        }

        private void Awake()
        {
            _layer = _animancer.Layers[_targetLayer];
            _animancer = null;

            if(_mask != null)
                _layer.SetMask(_mask);

        }

        private void OnEnable()
        {
            SetWeight(1);
        }

        private void OnDisable()
        {
            SetWeight(0);
        }


        public void SetWeight(float target)
        {
            _layer.Weight = target;
        }
        [Button]
        public void FadeWeight(float target)
        {
            _layer.SetWeight(target);
        }


        public AnimancerState SubscribeAnimation(AnimationClip clip)
        {
            return _layer.GetOrCreateState(clip);
        }
        public AnimancerState SubscribeTransition(ITransition transition)
        {
            return _layer.GetOrCreateState(transition);
        }
    }
}

using System.Collections.Generic;
using Animancer;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimancerEssentials
{
    public class AnimMixerTransitionHandler : MonoBehaviour
    {
        [SerializeReference] private LayerControlHandler _layerController = null;

        [SerializeReference, HideInPlayMode] private MixerTransition2D _transition = null;
        private MixerState.Transition2D _animations = null;

        private AnimancerState _state = null;

        public MixerState.Transition2D MixerTransition()
        {
            return _animations;
        }

        private void Awake()
        {
            _animations = _transition.Transition;
            _transition = null;
        }

        private void Start()
        {
            _state = _layerController.SubscribeTransition(_animations);
            OnEnable();
        }

        private void OnEnable()
        {
            _state?.Play();
        }
        private void OnDisable()
        {
            _state?.Stop();
        }

        public void UpdateAnimationParameters(Vector2 transitionParam)
        {
            _animations.State.Parameter = transitionParam;
        }

        [Button]
        private void TestParameters(Vector2 dir)
        {
            Timing.CallContinuously(3f,_Test);

            void _Test()
            {
                UpdateAnimationParameters(dir);
            }
        }


    }
}

using System;
using Animancer;
using AnimancerEssentials;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Animators
{
    [Serializable]
    public class FacialLayersAnimator : IBlinkHolder
    {
        [Title("References")]
        [SerializeReference, HideInPlayMode]
        private AnimancerComponent _animancer = null;

        [SerializeReference]
        private BlinkerHolder _blinkerHolder = null;

        [Title("Animations")]
        [SerializeField, HideInPlayMode]
        private AnimatorLayerParameters _baseLayer = new AnimatorLayerParameters(0, false);
        [SerializeField, HideInPlayMode] private LinearMixerTransition _baseAnimations = null;

        public LinearMixerRuntimeLayer BaseLayer = null;

        public void Awake()
        {
            BaseLayer = new LinearMixerRuntimeLayer(_animancer,_baseLayer,_baseAnimations.Transition);

            _baseLayer = null;
            _baseAnimations = null;

            BaseLayer.PlayLayer(1);
            BaseLayer.PauseState();

            _blinkerHolder.InjectAndStart(this);
        }

        public void AnimateCloseExpression(float eyesWeight)
        {
            BaseLayer.SetLayerWeight(eyesWeight);
        }
    }
}

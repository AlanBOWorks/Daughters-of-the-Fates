using System;
using Animancer;
using UnityEngine;

namespace AnimancerEssentials
{


    [Serializable]
    public class AnimatorLayerSetup
    {
        public bool IsAdditive = false;
        public AvatarMask Mask = null;
    }

    [Serializable]
    public class AnimatorLayerParameters
    {
        public readonly int LayerIndex;
        public readonly bool IsAdditive;
        public AvatarMask Mask = null;

        public AnimatorLayerParameters(int layerIndex, bool isAdditive)
        {
            LayerIndex = layerIndex;
            IsAdditive = isAdditive;
        }
    }

    public abstract class AnimatorRunTimeLayer
    {
        public readonly AnimancerLayer Layer = null;
        public AnimancerState State { get; protected set; }


        protected AnimatorRunTimeLayer(AnimancerComponent animancer, int layerIndex, bool isAdditive)
        {
            Layer = animancer.Layers[layerIndex];
            Layer.IsAdditive = isAdditive;
        }

        protected AnimatorRunTimeLayer(AnimancerComponent animancer, AnimatorLayerParameters layer)
        : this(animancer,layer.LayerIndex,layer.IsAdditive)
        {
            if (layer.Mask != null)
            {
                Layer.SetMask(layer.Mask);
            }
        }

        protected AnimatorRunTimeLayer(AnimancerComponent animancer, AnimatorLayerParameters layer,
            ITransition transition)
            : this(animancer, layer)
        {
            State = Layer.GetOrCreateState(transition);
        }


        public void PlayLayer(float layerWeight)
        {
            Layer.Weight = layerWeight;
            
            State.Play();
        }
        public void Stop()
        {
            State.Stop();
        }

        public void PauseState()
        {
            State.IsPlaying = false;
        }

        public void ResumeState()
        {
            State.IsPlaying = true;
        }

        public void SetLayerWeight(float weight)
        {
            Layer.Weight = weight;
        }

        public void SetStateWeight(float weight)
        {
            State.Weight = weight;
        }

        public void SetStateNormalizedTime(float targetTime)
        {
            State.NormalizedTime = targetTime;
        }

        public void SetCurrentStateNormalizedTime(float targetTime)
        {
            Layer.CurrentState.NormalizedTime = targetTime;
        }

    }

    public class LinearMixerRuntimeLayer : AnimatorRunTimeLayer
    {
        public readonly LinearMixerState.Transition Animations = null;
        public LinearMixerRuntimeLayer(AnimancerComponent animancer, AnimatorLayerParameters layer, LinearMixerState.Transition transition) : base(animancer, layer, transition)
        {
            Animations = transition;
        }


    }

    public class MixerAnimatorRunTimeLayer : AnimatorRunTimeLayer
    {
        public readonly MixerState.Transition2D Animations = null;

        public void InjectValues(Vector2 parameters)
        {
            Animations.State.Parameter = parameters;
        }

        public MixerAnimatorRunTimeLayer(AnimancerComponent animancer, AnimatorLayerParameters layer, MixerTransition2D transition) 
            : base(animancer, layer,transition)
        {
            Animations = transition.Transition;
        }

    }
}

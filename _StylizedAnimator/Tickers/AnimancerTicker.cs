using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{
    public class AnimancerTicker : TickerBase
    {
        private bool _enabled = true;
        [ShowInInspector]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Animancer.Playable.Speed = value ? 0 : 1;
            }
        }

        protected readonly AnimancerComponent Animancer;
        public AnimancerTicker(AnimancerComponent animancer)
        {
            Animancer = animancer;
            Enabled = true;
        }

        public override void DoTick(float deltaVariation)
        {
            AnimancerState state = Animancer.States.Current;
            if(!_enabled || state is null) return;
            state.Time += deltaVariation;
        }
    }
}

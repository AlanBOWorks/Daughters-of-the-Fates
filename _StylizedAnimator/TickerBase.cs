using UnityEngine;

namespace StylizedAnimator
{
    public abstract class TickerBase : IStylizedTicker
    {
        public abstract void DoTick(float deltaVariation);

        public void InjectInManager(int startInIndex = 0)
        {
            TickManagerSingleton.Instance.Entity.MainManager.AddTicker(this, startInIndex);
        }

        public void InjectInManager(StylizedTickManager.HigherFrameRate higherFrameRate)
        {
            InjectInManager((int)higherFrameRate);
        }
    }
}

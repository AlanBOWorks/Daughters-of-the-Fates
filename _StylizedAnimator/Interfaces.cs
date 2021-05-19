using MEC;
using UnityEngine;

namespace StylizedAnimator
{
    public interface IStylizedManagerHolder
    {
        StylizedTickManager GetManager();
    }

    public interface IStylizedTicker
    {
        void DoTick(float deltaVariation);
    }

}

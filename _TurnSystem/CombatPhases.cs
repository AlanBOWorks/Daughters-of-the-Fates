using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace TurnSystem
{
    public abstract class TurnPhaseBase : ITurnPhase
    {

        public abstract IEnumerator<float> _DoStep();
        protected abstract CoroutineHandle PhaseHandle();

        public void ForceStop()
        {
            Timing.KillCoroutines(PhaseHandle());
        }

        public bool IsRunning()
        {
            return PhaseHandle().IsRunning;
        }
    }
}

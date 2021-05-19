using System.Collections.Generic;
using UnityEngine;

namespace TurnSystem
{
    public interface ITurnPhase
    {
        IEnumerator<float> _DoStep();
    }

    /// <summary>
    /// While being the same than <see cref="ITurnPhase"/> it should be
    /// inherit for wrappers of <see cref="ITurnPhase"/> as a group
    /// </summary>
    public interface ITurnSection
    {

    }

    public interface ITurnSystemInvoker
    {

        Queue<IEnumerator<float>> GetPhases();
        /// <summary>
        /// Use it for reset/return to initial states
        /// </summary>
        void DoStopProcess();

    }
}

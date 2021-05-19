using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TurnSystem
{
    public class TurnManagerSingleton 
    {
        static TurnManagerSingleton() { }
        private TurnManagerSingleton() { }
        public static TurnManagerSingleton Instance { get; } = new TurnManagerSingleton();

        [SerializeField, HideInEditorMode, HideInPlayMode, HideInInlineEditors, HideDuplicateReferenceBox]
        public TurnManagerEntity Entity = new TurnManagerEntity();

    }

    [Serializable]
    public class TurnManagerEntity
    {
        public ITurnSystemInvoker Invoker;
        public Queue<IEnumerator<float>> Phases { get; private set; }

        private IEnumerator<float> _currentPhase;
        private CoroutineHandle _systemHandle;

        [Button]
        public void StartTurnSystem(ITurnSystemInvoker invoker)
        {
            if(Invoker != null) throw new Exception("The system is already running. Either the system didn't finish properly" +
                                              "or the system was called more than once");
            Invoker = invoker;
            Phases = invoker.GetPhases();

            _systemHandle = Timing.RunCoroutineSingleton(_DoTurnSystem(), _systemHandle, SingletonBehavior.Abort);
            IEnumerator<float> _DoTurnSystem()
            {
                while (Phases.Count > 0)
                {
                    _currentPhase = Phases.Dequeue();
                    yield return Timing.WaitUntilDone(_currentPhase);
                }

                Invoker = null;
            }
        }

        public void DirectStop()
        {
            Timing.KillCoroutines(_systemHandle);
            Invoker.DoStopProcess();
            Invoker = null;
        }
    }
}

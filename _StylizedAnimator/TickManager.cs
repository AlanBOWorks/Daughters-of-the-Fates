using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{
    public class StylizedTickManager
    {
        [ShowInInspector]
        private TickManagerParameters _parameters;
        public readonly UnityEngine.Object ManagerContainer;

        [ShowInInspector]
        private List<IStylizedTicker>[] _tickInvokers;
        private Dictionary<IStylizedTicker, int> _searchDictionary;
        private const int DefaultAmountOfTicksPerTier = 16;

        public StylizedTickManager(TickManagerParameters parameters, UnityEngine.Object managerContainer)
        {
            _parameters = parameters;
            InitializeCollections((int) parameters.FrameRateType);

            this.ManagerContainer = managerContainer;
        }

        private void InitializeCollections(int tickersAmount)
        {

            _tickInvokers = new List<IStylizedTicker>[tickersAmount];
            for (int i = 0; i < _tickInvokers.Length; i++)
            {
                _tickInvokers[i] = new List<IStylizedTicker>(DefaultAmountOfTicksPerTier);
            }
            _searchDictionary = new Dictionary<IStylizedTicker, int>(DefaultAmountOfTicksPerTier);
        }


        public void OnEnable()
        {
            Timing.ResumeCoroutines(_tickingHandle);
        }

        public void OnDisable()
        {
            Timing.PauseCoroutines(_tickingHandle);
        }

        public void AddTicker(IStylizedTicker ticker, int invokerIndex)
        {
            invokerIndex = ClampIndex(invokerIndex);

            List<IStylizedTicker> list = _tickInvokers[invokerIndex];
            list.Add(ticker);
            _searchDictionary.Add(ticker,invokerIndex);
        }

        public void AddTicker(IStylizedTicker ticker, HigherFrameRate targetRate)
        {
            AddTicker(ticker,(int) targetRate);
        }

        public void AddTicker(IStylizedTicker ticker)
        {
            AddTicker(ticker,_parameters.DefaultInjectionIndex);
        }

        public void RemoveTicker(IStylizedTicker ticker)
        {
            int invokerIndex = _searchDictionary[ticker];
            List<IStylizedTicker> list = _tickInvokers[invokerIndex];
            list.Remove(ticker);
            _searchDictionary.Remove(ticker);
        }

        public void ChangeTickTier(IStylizedTicker ticker,int targetInvokerIndex)
        {
            int invokerIndex = _searchDictionary[ticker];
            ChangeTickTier(ticker,targetInvokerIndex,invokerIndex);
        }

        public void ChangeTickTier(IStylizedTicker ticker, HigherFrameRate targetFrameRate)
        {
            ChangeTickTier(ticker,(int) targetFrameRate);
        }

        public void VariateTickerTiming(IStylizedTicker ticker, int variation)
        {
            int invokerIndex = _searchDictionary[ticker];
            int targetIndex = invokerIndex + variation;
            targetIndex = ClampIndex(targetIndex);
            ChangeTickTier(ticker, targetIndex, invokerIndex);
        }

        public CoroutineHandle GetTickHandle()
        {
            return _tickingHandle;
        }


        private int ClampIndex(int targetIndex)
        {
            return Mathf.Clamp(targetIndex, 0, _tickInvokers.Length - 1);
        }
        private void ChangeTickTier(IStylizedTicker ticker, int targetInvokerIndex, int invokerIndex)
        {
            List<IStylizedTicker> list = _tickInvokers[invokerIndex];
            list?.Remove(ticker);
            _searchDictionary.Remove(ticker);
            _tickInvokers[targetInvokerIndex].Add(ticker);
        }

        private CoroutineHandle _tickingHandle;
        public void StartTicks()
        {
            Timing.KillCoroutines(_tickingHandle);
            _tickingHandle = Timing.RunCoroutine(_CallTicks(0,1));
            _tickingHandle = Timing.RunCoroutine(_CallTicks(1,2));

            int frameRateTier = 4;
            for (int i = 2; i < _tickInvokers.Length && frameRateTier <= 60; i++)
            {
                Timing.LinkCoroutines(_tickingHandle, Timing.RunCoroutine(_CallTicks(i,frameRateTier)));

                if (frameRateTier <= 10 || frameRateTier > 50)
                    frameRateTier += 2; //To ignore odd numbers
                else
                {
                    if(frameRateTier < 48)
                        frameRateTier *= 2;
                    else
                    {
                        frameRateTier = 60;
                    }
                }
            }
        }

        



        private IEnumerator<float> _CallTicks(int index, int frameRate)
        {
            List<IStylizedTicker> tickers = _tickInvokers[index];
            float deltaVariation  = frameRate * Timing.DeltaTime;

            while (ManagerContainer)
            {
                yield return Timing.WaitForSeconds(deltaVariation);
                foreach (IStylizedTicker ticker in tickers)
                {
                    ticker.DoTick(deltaVariation);
                }
            }
        }

        public enum FrameRateType
        {
            LowBudget12 =   7, // 1,2,4,6,8,10,12 > 7 
            Traditional24 = 8, // 1,2,4,6,8,10,12,24
            Cinema48 =      9, // 1,2,4,6,8,10,12,24,48
            Smooth60 =      10 // 1,2,4,6,8,10,12,24,48,60
        }

        

        public enum LowerFrameRate
        {
            Ones,
            Twos,
            Fours,
            Sixes,
            Eights,
            Tens,
            Twelves
        }

        public enum HigherFrameRate
        {
            Half,
            Ones,
            Twos,
            Fours,
            Sixes,
            Eights,
            Tens,
            Twelves,
            TwentyFour,
            FortyEight,
            Sixty
        }

    }

    public class DebugStylizedTicker : IStylizedTicker
    {
        private string _debugName;
        public DebugStylizedTicker(string debugName)
        {
            _debugName = debugName;
        }
        public void DoTick(float deltaVariation)
        {
            Debug.Log($"{_debugName} - Tick variation: {deltaVariation}");
        }
    }

    public class RotationDebugStylizedTicker : IStylizedTicker
    {
        private Transform _rotateTransform;
        public RotationDebugStylizedTicker(Transform rotateThis)
        {
            _rotateTransform = rotateThis;
        }

        public void DoTick(float deltaVariation)
        {
            _rotateTransform.Rotate(Vector3.up, deltaVariation * 20);
        }
    }
}

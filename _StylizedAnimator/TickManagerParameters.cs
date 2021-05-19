using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{

    [Serializable]
    public class TickManagerParameters
    {


        [Title("List params")] [SuffixLabel("Lists")] 
        [SerializeField, HideInPlayMode]
        private StylizedTickManager.FrameRateType _frameRateType = StylizedTickManager.FrameRateType.Cinema48;
        public StylizedTickManager.FrameRateType FrameRateType => _frameRateType;


        [SerializeField] private int _defaultInjectionIndex = 3;
        public int DefaultInjectionIndex => _defaultInjectionIndex;

#if UNITY_EDITOR
        [SerializeField,HideInPlayMode] private bool _debugOnTick = true;
        public bool DebugOnTick => _debugOnTick;
#endif

        [Button,DisableInEditorMode]
        public void DoSwitchFrameRate(int vSyncAmount = 0)
        {
            SwitchFrameRate(_frameRateType, vSyncAmount);
        }

        public static void SwitchFrameRate(StylizedTickManager.FrameRateType targetFrameRate, int vSyncAmount = 0)
        {
            vSyncAmount = (vSyncAmount > 4) ? 4 : vSyncAmount;
            vSyncAmount = (vSyncAmount < 0) ? 0 : vSyncAmount;
            QualitySettings.vSyncCount = vSyncAmount;

            Application.targetFrameRate = (int) targetFrameRate;
        }
    }
}

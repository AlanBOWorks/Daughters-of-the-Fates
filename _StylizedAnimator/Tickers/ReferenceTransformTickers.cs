using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{
    [Serializable]
    public class ReferenceRotationTicker : ReferenceTicker
    {
        public override void DoTick(float deltaVariation)
        {
            if(!IsEnabled) return;
            applyOnTransform.rotation = reference.rotation;
        }

        public ReferenceRotationTicker(Transform applyOn, Transform onReference) : base(applyOn, onReference)
        {
        }

    }
    [Serializable]
    public class ReferencePositionTicker : ReferenceTicker
    {
      

        public override void DoTick(float deltaVariation)
        {
            if(!IsEnabled) return;
            applyOnTransform.position = reference.position;
        }

        public ReferencePositionTicker(Transform applyOn, Transform onReference) : base(applyOn, onReference)
        {
        }
    }

    [Serializable]
    public class ReferenceCanvasPositionTicker : ReferenceTicker
    {
        private Camera _renderingOnCamera;

        public ReferenceCanvasPositionTicker(Transform applyOn, Transform onReference, Camera renderingOn) : base(applyOn, onReference)
        {
            _renderingOnCamera = renderingOn;
        }

        public override void DoTick(float deltaVariation)
        {
            applyOnTransform.position = _renderingOnCamera.WorldToScreenPoint(reference.position);
        }
    }

    public abstract class ReferenceTicker : IStylizedTicker
    {
        public bool IsEnabled = true;

        protected readonly Transform applyOnTransform;
        protected readonly Transform reference;

        protected ReferenceTicker(Transform applyOn, Transform onReference) 
        {
            this.applyOnTransform = applyOn;
            reference = onReference;
        }

        public void InjectInManager(int startInIndex = 0)
        {
            TickManagerSingleton.Instance.Entity.MainManager.AddTicker(this, startInIndex);
        }

        public void InjectInManager(StylizedTickManager.HigherFrameRate higherFrameRate)
        {
            InjectInManager((int) higherFrameRate);
        }


        public abstract void DoTick(float deltaVariation);
    }
}

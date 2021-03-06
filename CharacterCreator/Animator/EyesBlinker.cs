
using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using SMaths;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animators
{
    [Serializable]
    public class EyesBlinkerCalculator
    {
        [Title("References")]
        [SerializeField] private SkinnedMeshRenderer _leftIris = null;
        [SerializeField] private SkinnedMeshRenderer _rightIris = null;

        private const int IrisFocusBlendIndex = 4;
        private int _blinkedCount;

        [Title("Params")]
        public SRange CloseSpeed = new SRange(40,60);
        public SRange OpenSpeed = new SRange(10,24);

        [SerializeField]
        private AnimationCurve _blinkCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
        [SerializeField]
        private AnimationCurve _irisFocusCurve = new AnimationCurve(new Keyframe(0,0), new Keyframe(.7f,1));
        private const float ComparisionThreshold = .01f;
        private int _blinkConsecutiveThreshold = 3;



        private bool _enabled = false;
        /// <summary>
        /// To force a stop if needed
        /// </summary>
        public CoroutineHandle BlinkingHandle { get; private set; }

        /// <summary>
        /// Will stop blinking after the animations are done
        /// </summary>
        public void SafeStopBlinking()
        {
            _enabled = false;
        }

        public void StartBlink(IBlinkHolder expressions)
        {
            Timing.KillCoroutines(BlinkingHandle);
            BlinkingHandle = Timing.RunCoroutine(_StartBlink(expressions));
        }
        private IEnumerator<float> _StartBlink(IBlinkHolder expressions)
        {
            _enabled = true;
            while (_enabled)
            {
                //Too guarantee a wait after many consecutive Blinks
                if (_blinkedCount < _blinkConsecutiveThreshold)
                {
                    yield return Timing.WaitForSeconds(Random.Range(0, .6f));
                    _blinkedCount++;
                }
                else
                {
                    yield return Timing.WaitForSeconds(Random.Range(3f, 5f));
                    _blinkedCount = 0;
                    _blinkConsecutiveThreshold = Random.Range(1, 3);
                }

                yield return Timing.WaitUntilDone(_DoBlink());

                if (Random.Range(0, 3) == 1)
                {
                    yield return Timing.WaitUntilDone(_DoBlink());
                }
            }

            IEnumerator<float> _DoBlink()
            {
                float eyesWeight = 0;
                float closeSpeed = CloseSpeed.RandomInRange();
                //Closing
                while (eyesWeight < 1 - ComparisionThreshold)
                {
                    UpdateWithCurveExpression(1, closeSpeed);

                    yield return Timing.WaitForSeconds(Time.deltaTime);
                }
                expressions.AnimateCloseExpression(1);

                yield return Timing.WaitForSeconds(Random.Range(0.1f, .4f));

                //Opening
                _leftIris.SetBlendShapeWeight(IrisFocusBlendIndex, 100); //Force the iris to shape in the "focus" state
                _rightIris.SetBlendShapeWeight(IrisFocusBlendIndex, 100);

                float irisWeight = _leftIris.GetBlendShapeWeight(IrisFocusBlendIndex);
                float openSpeed = OpenSpeed.RandomInRange();

                while (irisWeight > 0 + ComparisionThreshold)
                {
                    UpdateWithCurveExpression(0,openSpeed);

                    irisWeight = _irisFocusCurve.Evaluate(eyesWeight) * 100;
                    _leftIris.SetBlendShapeWeight(IrisFocusBlendIndex, irisWeight);
                    _rightIris.SetBlendShapeWeight(IrisFocusBlendIndex,irisWeight);

                    yield return Timing.WaitForSeconds(Time.deltaTime);
                }

                expressions.AnimateCloseExpression(0);
                _leftIris.SetBlendShapeWeight(IrisFocusBlendIndex, 0);
                _rightIris.SetBlendShapeWeight(IrisFocusBlendIndex, 0);


                void UpdateWithCurveExpression(float targetWeight, float deltaSpeed)
                {
                    eyesWeight = Mathf.Lerp(eyesWeight, targetWeight, Time.deltaTime * deltaSpeed);

                    expressions.AnimateCloseExpression(_blinkCurve.Evaluate(eyesWeight));
                }
            }

            
        }
    }

    public interface IBlinkHolder
    {
        void AnimateCloseExpression(float eyesWeight);
    }
}

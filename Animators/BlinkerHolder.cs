using SharedLibrary;
using UnityEngine;

namespace Animators
{
    public class BlinkerHolder : MonoReferencer<IBlinkHolder>
    {
        public EyesBlinkerCalculator BlinkerCalculator = new EyesBlinkerCalculator();

        private void OnEnable()
        {
            if(Reference != null)
                BlinkerCalculator.StartBlink(Reference);
        }

        private void OnDisable()
        {
            BlinkerCalculator.SafeStopBlinking();
        }

        public void InjectAndStart(IBlinkHolder blinkHolder)
        {
            InjectReference(blinkHolder);
            BlinkerCalculator.StartBlink(blinkHolder);
        }
    }
}

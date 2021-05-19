using StylizedAnimator;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    public class CharacterStylizedTickerController
    {
        private ReferenceRotationTicker _rotationTicker;
        private ReferencePositionTicker _positionTicker;

        public void InjectRotation(ReferenceRotationTicker rotationTicker)
        {
            if (_rotationTicker != null) return;

            _rotationTicker = rotationTicker;
            TickManagerSingleton.GetTickManager().AddTicker(_rotationTicker);
        }
    }
}

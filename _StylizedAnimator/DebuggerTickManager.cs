using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{
    public class DebuggerTickManager : MonoBehaviour
    {
        [Title("FrameRate debug")]
        [SerializeField]
        private Transform[] _rotationTransforms = new Transform[0];
        private RotationDebugStylizedTicker[] _rotationDebug;

        [Title("Camera debug")] 
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _rotateThisOnCamera;
        [SerializeField] 
        private Transform _rotationOnReference;

        [SerializeField,HideInPlayMode]
        private StylizedTickManager.HigherFrameRate _targetFrameRateForRotation =
            StylizedTickManager.HigherFrameRate.Fours;

        private ReferenceRotationTicker _rotationTicker = null;
        private ReferencePositionTicker _positionTicker = null;

        void Start()
        {
            _rotationDebug = new RotationDebugStylizedTicker[_rotationTransforms.Length];

            TickerManagerEntity entity = TickManagerSingleton.Instance.Entity;
            StylizedTickManager manager = entity.MainManager;
            for (int i = 0; i < _rotationTransforms.Length; i++)
            {
                Transform rotation = _rotationTransforms[i];
                RotationDebugStylizedTicker debug = new RotationDebugStylizedTicker(rotation);
                _rotationDebug[i] = debug;

                manager.AddTicker(debug,i);
            }

            _rotationTransforms = null;

            InvokeRotationOnCamera();
        }

        void OnDestroy()
        {
            if(_rotationDebug is null) return;

            TickerManagerEntity entity = TickManagerSingleton.Instance.Entity;
            StylizedTickManager manager = entity.MainManager;
            foreach (RotationDebugStylizedTicker debugStylizedTicker in _rotationDebug)
            {
                manager.RemoveTicker(debugStylizedTicker);
            }

            if (_rotationTicker != null)
            {
                manager.RemoveTicker(_rotationTicker);
                manager.RemoveTicker(_positionTicker);
            }
        }

        private void InvokeRotationOnCamera()
        {
            if(_camera is null || _rotateThisOnCamera is null || _rotationOnReference is null) return;
            _rotateThisOnCamera.parent = _camera.transform;

            _rotationTicker = new ReferenceRotationTicker(_rotateThisOnCamera,_rotationOnReference);
            _positionTicker = new ReferencePositionTicker(_rotateThisOnCamera, _rotationOnReference);

            _positionTicker.InjectInManager();
            _rotationTicker.InjectInManager(_targetFrameRateForRotation);
        }

        [Button,DisableInEditorMode]
        private void RotateCameraOnReference(float forSeconds = 10, float speedRotation = 30)
        {
            Timing.CallContinuously(forSeconds, Rotation);
            void Rotation()
            {
                _camera.transform.RotateAround(_rotationOnReference.position,Vector3.up, Time.deltaTime * speedRotation);
            }
        }
    }
}

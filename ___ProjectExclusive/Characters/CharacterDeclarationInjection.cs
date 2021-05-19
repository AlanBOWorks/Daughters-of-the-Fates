using System;
using _Player;
using Animancer;
using Sirenix.OdinInspector;
using StylizedAnimator;
using UnityEngine;

namespace ___ProjectExclusive.Characters
{
    public class CharacterDeclarationInjection : MonoBehaviour
    {
        [Title("Entity")]
        [SerializeField] private PlayerCharacterEntityVariable _variable = null;
         
        [Title("Declarations")]
        [SerializeField] private CharacterTransformData _transformData = new CharacterTransformData();

        [Title("References")] [SerializeField] private AnimancerComponent _bodyAnimancer = null;

        [Title("Animations")] [SerializeField] private AnimationClip _idleAnimation = null;

        private void Start()
        {
            if(_variable is null) 
                throw new NullReferenceException($"There's not any Variable referenced in : {transform.name}");

            CharacterEntity entity = _variable.Data;

            entity.TransformData = _transformData;


            Transform mainCamera = Camera.main.transform;
            Transform mesh = _transformData.MeshRoot;
            Transform characterRoot = _transformData.Root;

            ReferenceRotationTicker rotationTicker 
                = new ReferenceRotationTicker(mesh,characterRoot);
            ReferencePositionTicker positionTicker
                = new ReferencePositionTicker(mesh,characterRoot);

            AnimancerTicker animancerTicker = new AnimancerTicker(_bodyAnimancer);

            // Entity Injections
            entity.PositionTicker = positionTicker;
            entity.RotationTicker = rotationTicker;
            entity.BodyAnimancer = _bodyAnimancer;

            // Manager Injections
            positionTicker.InjectInManager(StylizedTickManager.HigherFrameRate.Half);
            rotationTicker.InjectInManager(StylizedTickManager.HigherFrameRate.Twos);
            animancerTicker.InjectInManager(StylizedTickManager.HigherFrameRate.Twos);

            // ReParenting
            mesh.parent = mainCamera;
            mesh.localScale = Vector3.one;

            // Animations
            if(_idleAnimation)
                _bodyAnimancer.Play(_idleAnimation);

            Destroy(this);
        }

    }
}

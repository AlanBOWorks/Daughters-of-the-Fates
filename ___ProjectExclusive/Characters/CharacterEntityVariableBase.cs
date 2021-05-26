using System;
using Animancer;
using CardSystem;
using CombatSystem;
using SharedLibrary;
using Sirenix.OdinInspector;
using StylizedAnimator;
using UnityEngine;
using UnityEngine.Serialization;

namespace ___ProjectExclusive.Characters
{
    public abstract class CharacterEntityVariableBase : ScriptableVariable<CharacterEntity>, ISerializationCallbackReceiver
    {
        [Title("Data")]
        [SerializeField]
        protected string entityName = "NULL";


        public abstract CharacterCombatStatsHolder GenerateCombatStats();
        public abstract IDeckCollection GetDeck();

        public virtual void OnBeforeSerialize()
        {
            
        }

        public virtual void OnAfterDeserialize()
        {
            Data.Variable = this;
        }
    }


    [Serializable]
    public class CharacterEntity
    {
        [Title("Visual Related")]
        [ShowInInspector, DisableInEditorMode]
        public ICharacterTransformData TransformData;
        [DisableInEditorMode]
        public ReferenceRotationTicker RotationTicker = null;
        [DisableInEditorMode]
        public ReferencePositionTicker PositionTicker = null;
        [DisableInEditorMode]
        public AnimancerComponent BodyAnimancer = null;
        public ITurnCombatAnimator CombatAnimator;

        [Title("Combat Related")] 
        [ShowInInspector, DisableInPlayMode]
        public CharacterEntityVariableBase Variable;



        [Button, HideInEditorMode]
        public void PlayAnimation(AnimationClip clip)
        {
            BodyAnimancer.Play(clip);
        }
    }
}

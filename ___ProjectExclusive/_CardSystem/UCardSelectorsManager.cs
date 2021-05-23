using System;
using System.Collections.Generic;
using CombatSystem;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class UCardSelectorsManager : MonoBehaviour, ICombatStartListener, ICardTargetHandler, ICardUser
    {
       
        private Dictionary<CombatSystemCharacter,UCardTargetSelector> _selectors;
        [SerializeField] private UCardTargetSelector _cloneSelector = null;

        private const int PredictedMaxLength = 8;
        private void Awake()
        {
            _selectors = new Dictionary<CombatSystemCharacter, UCardTargetSelector>(PredictedMaxLength);
        }

        private void Start()
        {
            CardCombatSystemSingleton.Instance.PlayerEntity.cardSelectorsManager = this;
            CardCombatSystemSingleton.Instance.Entity.AddOnStartListener(this);
        }

#if UNITY_EDITOR
        private float _horizontalModifier = -1;
        private Vector3 _spawnLocalPosition = new Vector3(100,0);
#endif

        private void InstantiateSelectors(CombatSystemCharacter character)
        {
            UCardTargetSelector selector = GameObject.Instantiate(_cloneSelector, transform);
            selector.GameObjectEnabled = false;
            _selectors.Add(character,selector);
            selector.Injection(this);
            selector.Injection(character);

#if UNITY_EDITOR
            if (_horizontalModifier > 0)
            {
                _horizontalModifier++;
            }
            

            selector.transform.localPosition = _spawnLocalPosition * _horizontalModifier;
            _horizontalModifier *= -1;

#endif

        }

        public void DoStart(CombatCharactersHolder characters)
        {
            List<CombatSystemCharacter> charactersInCombat = characters.ListCharactersInCombat;

            foreach (CombatSystemCharacter character in charactersInCombat)
            {
                InstantiateSelectors(character);
            }
        }

        [ShowInInspector, DisableInEditorMode, DisableInPlayMode]
        public CombatSystemCharacter User { get; private set; }
        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        public ICardData Card { get; private set; }

        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        public ICardStates InteractingCard { get; private set; }

        public void PushCard(CombatSystemCharacter target)
        {
            if (User != null && Card != null)
            {
                User.Hand.HandPusher.PrepareCardForPlay(this,target);
                InteractingCard.DoSwitchState(CardStates.States.Prepared);
                DisableSelectors();
                RemoveSelected();
            }
        }

        public void EnableSelectors(CombatSystemCharacter user, ICardData card, ICardStates callback)
        {
            if (Card != null || User != null || InteractingCard != null)
            {
                DisableSelectors();
                RemoveSelected();
            }

            User = user;
            Card = card;
            InteractingCard = callback;
            List<CombatSystemCharacter> targets = CharacterTeam.GetTarget(user, card);
            foreach (CombatSystemCharacter target in targets)
            {
                _selectors[target].GameObjectEnabled = true;
            }
        }

        public void DisableSelectors()
        {
            foreach (KeyValuePair<CombatSystemCharacter, UCardTargetSelector> selector in _selectors)
            {
                selector.Value.GameObjectEnabled = false;
            }
        }

        public void RemoveSelected()
        {
            User = null;
            Card = null;
            InteractingCard.DoSwitchState(CardStates.States.Idle);
            InteractingCard = null;
        }

    }

    public interface ICardTargetHandler
    {
        void EnableSelectors(CombatSystemCharacter user, ICardData card, ICardStates callback);
        void DisableSelectors();
    }


}

using System;
using System.Collections.Generic;
using CombatSystem;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class UCardSelectorsManager : MonoBehaviour, ICombatStartListener, ICardTargetHandler
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

        [Title("Debugging")]
        [ShowInInspector, DisableInEditorMode, DisableInPlayMode]
        public CombatSystemCharacter CurrentUser { get; private set; }
        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        public UCardHolder CurrentCard { get; private set; }

        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        public ICardStateHandler InteractingCard { get; private set; }

        public void PushCard(CombatSystemCharacter target)
        {
            if (CurrentUser != null && CurrentCard != null)
            {
                CardCombatSystemEntity entity = CardCombatSystemSingleton.Instance.Entity;
                ICardPlayRequest requester = entity.CharacterRequester[CurrentUser];
                requester.PrepareCard(CurrentCard.Card,target);
                CurrentCard.CurrentStateHandler.OnSubmit(CurrentCard);
                DisableSelectors();
                RemoveSelected();
            }
        }

        public void EnableSelectors(CombatSystemCharacter user, UCardHolder card, ICardStateHandler callback)
        {
            if (CurrentCard != null || CurrentUser != null || InteractingCard != null)
            {
                DisableSelectors();
                RemoveSelected();
            }

            CurrentUser = user;
            CurrentCard = card;
            InteractingCard = callback;
            List<CombatSystemCharacter> targets = CardUtils.GetCardsTarget(user,card.Card);
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
            CurrentUser = null;
            CurrentCard = null;
            InteractingCard = null;
        }

    }

    public interface ICardTargetHandler
    {
        void EnableSelectors(CombatSystemCharacter user, UCardHolder card, ICardStateHandler callback);
        void DisableSelectors();
    }


}

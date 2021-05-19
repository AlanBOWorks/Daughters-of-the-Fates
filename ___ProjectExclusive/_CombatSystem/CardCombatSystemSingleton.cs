﻿using System;
using System.Collections.Generic;
using _Player;
using CardSystem;
using Sirenix.OdinInspector;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    public sealed class CardCombatSystemSingleton
    {
        static CardCombatSystemSingleton() { }
        private CardCombatSystemSingleton() { }
        public static CardCombatSystemSingleton Instance { get; } = new CardCombatSystemSingleton();

        [SerializeField, HideInEditorMode, HideInPlayMode, HideInInlineEditors, HideDuplicateReferenceBox]
        public CardCombatSystemEntity Entity = new CardCombatSystemEntity();

        public static void StartCombat(SCombatEnemiesPreset combatPreset)
        {
            Instance.Entity.StartCombat(combatPreset);
        }

        public static void ForceStopCombat()
        {
            TurnManagerSingleton.Instance.Entity.DirectStop();
        }


        public static int GetAmountOfCharacters()
        {
            return Instance.Entity.CurrentCharacters.GetAmountOfCharacters();
        }
    }

    [Serializable]
    public class CardCombatSystemEntity : ITurnSystemInvoker
    {
        [Title("Managers")]
        public CombatCharactersHolder CurrentCharacters = null;
        public UCardsHandManager CardsHandManager = null;

        [Title("Sections")]
        [SerializeField]
        private InCombatTurnSection _inCombatTurnSection = new InCombatTurnSection();
        private Queue<IEnumerator<float>> _phaseSections;

        private delegate void CombatStarEvent(CombatCharactersHolder characters);
        private event CombatStarEvent OnStart;
        public void AddOnStartListener(ICombatStartListener listener)
        {
            OnStart += listener.DoStart;
        }
        public void RemoveOnStatListener(ICombatStartListener listener)
        {
            OnStart -= listener.DoStart;
        }

        public void AddOnDrawListener(IPlayerDrawsListener listener)
        {
            _inCombatTurnSection.DrawPhase.OnDrawEvent += listener.OnDrawCards;
        }
        public void RemoveOnDrawListener(IPlayerDrawsListener listener)
        {
            _inCombatTurnSection.DrawPhase.OnDrawEvent -= listener.OnDrawCards;
        }

        [Button,DisableInEditorMode]
        public void StartCombat(SCombatEnemiesPreset combatPreset)
        {
            // Prepare the Combat
            DoInjection();
            DoEnqueue();


            // Starts Combat
            TurnManagerSingleton.Instance.Entity.StartTurnSystem(this);

            void DoInjection()
            {
                CurrentCharacters = new CombatCharactersHolder(
                    PlayerEntitySingleton.Instance.Entity.ControllingCharacters,
                    combatPreset.GetEnemies());

                _inCombatTurnSection.Injection(CurrentCharacters);

                OnStart?.Invoke(CurrentCharacters);
            }

            void DoEnqueue()
            {
                _phaseSections = new Queue<IEnumerator<float>>(3); //StartCombat, InCombat, EndCombat 
                //TODO make startCombatSection (where animations, text, show UI starts)
                _phaseSections.Enqueue(_inCombatTurnSection._DoSection());
                //TODO make endCombatSection (where rewards are given)
            }
        }

        public static void ForceStopCombat()
        {
            TurnManagerSingleton.Instance.Entity.DirectStop();
        }


        public Queue<IEnumerator<float>> GetPhases() => _phaseSections;

        public void DoStopProcess()
        {
        }
    }

    public static class CombatSystemUtility
    {
        public static void StartCombat(SCombatEnemiesPreset combatPreset)
        {
            CardCombatSystemSingleton.Instance.Entity.StartCombat(combatPreset);
        }

    }

    public interface ICombatStartListener
    {
        void DoStart(CombatCharactersHolder characters);
    }

}
using System;
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

        private CardCombatSystemSingleton()
        {
            Entity = new CardCombatSystemEntity();
            PlayerEntity = new PlayerCombatSystemEntity();
        }
        public static CardCombatSystemSingleton Instance { get; } = new CardCombatSystemSingleton();

        
        public CardCombatSystemEntity Entity;
        public PlayerCombatSystemEntity PlayerEntity;

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
        public Dictionary<CombatSystemCharacter, int> CharacterRoundOrder { get; private set; }
        public Queue<CombatSystemCharacter> CharactersOrder { get; private set; }

        public PlayedCardsTracker playedCardsTracker = null;

        public Dictionary<CombatSystemCharacter,ICardPlayRequest> CharacterRequester { get; private set; }

        [Title("Sections")]
        [SerializeField]
        private InCombatTurnSection _inCombatTurnSection = new InCombatTurnSection();
        public InCombatTurnSection GetCombatSection() => _inCombatTurnSection;

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
            Declaration();
            DoInjection();
            AfterInjectionDeclaration();
            DoEnqueue();


            // Starts Combat
            TurnManagerSingleton.Instance.Entity.StartTurnSystem(this);

            void Declaration()
            {
                CurrentCharacters = new CombatCharactersHolder(
                    PlayerEntitySingleton.Instance.Entity.ControllingCharacters,
                    combatPreset.GetEnemies());
                int amountOfCharacters = CurrentCharacters.GetAmountOfCharacters();
                CharacterRoundOrder = new Dictionary<CombatSystemCharacter, int>(
                    amountOfCharacters);
                CharactersOrder = new Queue<CombatSystemCharacter>(
                    amountOfCharacters);
                CharacterRequester = new Dictionary<CombatSystemCharacter, ICardPlayRequest>(
                    amountOfCharacters);
            }

            void DoInjection()
            {
                _inCombatTurnSection.Injection(CurrentCharacters);
                OnStart?.Invoke(CurrentCharacters);
            }

            void AfterInjectionDeclaration()
            {

                foreach (CombatSystemCharacter character in CurrentCharacters.PlayerCharactersInCombat.Characters)
                {
                    PlayerPlayCardsRequest cardsRequest = new PlayerPlayCardsRequest(character);
                    CharacterRequester.Add(character, cardsRequest);
                }

                foreach (CombatSystemCharacter character in CurrentCharacters.ListEnemiesInCombat)
                {
                    AIPlayCardsRequest cardsRequest = new AIPlayCardsRequest(character);
                    CharacterRequester.Add(character, cardsRequest);
                }
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


    public class PlayerCombatSystemEntity
    {
        // this stuff are related to the UI
        public UCardSelectorsManager cardSelectorsManager = null;
        public UPoolCardsManager poolCardsManager = null;
        public UCardPilesManager cardPilesManager = null;

        public CardsStatesManager cardsStatesManager;

        public PlayerCombatSystemEntity()
        {
            cardsStatesManager = new CardsStatesManager();
        }
    }
}

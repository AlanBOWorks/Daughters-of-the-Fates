using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    [SerializeField]
    public class InCombatTurnSection : ITurnSection 
    {
        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        public DrawPhase DrawPhase { get; private set; }


        [ShowInInspector, DisableInEditorMode, DisableInPlayMode]
        private PrepareCardsPhase _prepareCardsPhase;

        [ShowInInspector, DisableInEditorMode, DisableInPlayMode]
        private PlayCardsPhase _playCardsPhase;

        public void Injection(CombatCharactersHolder charactersHolder)
        {
            DrawPhase = new DrawPhase(charactersHolder);
            _prepareCardsPhase = new PrepareCardsPhase(charactersHolder);
            _playCardsPhase = new PlayCardsPhase(_prepareCardsPhase);
        }

        public bool IsCombatFinish()
        {
            //TODO make the check each time a card has been played
            return false;
        }

        public IEnumerator<float> _DoSection()
        {
            while (!IsCombatFinish())
            {
                yield return Timing.WaitUntilDone(DrawPhase._DoStep());
                yield return Timing.WaitUntilDone(_prepareCardsPhase._DoStep());
                yield return Timing.WaitUntilDone(_playCardsPhase._DoStep());
            }
        }
    }
}

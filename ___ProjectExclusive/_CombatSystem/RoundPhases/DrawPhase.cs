using System.Collections.Generic;
using CardSystem;
using MEC;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    public class DrawPhase : TurnPhaseBase
    {
        private CombatCharactersHolder _combatCharacterHolder;

        public delegate void OnDrawCards(CombatSystemCharacter character, Queue<ICardData> cards);
        public event OnDrawCards OnDrawEvent;

        public DrawPhase(CombatCharactersHolder characterHolder)
        {
            _combatCharacterHolder = characterHolder;
        }

        private CoroutineHandle _stepHandle;
        public override IEnumerator<float> _DoStep()
        {
            _stepHandle = Timing.CurrentCoroutine;

            //Draw enemies
            foreach (CombatSystemCharacter character in _combatCharacterHolder.EnemiesInCombat)
            {
                character.Hand.DrawAllCardPossible();
            }

            foreach (CombatSystemCharacter character in _combatCharacterHolder.PlayerCharactersInCombat.Characters)
            {
                Queue<ICardData> drawnCards = character.Hand.DrawAllCardPossible();
                OnDrawEvent?.Invoke(character,drawnCards);
            }

            yield break;//TODO do animation;
        }

        protected override CoroutineHandle PhaseHandle()
        {
            return _stepHandle;
        }
    }

    public interface IPlayerDrawsListener
    {
        void OnDrawCards(CombatSystemCharacter character,Queue<ICardData> cards);
    }
}

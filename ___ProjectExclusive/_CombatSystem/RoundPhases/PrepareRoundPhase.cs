using System;
using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using CardSystem;
using MEC;
using Sirenix.OdinInspector;
using TurnSystem;
using Random = UnityEngine.Random;

namespace CombatSystem
{
    public class PrepareRoundPhase : ITurnPhase
    {

        private readonly List<CombatSystemCharacter> _systemCharacters;
        private readonly RoundOrderHandler _roundOrderHandler;
        private readonly PlayedCardsTracker _playedCardsTracker;



        public PrepareRoundPhase(CombatCharactersHolder charactersHolder)
        {
            CardCombatSystemEntity entity = CardCombatSystemSingleton.Instance.Entity;
            _systemCharacters = charactersHolder.ListCharactersInCombat;

            _roundOrderHandler = new RoundOrderHandler();
            _playedCardsTracker = new PlayedCardsTracker();

            entity.playedCardsTracker = _playedCardsTracker;
        }

        public IEnumerator<float> _DoStep()
        {
            _roundOrderHandler.GenerateRandomCharactersOrder(_systemCharacters);
            _playedCardsTracker.ToInitialState();
            yield break; //TODO wait until all is prepared (animations, draws, orders, etc)
        }
    }



}

using System;
using System.Collections.Generic;
using CardSystem;
using MEC;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    public class PlayCardsPhase : ITurnPhase
    {
        private readonly PlayedCardsTracker _playedCardsTracker;
        private readonly Queue<CombatSystemCharacter> _charactersOrder;
        private readonly Dictionary<CombatSystemCharacter, ICardPlayRequest> _characterRequests;

        public PlayCardsPhase(CombatCharactersHolder charactersHolder)
        {
            CardCombatSystemEntity entity = CardCombatSystemSingleton.Instance.Entity;
            _playedCardsTracker = entity.playedCardsTracker;
            
            _charactersOrder = entity.CharactersOrder;
            _characterRequests = entity.CharacterRequester;
        }

        private Func<bool> _isPlayCardsFinish;
        public IEnumerator<float> _DoStep()
        {
            foreach (CombatSystemCharacter character in _charactersOrder)
            {
                Debug.Log($"Requesting: {character.Stats.CharacterName}");
                ICardPlayRequest request = _characterRequests[character];
                request.RequestForPlay();
                _isPlayCardsFinish = request.IsFinishPlaying;

                yield return Timing.WaitUntilTrue(_isPlayCardsFinish);
            }
        }
    }
}

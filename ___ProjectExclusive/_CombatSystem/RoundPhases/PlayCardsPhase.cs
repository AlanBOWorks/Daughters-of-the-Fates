using System.Collections.Generic;
using CardSystem;
using MEC;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    public class PlayCardsPhase : ITurnPhase
    {
        private readonly PrepareCardsPhase _preparedCards;

        private List<PreparedCard>[] GetPreparedCards() => _preparedCards.PreparedCards;

        public PlayCardsPhase(PrepareCardsPhase preparationCards)
        {
            _preparedCards = preparationCards;
        }

        public IEnumerator<float> _DoStep()
        {
            List<PreparedCard>[] currentPlayingCard = GetPreparedCards();

            for (int i = 0; i < currentPlayingCard.Length; i++)
            {
                for (int j = 0; j < currentPlayingCard[i].Count; j++)
                {
                    PreparedCard card = currentPlayingCard[i][j];
                    card.DoEffect();
                    yield return Timing.WaitForOneFrame; //TODO wait for animations
                    card.DiscardFromDeck();
                }
            }
        }
    }
}

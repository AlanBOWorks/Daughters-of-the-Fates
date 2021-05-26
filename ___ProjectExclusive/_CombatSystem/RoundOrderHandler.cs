using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class RoundOrderHandler 
    {
        private readonly Dictionary<CombatSystemCharacter, int> _characterOrder;
        private readonly Queue<CombatSystemCharacter> _charactersOrder;
        private readonly List<int> _randomPicks;

        public RoundOrderHandler()
        {
            CardCombatSystemEntity entity = CardCombatSystemSingleton.Instance.Entity;
            _characterOrder = entity.CharacterRoundOrder;
            _charactersOrder = entity.CharactersOrder;
            _randomPicks = new List<int>(_characterOrder.Count);
        }

        public void GenerateRandomCharactersOrder(List<CombatSystemCharacter> activeCharacters)
        {
            _characterOrder.Clear();
            _charactersOrder.Clear();
            _randomPicks.Clear();

            int amountOfCurrentCharacters = activeCharacters.Count; //Characters can die and be removed of the initial total
            for (int i = 0; i < amountOfCurrentCharacters; i++)
            {
                _randomPicks.Add(i);
            }

            for (int i = 0; i < amountOfCurrentCharacters; i++)
            {
                int pick = Random.Range(0, _randomPicks.Count);
                int randomPick = _randomPicks[pick];
                _characterOrder.Add(activeCharacters[i], randomPick);
                _charactersOrder.Enqueue(activeCharacters[randomPick]);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    public abstract class UCardPileBase : MonoBehaviour, ICardsPile
    {
        private List<GameObject> _cards = null;
        public List<GameObject> Cards => _cards;

        private const int DefaultCardsLength = 16;
        private void Awake()
        {
            _cards = new List<GameObject>(16);
        }

        public abstract void Add(GameObject card, int index = -1);
        public abstract void Remove(GameObject card);
        public abstract void RemoveAt(int index);
        public abstract void RemoveAll();
        public abstract void UpdatePositions(bool animatePositions = true);
    }
}

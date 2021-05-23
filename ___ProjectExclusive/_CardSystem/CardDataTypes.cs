using CombatSystem;
using UnityEngine;

namespace CardSystem
{
    public struct CardUser
    {
        public CombatSystemCharacter User;
        public ICardData Card;

        public CardUser(CombatSystemCharacter user, ICardData card)
        {
            User = user;
            Card = card;
        }
    }
}

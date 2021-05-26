using System;
using System.Collections.Generic;
using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace CardSystem
{
    public class UCardPilesManager : MonoBehaviour, ICombatStartListener
    {
        [Title("Player")]
        [SerializeField,HideInPlayMode]
        private PlayerHandPiles playerHand = new PlayerHandPiles();
        public SerializedPlayerHandPiles SerializedPlayerHands { get; private set; }

        [Title("Used Cards")]
        [SerializeField]
        private CardUsedPiles usedPiles = new CardUsedPiles();

        [Title("Draw Cards")]
        [SerializeField] 
        private UCardPile showDrawPile = null;

        private Dictionary<UCardHolder, IItemPile<UCardHolder>> _pileDictionary;
        public Dictionary<CombatSystemCharacter, IItemPile<UCardHolder>> characterPiles = null;

        private void HandleCard(UCardHolder card, IItemPile<UCardHolder> targetPile,
            PileAnimation.Type animationType)
        {
            if (_pileDictionary.ContainsKey(card))
            {
                IItemPile<UCardHolder> previousPile = _pileDictionary[card];
                previousPile.Remove(card);

                _pileDictionary[card] = targetPile;
            }
            else
            {
                _pileDictionary.Add(card, targetPile);
            }
            targetPile.Add(card, animationType);
        }

        private void HandleCard(UCardHolder card, IItemPile<UCardHolder> targetPile,
            bool animated = false)
        {
            var targetAnimation = PileAnimation.GetUpdateType(animated);

            HandleCard(card,targetPile,targetAnimation);
        }


        public void AddCardToShowPile(UCardHolder card, bool animated = false)
        {
            IItemPile<UCardHolder> targetPile = showDrawPile;
            HandleCard(card,targetPile,animated);
        }

        public void AddCardToCharactersPile(UCardHolder card, CombatSystemCharacter character, bool animated = false)
        {
            IItemPile<UCardHolder> targetPile = characterPiles[character];
            HandleCard(card,targetPile,animated);
        }

        public void AddCardToPreparation(UCardHolder card, CombatSystemCharacter character, 
            PileAnimation.Type animationType)
        {
            IItemPile<UCardHolder> targetPile = usedPiles.GetPreparationPile(character);
            HandleCard(card, targetPile, animationType);
        }

        public void TransferPreparedPileToPrevious()
        {
            usedPiles.TransferPreparedToPrevious();
        }


        private void Awake()
        {
            _pileDictionary = new Dictionary<UCardHolder, IItemPile<UCardHolder>>(128);

            CardCombatSystemSingleton.Instance.PlayerEntity.
                cardPilesManager = this;

            CardCombatSystemSingleton.Instance.Entity.
                AddOnStartListener(this);

            SerializedPlayerHands = new SerializedPlayerHandPiles(playerHand);
            playerHand = null;
        }

        public void DoStart(CombatCharactersHolder characters)
        {
            usedPiles.DoStart(characters);
            int amountOfCharacters = characters.GetAmountOfCharacters();
            characterPiles 
                = new Dictionary<CombatSystemCharacter, IItemPile<UCardHolder>>(amountOfCharacters);
            CombatSystemCharacter[] playersCharacters 
                = characters.PlayerCharactersInCombat.Characters;
            IItemPile<UCardHolder>[] playersPiles
                = SerializedPlayerHands.Characters;

            for (int i = 0; i < playersCharacters.Length; i++)
            {
                characterPiles.Add(playersCharacters[i],playersPiles[i]);
            }


        }
    }
    [Serializable]
    public class PlayerHandPiles : SerializablePlayerCharacters<UCardPile> { }

    public class SerializedPlayerHandPiles : SerializedPlayerCharacters<UCardPile>
    {
        public SerializedPlayerHandPiles(IPlayerCharacter<UCardPile> piles):
            base(piles) {}
    }


    [Serializable]
    public class CardUsedPiles : ICombatStartListener, IOnPreparedCharactersOrder
    {
        public Dictionary<CombatSystemCharacter, int> CharactersOrder { get; private set; }

        [Title("Params")]
        [SerializeField]
        private UTransformPile multiPileHolder = null;
        [SerializeField, HideInPlayMode]
        private UsedPile multiPileEntity;

        [Title("MultiPile"), NonSerialized, ShowInInspector, HideInEditorMode]
        public List<UsedPile> UsedPiles;

        public void DoStart(CombatCharactersHolder characters)
        {
            int amountOfCharacters = characters.GetAmountOfCharacters();
            UsedPiles = new List<UsedPile>(amountOfCharacters);

            for (int i = 0; i < amountOfCharacters; i++)
            {
                GameObject usedPileHolder = new GameObject("Used Pile < HOLDER >");
                UCardPile originalPreparationPile = multiPileEntity.preparationPile;
                UCardPile originalPreviousPile = multiPileEntity.previousTurnPile;

                UCardPile preparationPile
                    = Object.Instantiate(multiPileEntity.preparationPile, usedPileHolder.transform, true);
                preparationPile.transform.localPosition = originalPreparationPile.transform.localPosition;
                UCardPile previousPile
                    = Object.Instantiate(multiPileEntity.previousTurnPile, usedPileHolder.transform, true);
                previousPile.transform.localPosition = originalPreviousPile.transform.localPosition;

                UsedPiles.Add(new UsedPile(preparationPile, previousPile));
                multiPileHolder.Add(usedPileHolder.transform);
            }
            multiPileHolder.UpdatePositions();
        }

        public IItemPile<UCardHolder> GetPreparationPile(CombatSystemCharacter character)
        {
            int characterOrder = CharactersOrder[character];
            UsedPile targetUsedPile = UsedPiles[characterOrder];
            return targetUsedPile.preparationPile;
        }

        public void TransferPreparedToPrevious()
        {
            foreach (UsedPile pile in UsedPiles)
            {
                pile.preparationPile.TransferPileTo(pile.previousTurnPile);
            }
        }

        public void UpdateOrder(Dictionary<CombatSystemCharacter, int> charactersOrder)
        {
            CharactersOrder = charactersOrder;
        }

        [Serializable]
        public struct UsedPile
        {
            public UCardPile preparationPile;
            public UCardPile previousTurnPile;

            public UsedPile(UCardPile preparationPile, UCardPile previousPile)
            {
                this.preparationPile = preparationPile;
                this.previousTurnPile = previousPile;
            }
        }
    }

}

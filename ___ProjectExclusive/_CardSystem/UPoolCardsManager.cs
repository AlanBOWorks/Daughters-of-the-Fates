using System;
using System.Collections.Generic;
using CombatSystem;
using MEC;
using Sirenix.OdinInspector;
using SMaths;
using UnityEngine;

namespace CardSystem
{
    public class UPoolCardsManager : MonoBehaviour, ICombatStartListener, IPlayerDrawsListener
    {
        [Title("Piles")] 
        [SerializeField] private UCardPilesManager pilesManager = null;

        [Title("Prefabs")] 
        [SerializeField] private GameObject _cardPrefab = null;
        private Stack<UCardHolder> _instantiationPool = null;

        [Title("Params")] 
        [SerializeField] private RectTransform _drawCardPoint = null;


        private void Awake()
        {
            CardCombatSystemEntity entity
                = CardCombatSystemSingleton.Instance.Entity;
            PlayerCombatSystemEntity playerEntity =
                CardCombatSystemSingleton.Instance.PlayerEntity;

            playerEntity.poolCardsManager = this;
            entity.AddOnStartListener(this);
            _instantiationPool = new Stack<UCardHolder>(32);
        }

        private void OnDestroy()
        {
            CardCombatSystemSingleton.Instance.Entity.RemoveOnStatListener(this);
            foreach (UCardHolder cardHolder in _instantiationPool)
            {
                Destroy(cardHolder);
            }

        }

        private int CardAmountPrediction = 20;
        public void DoStart(CombatCharactersHolder characters)
        {
            CardCombatSystemEntity entity
                = CardCombatSystemSingleton.Instance.Entity;

            entity.AddOnDrawListener(this);
            InstantiatePredictedCards();
            _firstsDrawAmount = 0;


            void InstantiatePredictedCards()
            {
                for (int i = 0; i < CardAmountPrediction; i++)
                {
                    InstantiateCard();
                }

            }
        }

        private void InstantiateCard()
        {
            GameObject cardGO = GameObject.Instantiate(
                _cardPrefab, _drawCardPoint);
            UCardHolder cardHolder = cardGO.GetComponent<UCardHolder>();
            _instantiationPool.Push(cardHolder);
        }

        private UCardHolder PoolCard()
        {
            if (_instantiationPool.Count <= 0)
            {
                InstantiateCard();
            }

            return _instantiationPool.Pop();
        }

        public void ReturnCardToPool(UCardHolder card)
        {
            _instantiationPool.Push(card);
        }

        [SuffixLabel("Seconds")]
        public float DrawCardFrequency = .1f;
        public float PutHandFrequency = .2f;
        [SuffixLabel("Seconds")]
        public SRange ShowDraw = new SRange(.6f,1.6f);

        private const int FirstDrawCharactersAmount = 2;
        private int _firstsDrawAmount;
        public void OnDrawCards(CombatSystemCharacter character, Queue<ICardData> drawQueue)
        {
            
            if ( _firstsDrawAmount < FirstDrawCharactersAmount)
            {
                _firstsDrawAmount ++;
                InitialDraw();
                return;
            }

            Timing.RunCoroutine(_DoDraws());
            IEnumerator<float> _DoDraws()
            {
                int cardsCount = drawQueue.Count;
                Queue<UCardHolder> cardHolders = new Queue<UCardHolder>(cardsCount);
                //Draw and show to the player
                foreach (ICardData cardData in drawQueue)
                {
                    yield return Timing.WaitForSeconds(DrawCardFrequency);
                    UCardHolder cardHolder = PoolCard();
                    cardHolder.InjectCard(character, cardData, true);
                    cardHolders.Enqueue(cardHolder);
                    pilesManager.AddCardToShowPile(cardHolder,true);
                }

                yield return Timing.WaitForSeconds(ShowDraw.MaxClampAddition(DrawCardFrequency * cardsCount));

                //Puts in the Character's hand
                foreach (UCardHolder cardHolder in cardHolders)
                {
                    yield return Timing.WaitForSeconds(PutHandFrequency);
                    pilesManager.AddCardToCharactersPile(cardHolder,character,true);
                }
            }

            void InitialDraw()
            {

                foreach (ICardData cardData in drawQueue)
                {
                    UCardHolder cardHolder = PoolCard();
                    cardHolder.InjectCard(character,cardData,true);
                    pilesManager.AddCardToCharactersPile(cardHolder,character);
                }
            }


        }

        [Serializable]
        private class SerializedPlayerHand : SerializablePlayerCharacters<UCardPile>
        {}
    }
}

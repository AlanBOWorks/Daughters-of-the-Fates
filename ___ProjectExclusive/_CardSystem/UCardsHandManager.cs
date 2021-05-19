using System;
using System.Collections.Generic;
using CombatSystem;
using MEC;
using Sirenix.OdinInspector;
using SMaths;
using UnityEngine;

namespace CardSystem
{
    public class UCardsHandManager : MonoBehaviour, ICombatStartListener, IPlayerDrawsListener
    {
        [Title("Piles")]
        [SerializeField] private UCardPileBase _drawPile = null;

        [SerializeField,HideInPlayMode] 
        private SerializedPlayerHand _playerHandHolders = new SerializedPlayerHand();
        [ShowInInspector,HideInEditorMode]
        public SerializedPlayerCharacters<UCardPileBase> SerializedPlayerHands { get; private set; }

        [Title("Prefabs")] 
        [SerializeField] private GameObject _cardPrefab = null;
        private Stack<UCardHolder> _instantiationPool = null;

        [Title("Params")] 
        [SerializeField] private RectTransform _drawCardPoint = null;

        private Dictionary<CombatSystemCharacter, ICardsPile> _charactersHands = null;

        private void Awake()
        {
            CardCombatSystemEntity entity
                = CardCombatSystemSingleton.Instance.Entity;

            SerializedPlayerHands = new SerializedPlayerCharacters<UCardPileBase>(_playerHandHolders);
            _playerHandHolders = null;


            entity.CardsHandManager = this;
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
            InstantiateDictionaries();
            InstantiatePredictedCards();
            _firstsDrawAmount = 0;

            void InstantiateDictionaries()
            {
                int charactersAmount = characters.GetAmountOfPlayersCharacters();
                _charactersHands = new Dictionary<CombatSystemCharacter, ICardsPile>(charactersAmount);
                for (var i = 0; i < charactersAmount; i++)
                {
                    CombatSystemCharacter character = characters.PlayerCharactersInCombat.Characters[i];
                    ICardsPile hand = SerializedPlayerHands.Characters[i];
                    _charactersHands.Add(character,hand);
                }
            }

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

        private const int FirstDrawAmount = 2;
        private int _firstsDrawAmount;
        public void OnDrawCards(CombatSystemCharacter character, Queue<ICardData> cards)
        {
            if (_firstsDrawAmount < FirstDrawAmount)
            {
                _firstsDrawAmount ++;
                InitialDraw();
                return;
            }

            Timing.RunCoroutine(_DoDraws());
            IEnumerator<float> _DoDraws()
            {
                int cardsCount = cards.Count;
                Queue<UCardHolder> cardHolders = new Queue<UCardHolder>(cardsCount);
                //Draw and show to the player
                foreach (ICardData cardData in cards)
                {
                    yield return Timing.WaitForSeconds(DrawCardFrequency);
                    UCardHolder cardHolder = PoolCard();
                    cardHolder.InjectCard(cardData, true);
                    _drawPile.Add(cardHolder.gameObject);
                    cardHolders.Enqueue(cardHolder);
                    _drawPile.UpdatePositions();

                }

                yield return Timing.WaitForSeconds(ShowDraw.MaxClampAddition(DrawCardFrequency * cardsCount));

                //Puts in the Character's hand
                ICardsPile hand = _charactersHands[character];
                foreach (UCardHolder cardHolder in cardHolders)
                {
                    _drawPile.Remove(cardHolder.gameObject);
                    yield return Timing.WaitForSeconds(PutHandFrequency);
                    hand.Add(cardHolder.gameObject);
                    hand.UpdatePositions();
                }
            }

            void InitialDraw()
            {
                ICardsPile hand = _charactersHands[character];

                foreach (ICardData cardData in cards)
                {
                    UCardHolder cardHolder = PoolCard();
                    cardHolder.InjectCard(cardData,true);
                    hand.Add(cardHolder.gameObject);
                    hand.UpdatePositions(false);
                }
            }


        }

        [Serializable]
        private class SerializedPlayerHand : SerializablePlayerCharacters<UCardPileBase>
        {}
    }
}

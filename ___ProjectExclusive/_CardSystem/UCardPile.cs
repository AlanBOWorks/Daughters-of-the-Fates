using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace CardSystem
{
	/// <summary>
	/// <inheritdoc cref="CardsPile"/>
	/// </summary>
    public class UCardPile : UCardPileBase
    {
		[Title("Card related")]
		[Range(-3,3)]
        public float WidthPercentage = 1f;
        [Range(-3, 3)] 
        public float HeightPercentage = 0f;

		[Range(0f, 90f)] public float maxCardAngle = 5f;
        public float yOffsetPerCard = 0;

        public float moveDuration = 0.5f;

		public event Action<int> OnCountChanged;


		public override void Add(GameObject card, int index = -1)
		{
            if (index < 0)
			{
				Cards.Add(card);
			}
			else
			{
				Cards.Insert(index, card);
			}

            OnCountChanged?.Invoke(Cards.Count);
		}

		public override void Remove(GameObject card)
		{
			if (!Cards.Contains(card))
				return;

            Cards.Remove(card);
			card.transform.DOKill();

			OnCountChanged?.Invoke(Cards.Count);
		}

		public override void RemoveAt(int index)
		{
			Remove(Cards[index]);
		}

		public override void RemoveAll()
		{
			while (Cards.Count > 0)
				Remove(Cards[0]);
		}

		//Uses the algorithm from CardsPile (cleanup a little bit)
        public override void UpdatePositions(bool animatePositions = true)
        {
            List<GameObject> cards = base.Cards;
			if(cards is null) return;
            int cardsCount = cards.Count;

			RectTransform rectTransform = GetComponent<RectTransform>();
            Rect rect = rectTransform.rect;
            float height = rect.height * HeightPercentage;
            float width = rect.width * WidthPercentage;

			float radius = Mathf.Abs(height) < 0.001f
				? width * width / 0.001f * Mathf.Sign(height)
				: height / 2f + width * width / (8f * height);

			float angle = 2f * Mathf.Asin(0.5f * width / radius) * Mathf.Rad2Deg;
			angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxCardAngle * (cards.Count - 1));
			float cardAngle = cards.Count == 1 ? 0f : angle / (cards.Count - 1f);

			for (int index = 0; index < cards.Count; index++)
            {
                Transform card = cards[index].transform;
                card.SetParent(rectTransform, true);

                float wideAngle = angle / 2f - cardAngle * index;

                Quaternion targetLocalRotation 
                    = Quaternion.Euler(0, 0, wideAngle);
                Vector3 targetLocalPosition 
                    = new Vector3(0f, radius, 0f);
                targetLocalPosition = targetLocalRotation * targetLocalPosition;
                targetLocalPosition.y += height - radius;
                targetLocalPosition.y +=  yOffsetPerCard * index;
                
               


                card.DOKill();
                if (animatePositions)
                {
                    card.DOLocalMove(targetLocalPosition, moveDuration);
                    card.DOLocalRotate(targetLocalRotation.eulerAngles, moveDuration);
                }
                else
                {
                    card.localPosition = targetLocalPosition;
                    card.localRotation = targetLocalRotation;
                }

            }
		}


		void OnValidate()
		{
			UpdatePositions();
		}
	}


}

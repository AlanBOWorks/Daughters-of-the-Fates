using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CardSystem
{
    /// <summary>
    /// <inheritdoc cref="CardsPile"/>
    /// </summary>
    public abstract class UPileBase<T> : MonoBehaviour, IItemPile<T>
    {
        [ShowInInspector,DisableInPlayMode,HideInEditorMode]
        private List<T> _items = null;
        public List<T> Items => _items;

        private const int DefaultCardsLength = 16;
        private void Awake()
        {
            _items = new List<T>(DefaultCardsLength);
        }


        public abstract void Add(T item, PileAnimation.Type animationType = PileAnimation.Type.None, int index = -1);
        public abstract void Remove(T item);
        public abstract void RemoveAt(int index);
        public abstract void RemoveAll();
        public abstract void UpdatePositions(bool animatePositions = true);


        public void TransferPileTo(IItemPile<T> toPile, PileAnimation.Type animationType = PileAnimation.Type.None)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                T item = _items[i];
                _items.Remove(item);
                toPile.Add(item,animationType);
            }
        }
    }

    public static class PileAnimation
    {
        public enum Type
        {
            None,
            Animated,
            DirectTransform
        }

        /// <summary>
        /// Implied use for <seealso cref="UPileBase{T}.UpdatePositions"/> through
        /// <seealso cref="UPileBase{T}.Add"/> 
        /// </summary>
        public static Type GetUpdateType(bool isAnimated)
        {
            Type targetAnimation = isAnimated
                ? Type.Animated
                : Type.DirectTransform;
            return targetAnimation;
        }
    }

    public abstract class UPile<T> : UPileBase<T> where T : Component
    {
        [Title("Card related")]
        [Range(-3, 3)]
        public float WidthPercentage = 1f;
        [Range(-3, 3)]
        public float HeightPercentage = 0f;

        [Range(0f, 90f)] public float maxCardAngle = 5f;
        public float yOffsetPerCard = 0;

        public float moveDuration = 0.5f;

        public event Action<int> OnCountChanged;


        public override void Add(T item, PileAnimation.Type animationType = PileAnimation.Type.None, int index = -1)
        {
            if (index < 0)
            {
                Items.Add(item);
            }
            else
            {
                Items.Insert(index, item);
            }

            OnCountChanged?.Invoke(Items.Count);

            switch (animationType)
            {
                default:
                    break;
                case PileAnimation.Type.Animated:
                    UpdatePositions(true);
                    break;
                case PileAnimation.Type.DirectTransform:
                    UpdatePositions(false);
                    break;
            }
        }

        public override void Remove(T item)
        {
            if (!Items.Contains(item))
                return;

            Items.Remove(item);
            item.transform.DOKill();

            OnCountChanged?.Invoke(Items.Count);
        }

        public override void RemoveAt(int index)
        {
            Remove(Items[index]);
        }

        public override void RemoveAll()
        {
            while (Items.Count > 0)
                Remove(Items[0]);
        }

        //Uses the algorithm from CardsPile (cleanup a little bit)
        public override void UpdatePositions(bool animatePositions = true)
        {
            List<T> items = base.Items;
            if (items is null) return;
            int cardsCount = items.Count;

            RectTransform rectTransform = GetComponent<RectTransform>();
            Rect rect = rectTransform.rect;
            float height = rect.height * HeightPercentage;
            float width = rect.width * WidthPercentage;

            float radius = Mathf.Abs(height) < 0.001f
                ? width * width / 0.001f * Mathf.Sign(height)
                : height / 2f + width * width / (8f * height);

            float angle = 2f * Mathf.Asin(0.5f * width / radius) * Mathf.Rad2Deg;
            angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxCardAngle * (items.Count - 1));
            float cardAngle = items.Count == 1 ? 0f : angle / (items.Count - 1f);

            for (int index = 0; index < items.Count; index++)
            {
                Transform itemTransform = items[index].transform;
                itemTransform.SetParent(rectTransform, true);

                float wideAngle = angle / 2f - cardAngle * index;

                Quaternion targetLocalRotation
                    = Quaternion.Euler(0, 0, wideAngle);
                Vector3 targetLocalPosition
                    = new Vector3(0f, radius, 0f);
                targetLocalPosition = targetLocalRotation * targetLocalPosition;
                targetLocalPosition.y += height - radius;
                targetLocalPosition.y += yOffsetPerCard * index;




                itemTransform.DOKill();
                if (animatePositions)
                {
                    itemTransform.DOLocalMove(targetLocalPosition, moveDuration);
                    itemTransform.DOLocalRotate(targetLocalRotation.eulerAngles, moveDuration);
                }
                else
                {
                    itemTransform.localPosition = targetLocalPosition;
                    itemTransform.localRotation = targetLocalRotation;
                }

            }
        }


        void OnValidate()
        {
            UpdatePositions();
        }
    }

}

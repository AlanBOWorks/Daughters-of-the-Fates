using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class UCardTargetSelector : MonoBehaviour, IPointerClickHandler
    {
        private UCardSelectorsManager _handler;
        [ShowInInspector,DisableInEditorMode,DisableInPlayMode]
        protected CombatSystemCharacter SelectingCharacter { get; private set; }
        public void Injection(CombatSystemCharacter selectionCharacter) => SelectingCharacter = selectionCharacter;
        public void Injection(UCardSelectorsManager handler) => _handler = handler;


        public bool GameObjectEnabled
        {
            set => gameObject.SetActive(value);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //TODO do animation of click
            DoPushCard();
        }

        [Button,HideInEditorMode]
        private void DoPushCard()
        {
            GameObjectEnabled = false;
            _handler.PushCard(SelectingCharacter);
        }
    }
}

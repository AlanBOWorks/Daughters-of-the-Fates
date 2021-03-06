using System;
using CombatSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class CardsStatesManager
    {
        private readonly ICardStateHandler[] _handlers;

        //This states can switch between each other, but inactive can only be switched by other meanings
        public ICardStateHandler IdleState => _handlers[IdleHandlerIndex];
        public ICardStateHandler TargetState => _handlers[TargetHandlerIndex];
        public ICardStateHandler SelectedState => _handlers[SelectedHandlerIndex];

        public CardsStatesManager()
        {
            var inactiveState = new CardStateBase(this);
            var idleState = new IdleCardState(this);
            var targetModeState = new TargetModeCardState(this);
            var selectedState = new SelectedCardStateHandler(this);

            _handlers = new ICardStateHandler[]
            {
                inactiveState,
                idleState,
                targetModeState,
                selectedState
            };

        }


        private const int InactiveHandlerIndex = 0;
        private const int IdleHandlerIndex = 1;
        private const int TargetHandlerIndex = 2;
        private const int SelectedHandlerIndex = 3;
        public enum HandlerStates
        {
            Inactive = InactiveHandlerIndex,
            Idle = IdleHandlerIndex,
            Target = TargetHandlerIndex,
            Selected = SelectedHandlerIndex
        }

        public ICardStateHandler GetState(HandlerStates targetStates)
        {
            return _handlers[(int)targetStates];
        }

        public static ICardStateHandler GetStateFromSingleton(HandlerStates targetStates)
        {
            CardsStatesManager manager = CardCombatSystemSingleton.Instance.PlayerEntity.cardsStatesManager;
            return manager.GetState(targetStates);
        }
    }

    public interface ICardStateHandler
    {
        void OnSwitchState(UCardHolder onHolder);
        void OnClick(UCardHolder onHolder);
        void OnCancel(UCardHolder onHolder);

        void OnSubmit(UCardHolder onHolder);

        void OnPointerEnter(UCardHolder onHolder);
        void OnPointerExit(UCardHolder onHolder);
    }

    public interface ICardStateHandlerMono : ISubmitHandler, ICancelHandler,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    { }


    public class CardStateBase : ICardStateHandler
    {

        protected readonly CardsStatesManager manager;
        public CardStateBase(CardsStatesManager manager)
        {
            this.manager = manager;
        }

        public virtual void OnSwitchState(UCardHolder onHolder)
        {}

        public virtual void OnPointerEnter(UCardHolder onHolder)
        {}

        public virtual void OnPointerExit(UCardHolder onHolder)
        {}

        public virtual void OnClick(UCardHolder onHolder)
        {}

        public virtual void OnCancel(UCardHolder onHolder)
        {}

        public virtual void OnSubmit(UCardHolder onHolder) 
        {}


    }

    public class IdleCardState : CardStateBase
    {
        public IdleCardState(CardsStatesManager manager) : base(manager)
        {
        }

    }

    public class TargetModeCardState : CardStateBase
    {
        public override void OnSwitchState(UCardHolder onHolder)
        {
            ICardTargetHandler targetHandler =
                CardCombatSystemSingleton.Instance.PlayerEntity.cardSelectorsManager;
            targetHandler.EnableSelectors(onHolder.User, onHolder, this);
        }

        public override void OnClick(UCardHolder onHolder)
        {
            OnCancel(onHolder);
        }

        public override void OnCancel(UCardHolder onHolder)
        {
            UCardSelectorsManager selectorsManager =
                CardCombatSystemSingleton.Instance.PlayerEntity.cardSelectorsManager;
            selectorsManager.RemoveSelected();
        }

        public TargetModeCardState(CardsStatesManager manager) : base(manager)
        {
        }
    }

    public class SelectedCardStateHandler : CardStateBase
    {
        public override void OnClick(UCardHolder onHolder)
        {
            OnCancel(onHolder);
        }

        public SelectedCardStateHandler(CardsStatesManager manager) : base(manager)
        {
        }
    }

}

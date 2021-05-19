using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace StylizedAnimator
{
    public class UStylizedManagerInvoker : MonoBehaviour, IStylizedManagerHolder
    {
        [SerializeField]
        private TickManagerParameters _parameters
            = new TickManagerParameters();
        private StylizedTickManager _manager;

        public TickerManagerEntity Entity = TickManagerSingleton.Instance.Entity;

        private void Awake()
        {
            _manager = new StylizedTickManager(_parameters,this);
            Entity.MainManager = _manager;

        }

        private void Start()
        {
            _manager = TickManagerSingleton.GetTickManager();
            _manager.StartTicks();
        }


        private void OnEnable()
        {
            _manager?.OnEnable();
        }

        private void OnDisable()
        {
            _manager?.OnDisable();
        }

        public StylizedTickManager GetManager()
        {
            return _manager;
        }

    }


}

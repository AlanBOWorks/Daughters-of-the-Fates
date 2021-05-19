using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StylizedAnimator
{
    // It uses Singleton because there's only one manager of this type per Game
    public sealed class TickManagerSingleton 
    {
        static TickManagerSingleton() { }
        private TickManagerSingleton() { }
        public static TickManagerSingleton Instance { get; } = new TickManagerSingleton();

        [SerializeField, HideInEditorMode, HideInPlayMode, HideInInlineEditors, HideDuplicateReferenceBox]
        public TickerManagerEntity Entity = new TickerManagerEntity();

        public static StylizedTickManager GetTickManager() => Instance.Entity.MainManager;
    }

    [Serializable]
    public class TickerManagerEntity
    {
        [ShowInInspector,DisableInEditorMode]
        public StylizedTickManager MainManager = null;
    }
}

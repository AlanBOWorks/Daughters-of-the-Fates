using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Player
{
    public sealed class PlayerEntitySingleton
    {
        static PlayerEntitySingleton() { }
        private PlayerEntitySingleton() { }
        public static PlayerEntitySingleton Instance { get; } = new PlayerEntitySingleton();

        [SerializeField, HideInEditorMode, HideInPlayMode, HideInInlineEditors, HideDuplicateReferenceBox]
        public PlayerEntity Entity = new PlayerEntity();

        public static bool HasEnoughCharacters()
        {
            return Instance.Entity.ControllingCharacters.Count >= 2;
        }
    }

    [Serializable]
    public class PlayerEntity
    {
        public List<PlayerCharacterEntityVariable> ControllingCharacters = new List<PlayerCharacterEntityVariable>(2);
    }
}

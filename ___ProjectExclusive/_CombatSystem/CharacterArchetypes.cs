using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{

    public interface IPlayerCharacter<out T>
    {
        T Vanguard { get; }
        T Support { get; }
    }

    public static class PlayerCharacters
    {
        public const int VanguardIndex = 0;
        public const int SupportIndex = VanguardIndex + 1;
        public const int CharactersAmount = SupportIndex + 1;
    }

    [Serializable]
    public class SerializablePlayerCharacters<T> : IPlayerCharacter<T>
    {
        [SerializeField] private T _vanguard;
        public T Vanguard => _vanguard;

        [SerializeField] private T _support;
        public T Support => _support;

    }

    public class SerializedPlayerCharacters<T> : IPlayerCharacter<T>
    {
        public readonly T[] Characters;

        public const int VanguardIndex = PlayerCharacters.VanguardIndex;
        public const int SupportIndex = PlayerCharacters.SupportIndex;
        public const int CharactersAmount = PlayerCharacters.CharactersAmount;

        public int GetCharacterAmount() => CharactersAmount;


        public SerializedPlayerCharacters()
        {
            Characters = new T[CharactersAmount];
        }
        public SerializedPlayerCharacters(T vanguard, T support)
        {
            Characters = new[]
            {
                vanguard,
                support
            };
        }

        public SerializedPlayerCharacters(List<T> characters)
        {
            if (characters.Count > CharactersAmount) return;
            Characters = characters.ToArray();
        }

        public SerializedPlayerCharacters(IPlayerCharacter<T> charactersWrapper)
        : this(charactersWrapper.Vanguard, charactersWrapper.Support)
        { }

        public T Vanguard
        {
            get => Characters[VanguardIndex];
            set => Characters[VanguardIndex] = value;
        }
        public T Support
        {
            get => Characters[SupportIndex];
            set => Characters[SupportIndex] = value;
        }
    }

    public class ControllableArchetypesBase
    {
        public enum ControllableArchetypes
        {
            Player = PlayerIndex,
            Enemy = EnemyIndex,
        }

        public static T GetValue<T>(IControllableArchetype<T> archetype, ControllableArchetypes type)
        {
            switch (type)
            {
                default:
                    return archetype.Player;
                case ControllableArchetypes.Enemy:
                    return archetype.Enemy;
            }
        }

        public const int PlayerIndex = 0;
        public const int EnemyIndex = PlayerIndex + 1;
        public const int ControllableCount = EnemyIndex + 1;
    }

    public interface IControllableArchetype<out T>
    {
        T Player { get; }
        T Enemy { get; }

        T GetArchetypeValue(ControllableArchetypesBase.ControllableArchetypes type);
    }

    public class SerializableControllableArchetypes<T> : ControllableArchetypesBase, IControllableArchetype<T>
    {
        [SerializeField] private T _player;
        [SerializeField] private T _enemy;
        public T Player => _player;
        public T Enemy => _enemy;

        public T GetArchetypeValue(ControllableArchetypes type)
        {
            return GetValue(this, type);
        }
    }

    public class ControllableArchetypes<T> : ControllableArchetypesBase, IControllableArchetype<T>
    {
        public T Player => SerializedArchetypes[PlayerIndex];
        public T Enemy => SerializedArchetypes[EnemyIndex];
        public T[] SerializedArchetypes { get; }

        public ControllableArchetypes(IControllableArchetype<T> archetypes)
        {
            SerializedArchetypes = new[]
            {
                archetypes.Player,
                archetypes.Enemy
            };
        }

        public T GetArchetypeValue(ControllableArchetypes type)
        {
            return GetValue(this, type);
        }
    }
}

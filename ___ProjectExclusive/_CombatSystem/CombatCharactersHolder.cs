using System;
using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using _Player;
using CardSystem;
using Sirenix.OdinInspector;
using TurnSystem;
using UnityEngine;

namespace CombatSystem
{
    public class CombatCharactersHolder 
    {
        public readonly List<CombatSystemCharacter> ListCharactersInCombat;

        [HideInInspector]
        public readonly SerializedPlayerCharacters<CombatSystemCharacter> PlayerCharactersInCombat;
        [ShowInInspector] 
        public readonly CharacterTeam PlayerTeam;

        [ShowInInspector] 
        public readonly CharacterTeam EnemyTeam;
        [ShowInInspector] 
        public readonly List<CombatSystemCharacter> ListEnemiesInCombat;

        public int GetAmountOfCharacters() => (ListCharactersInCombat.Count);
        public int GetAmountOfPlayersCharacters() => PlayerCharactersInCombat.GetCharacterAmount();

        public CombatCharactersHolder
            (List<PlayerCharacterEntityVariable> playerCharacters,
            List<EnemyEntityVariable> enemies)
        {
            int playerCharactersLength = playerCharacters.Count;
            ListCharactersInCombat = new List<CombatSystemCharacter>(playerCharactersLength + enemies.Capacity);
            ListEnemiesInCombat = new List<CombatSystemCharacter>(enemies.Capacity);
            List<CombatSystemCharacter> listPlayerCharactersInCombat = new List<CombatSystemCharacter>(playerCharactersLength);

            PlayerTeam = new CharacterTeam(listPlayerCharactersInCombat);
            EnemyTeam = new CharacterTeam(ListEnemiesInCombat);
            PlayerTeam.EnemyTeam = EnemyTeam;
            EnemyTeam.EnemyTeam = PlayerTeam;

            PlayerCharactersInCombat
                = new SerializedPlayerCharacters<CombatSystemCharacter>();

            for (var i = 0; i < playerCharactersLength; i++)
            {
                PlayerCharacterEntityVariable character = playerCharacters[i];
                CombatSystemCharacter playerCharacter = new CombatSystemCharacter(character.Data);
                ListCharactersInCombat.Add(playerCharacter);
                listPlayerCharactersInCombat.Add(playerCharacter);

                PlayerCharactersInCombat.Characters[i] = playerCharacter;

                playerCharacter.InjectTeam(PlayerTeam);
                playerCharacter.InjectEnemies(EnemyTeam);
            }


            for (var i = 0; i < enemies.Count; i++)
            {
                EnemyEntityVariable character = enemies[i];
                CombatSystemCharacter enemy = new CombatSystemCharacter(character.Data);
                ListCharactersInCombat.Add(enemy);
                ListEnemiesInCombat.Add(enemy);

                enemy.InjectEnemies(EnemyTeam);
                enemy.InjectTeam(PlayerTeam);
            }

        }

    }

    public class CombatSystemCharacter
    {
        public readonly CharacterCombatStatsHolder Stats;
        public readonly CombatDeck Deck;
        [ShowInInspector]
        public readonly CardsHand Hand;
        public readonly ITurnCombatAnimator CharacterAnimator;

        /// <summary>
        /// This is a list of one element of this only <see cref="CombatSystemCharacter"/>; It's
        /// used for selectors that only can target them self without any other character
        /// </summary>
        public readonly List<CombatSystemCharacter> SelfAgent;
        public readonly List<CombatSystemCharacter> Allies;

        [ShowInInspector,DisableInPlayMode]
        public CharacterTeam Team { get; private set; }
        [ShowInInspector,DisableInPlayMode]
        public CharacterTeam EnemyTeam { get; private set; }

        public void InjectTeam(CharacterTeam allies)
        {
            Team = allies;
            foreach (CombatSystemCharacter character in allies.Members)
            {
                if(character != this)
                    Allies.Add(character);
            }
        }
        public void InjectEnemies(CharacterTeam enemies) => EnemyTeam = enemies;

        private const int MaxAmountOfAllies = 4;
        public CombatSystemCharacter(CharacterEntity character)
        {
            SelfAgent = new List<CombatSystemCharacter>(1) {this};
            Allies = new List<CombatSystemCharacter>(MaxAmountOfAllies);

            Stats = character.Variable.GenerateCombatStats();
            Deck = new CombatDeck(character.Variable.GetDeck().GenerateDeck());
            Hand = new CardsHand(this,Deck,Stats.MainStats);
            CharacterAnimator = character.CombatAnimator;
        }
    }

}

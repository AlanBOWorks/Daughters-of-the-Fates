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
        public readonly CharacterCombatStatsHolder CharacterCombatStats;
        public readonly CombatDeck Deck;
        [ShowInInspector]
        public readonly CardsHand Hand;
        public readonly ITurnCombatAnimator CharacterAnimator;

        [ShowInInspector,DisableInPlayMode]
        public CharacterTeam Team { get; private set; }
        [ShowInInspector,DisableInPlayMode]
        public CharacterTeam EnemyTeam { get; private set; }

        public void InjectTeam(CharacterTeam allies) => Team = allies;
        public void InjectEnemies(CharacterTeam enemies) => EnemyTeam = enemies;

        public CombatSystemCharacter(CharacterEntity character)
        {
            CharacterCombatStats = character.Variable.GenerateCombatStats();
            Deck = new CombatDeck(character.Variable.GetDeck().GenerateDeck());
            Hand = new CardsHand(this,Deck,CharacterCombatStats.MainStats);
            CharacterAnimator = character.CombatAnimator;
        }
    }

}

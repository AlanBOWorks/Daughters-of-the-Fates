using System.Collections.Generic;
using ___ProjectExclusive.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "Combat Declaration - N [Preset]",
        menuName = "Preset/Combat Declaration")]
    public class SCombatEnemiesPreset : ScriptableObject
    {
        [SerializeField]
        private List<EnemyEntityVariable> _enemies = new List<EnemyEntityVariable>(1);

        public List<EnemyEntityVariable> GetEnemies() => _enemies;

    }
}

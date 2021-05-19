using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "Combat System Invoker [Singleton]",
        menuName = "Singleton/Combat System Invoker")]
    public class SCombatSystemInvokerSingleton : ScriptableObject
    {
        [SerializeField] 
        private CardCombatSystemEntity _combatSystem = CardCombatSystemSingleton.Instance.Entity;
    }
}

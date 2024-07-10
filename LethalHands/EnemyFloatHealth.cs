using System.Collections.Generic;
using System.Reflection;

namespace LethalHands
{
    public class EnemyFloatHealth
    {
        public static EnemyFloatHealth Instance;
        private Dictionary<EnemyAI, float> enemyHitCounter;

        public EnemyFloatHealth()
        {
            if(Instance == null) Instance = this;
            enemyHitCounter = new Dictionary<EnemyAI, float>();
        }

        public void Reset()
        {
            enemyHitCounter.Clear();
        }

        public int DamageEnemy(EnemyAI enemy)
        {
            if (!enemyHitCounter.ContainsKey(enemy)) enemyHitCounter.Add(enemy, 0);
            int damage = 0;
            if(enemy is NutcrackerEnemyAI) // Nutcracker vulnerability check
            {
                NutcrackerEnemyAI nutcracker = enemy as NutcrackerEnemyAI;
                FieldInfo isInspectingAttribute = typeof(NutcrackerEnemyAI).GetField("isInspecting", BindingFlags.NonPublic | BindingFlags.Instance);
                if(!(bool)isInspectingAttribute.GetValue(nutcracker) && nutcracker.currentBehaviourStateIndex != 2) return 0;
            }
            enemyHitCounter[enemy] += LethalHands.Instance.enemyPunchDamage;
            while (enemyHitCounter[enemy] >= 1f)
            {
                enemyHitCounter[enemy]--;
                damage++;
            }
            LethalHandsPlugin.Instance.manualLogSource.LogInfo($"new damage : {damage}");
            return damage;
        }
    }
}

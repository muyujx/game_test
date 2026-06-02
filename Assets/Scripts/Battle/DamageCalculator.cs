using UnityEngine;

namespace PokemonBattle.Battle
{
    /// <summary>
    /// 伤害计算工具类
    /// 实现经典宝可梦伤害公式 (简化版)
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// 计算技能伤害
        /// 公式：伤害 = ((((2 * Level / 5 + 2) * Power * A / D) / 50) + 2) * 修正
        /// </summary>
        public static int CalculateDamage(
            BattleCreature attacker,
            BattleCreature defender,
            SkillData skill,
            int moveIndex = 0)
        {
            // 变化技能不造成伤害
            if (skill.category == SkillCategory.Status)
                return 0;
            
            // 基础伤害计算
            int level = attacker.level;
            int power = skill.power;
            
            // 确定使用攻击方和防御方的能力值
            float attackStat = skill.category == SkillCategory.Physical 
                ? attacker.attack 
                : attacker.spAtk;
                
            float defenseStat = skill.category == SkillCategory.Physical 
                ? defender.defense 
                : defender.spDef;
            
            // 应用能力阶段修正
            int atkStage = skill.category == SkillCategory.Physical 
                ? attacker.statStageAtk 
                : attacker.statStageSpAtk;
                
            int defStage = skill.category == SkillCategory.Physical 
                ? defender.statStageDef 
                : defender.statStageSpDef;
            
            attackStat *= attacker.GetStatStageMultiplier(atkStage);
            defenseStat *= defender.GetStatStageMultiplier(defStage);
            
            // 基础公式
            float baseDamage = ((((2f * level / 5f + 2) * power * attackStat / defenseStat) / 50) + 2);
            
            // 修正系数
            float modifier = 1.0f;
            
            // 属性克制 (最重要)
            float typeEffectiveness = defender.GetTypeEffectiveness(skill.skillType);
            modifier *= typeEffectiveness;
            
            // 本系加成 (STAB - Same Type Attack Bonus)
            if (attacker.data.primaryType == skill.skillType || 
                attacker.data.secondaryType == skill.skillType)
            {
                modifier *= 1.5f;
            }
            
            // 会心一击 (2 倍，简化处理：随机 6.25% 概率)
            bool isCritical = Random.value < 0.0625f;
            if (isCritical)
            {
                modifier *= 1.5f; // Gen6+ 会心倍率
                Debug.Log("会心一击!");
            }
            
            // 随机波动 (0.85 ~ 1.00)
            float random = Random.Range(0.85f, 1.0f);
            modifier *= random;
            
            // 状态异常修正 (灼伤物理攻击减半)
            if (attacker.status == StatusCondition.Burn && skill.category == SkillCategory.Physical)
            {
                modifier *= 0.5f;
            }
            
            // 最终伤害
            int finalDamage = Mathf.Max(1, Mathf.FloorToInt(baseDamage * modifier));
            
            // 输出战斗日志
            string effectivenessText = typeEffectiveness switch
            {
                > 1.5f => "效果绝佳!",
                > 1.0f => "效果很好",
                < 0.5f and > 0.0f => "效果不好...",
                0.0f => "没有效果...",
                _ => ""
            };
            
            if (!string.IsNullOrEmpty(effectivenessText))
            {
                Debug.Log(effectivenessText);
            }
            
            return finalDamage;
        }
        
        /// <summary>
        /// 判断技能是否命中
        /// </summary>
        public static bool CheckAccuracy(
            BattleCreature attacker,
            BattleCreature defender,
            SkillData skill)
        {
            // 必中技能
            if (skill.accuracy == 0) // 0 表示必中
                return true;
            
            // 命中率和闪避率阶段修正
            int accuracyStage = attacker.statStageAccuracy - defender.statStageEvasion;
            float accuracyMultiplier = attacker.GetStatStageMultiplier(accuracyStage);
            
            // 实际命中率
            float actualAccuracy = skill.accuracy * accuracyMultiplier / 100f;
            
            return Random.value < actualAccuracy;
        }
    }
}

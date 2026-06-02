using UnityEngine;

namespace PokemonBattle.Battle
{
    /// <summary>
    /// 战斗中的生物实例
    /// 包含等级、个体值、努力值、当前状态等运行时数据
    /// </summary>
    public class BattleCreature
    {
        // 基础数据引用
        public CreatureData data;
        
        // 个体参数
        public int level = 50;
        public int[] ivs = new int[6]; // HP, Atk, Def, SpA, SpD, Spe
        public int[] evs = new int[6];
        public float nature = 1.0f; // 性格修正
        
        // 计算后的能力值
        public int maxHP;
        public int currentHP;
        public int attack;
        public int defense;
        public int spAtk;
        public int spDef;
        public int speed;
        
        // 战斗状态
        public bool isFainted => currentHP <= 0;
        public int statStageAtk = 0;   // -6 to +6
        public int statStageDef = 0;
        public int statStageSpAtk = 0;
        public int statStageSpDef = 0;
        public int statStageSpeed = 0;
        public int statStageAccuracy = 0;
        public int statStageEvasion = 0;
        
        // 技能槽 (最多 4 个)
        public SkillData[] moves = new SkillData[4];
        public int[] movePP = new int[4];
        public int[] moveMaxPP = new int[4];
        
        // 状态异常
        public StatusCondition status = StatusCondition.None;
        public int statusTurns = 0;
        
        public BattleCreature(CreatureData creatureData, int lvl = 50)
        {
            data = creatureData;
            level = lvl;
            
            // 初始化个体值为 31 (满)
            for (int i = 0; i < 6; i++) ivs[i] = 31;
            
            CalculateStats();
            
            // 初始化技能 PP
            if (data.levelUpSkills != null)
            {
                int moveIndex = 0;
                foreach (var skillLv in data.levelUpSkills)
                {
                    if (skillLv.level <= level && moveIndex < 4)
                    {
                        moves[moveIndex] = skillLv.skill;
                        moveMaxPP[moveIndex] = skillLv.skill.pp;
                        movePP[moveIndex] = skillLv.skill.pp;
                        moveIndex++;
                    }
                }
            }
        }
        
        /// <summary>
        /// 计算所有能力值
        /// </summary>
        public void CalculateStats()
        {
            maxHP = data.CalculateStat(StatType.HP, level, ivs[0], evs[0]);
            attack = data.CalculateStat(StatType.Attack, level, ivs[1], evs[1], nature);
            defense = data.CalculateStat(StatType.Defense, level, ivs[2], evs[2], nature);
            spAtk = data.CalculateStat(StatType.SpAtk, level, ivs[3], evs[3], nature);
            spDef = data.CalculateStat(StatType.SpDef, level, ivs[4], evs[4], nature);
            speed = data.CalculateStat(StatType.Speed, level, ivs[5], evs[5], nature);
            
            if (currentHP == 0 || currentHP > maxHP)
                currentHP = maxHP;
        }
        
        /// <summary>
        /// 造成伤害
        /// </summary>
        public int TakeDamage(int damage)
        {
            currentHP = Mathf.Max(0, currentHP - damage);
            return currentHP;
        }
        
        /// <summary>
        /// 回复 HP
        /// </summary>
        public int HealHP(int amount)
        {
            int oldHP = currentHP;
            currentHP = Mathf.Min(maxHP, currentHP + amount);
            return currentHP - oldHP;
        }
        
        /// <summary>
        /// 获取能力阶段修正倍率
        /// </summary>
        public float GetStatStageMultiplier(int stage)
        {
            if (stage >= 0)
                return (2 + stage) / 2.0f;
            else
                return 2 / (2 + Mathf.Abs(stage)) * 1.0f;
        }
        
        /// <summary>
        /// 获取属性克制倍率 (考虑双重属性)
        /// </summary>
        public float GetTypeEffectiveness(ElementType attackType)
        {
            if (data.HasSecondaryType())
            {
                return TypeChart.GetEffectiveness(attackType, data.primaryType, data.secondaryType);
            }
            else
            {
                return TypeChart.GetEffectiveness(attackType, data.primaryType);
            }
        }
    }
    
    /// <summary>
    /// 状态异常枚举
    /// </summary>
    public enum StatusCondition
    {
        None,
        Poison,      // 中毒
        BadPoison,   // 剧毒
        Paralysis,   // 麻痹
        Sleep,       // 睡眠
        Freeze,      // 冰冻
        Burn         // 灼伤
    }
}

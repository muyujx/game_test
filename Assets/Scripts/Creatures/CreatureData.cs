using UnityEngine;

namespace PokemonBattle.Creatures
{
    /// <summary>
    /// 宝可梦种族值数据结构 (ScriptableObject)
    /// 用于定义每个物种的基础能力值
    /// </summary>
    [CreateAssetMenu(fileName = "NewCreature", menuName = "Pokemon Battle/Creature Data")]
    public class CreatureData : ScriptableObject
    {
        [Header("基础信息")]
        public string creatureName = "New Creature";
        public Sprite icon;
        public GameObject modelPrefab;
        
        [Header("属性")]
        public ElementType primaryType = ElementType.Normal;
        public ElementType secondaryType = ElementType.Normal; // 可为 Normal 表示单属性
        
        [Header("种族值 (Base Stats)")]
        [Range(1, 255)] public int baseHP = 50;
        [Range(1, 255)] public int baseAttack = 50;
        [Range(1, 255)] public int baseDefense = 50;
        [Range(1, 255)] public int baseSpAtk = 50;
        [Range(1, 255)] public int baseSpDef = 50;
        [Range(1, 255)] public int baseSpeed = 50;
        
        [Header("成长参数")]
        [Range(1, 3)] public int growthRate = 2; // 1=慢，2=中，3=快
        [Range(1, 100)] public int catchRate = 45;
        
        [Header("可学技能")]
        public SkillLevelUp[] levelUpSkills;
        
        [Header("其他")]
        public float height = 1.0f; // 米
        public float weight = 10.0f; // 千克
        public string description = "";

        /// <summary>
        /// 计算实际能力值 (简化版公式)
        /// HP = ((2 * Base + IV + EV/4) * Level / 100 + 10 + Level)
        /// 其他 = (((2 * Base + IV + EV/4) * Level / 100 + 5) * Nature)
        /// </summary>
        public int CalculateStat(StatType statType, int level, int iv = 31, int ev = 0, float nature = 1.0f)
        {
            int baseStat = GetBaseStat(statType);
            
            if (statType == StatType.HP)
            {
                return Mathf.FloorToInt((2 * baseStat + iv + ev / 4f) * level / 100f + 10 + level);
            }
            else
            {
                return Mathf.FloorToInt(((2 * baseStat + iv + ev / 4f) * level / 100f + 5) * nature);
            }
        }

        private int GetBaseStat(StatType statType)
        {
            return statType switch
            {
                StatType.HP => baseHP,
                StatType.Attack => baseAttack,
                StatType.Defense => baseDefense,
                StatType.SpAtk => baseSpAtk,
                StatType.SpDef => baseSpDef,
                StatType.Speed => baseSpeed,
                _ => 0
            };
        }

        public bool HasSecondaryType()
        {
            return secondaryType != ElementType.Normal;
        }
    }

    /// <summary>
    /// 能力值类型
    /// </summary>
    public enum StatType
    {
        HP,
        Attack,
        Defense,
        SpAtk,
        SpDef,
        Speed
    }

    /// <summary>
    /// 升级习得技能数据
    /// </summary>
    [System.Serializable]
    public struct SkillLevelUp
    {
        public int level;
        public SkillData skill;
    }

    /// <summary>
    /// 技能数据结构 (占位，后续实现)
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Pokemon Battle/Skill Data")]
    public class SkillData : ScriptableObject
    {
        public string skillName = "New Skill";
        public ElementType skillType = ElementType.Normal;
        public SkillCategory category = SkillCategory.Physical;
        public int power = 0;
        [Range(0, 100)] public int accuracy = 100;
        [Range(1, 40)] public int pp = 35;
        public int priority = 0;
    }

    public enum SkillCategory
    {
        Physical,  // 物理
        Special,   // 特殊
        Status     // 变化
    }
}

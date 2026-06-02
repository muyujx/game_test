using UnityEngine;

namespace PokemonBattle.Battle
{
    /// <summary>
    /// 战斗场景引导脚本
    /// 用于在 Unity 编辑器中快速设置和测试战斗
    /// </summary>
    public class BattleSceneSetup : MonoBehaviour
    {
        [Header("测试数据 (拖入 ScriptableObject)")]
        public CreatureData playerCreatureData;
        public CreatureData enemyCreatureData;
        
        [Header("场景引用")]
        public BattleManager battleManager;
        public BattleUI battleUI;
        
        private void Start()
        {
            // 如果未手动赋值，尝试自动查找
            if (battleManager == null)
                battleManager = FindObjectOfType<BattleManager>();
                
            if (battleUI == null)
                battleUI = FindObjectOfType<BattleUI>();
            
            // 创建战斗实例
            if (playerCreatureData != null && enemyCreatureData != null && battleManager != null)
            {
                SetupBattle();
            }
            else
            {
                Debug.LogWarning("请在 Inspector 中分配 CreatureData 和 BattleManager!");
                Debug.Log("按空格键使用默认测试数据");
            }
        }
        
        private void Update()
        {
            // 按空格键快速测试
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CreateTestData();
                SetupBattle();
            }
        }
        
        void SetupBattle()
        {
            if (battleManager == null) return;
            
            // 创建战斗生物实例
            battleManager.playerCreature = new BattleCreature(playerCreatureData, 50);
            battleManager.enemyCreature = new BattleCreature(enemyCreatureData, 50);
            
            Debug.Log($"战斗开始! {playerCreatureData.creatureName} VS {enemyCreatureData.creatureName}");
            Debug.Log($"属性：{playerCreatureData.primaryType} vs {enemyCreatureData.primaryType}");
            
            // 初始化战斗
            battleManager.InitializeBattle();
            
            // 更新 UI
            if (battleUI != null)
            {
                battleUI.UpdateMoveButtons();
                battleUI.UpdateHPBars();
            }
        }
        
        /// <summary>
        /// 创建测试用的 ScriptableObject 数据
        /// </summary>
        void CreateTestData()
        {
            if (playerCreatureData == null)
            {
                playerCreatureData = ScriptableObject.CreateInstance<CreatureData>();
                playerCreatureData.creatureName = "小火龙";
                playerCreatureData.primaryType = ElementType.Fire;
                playerCreatureData.baseHP = 39;
                playerCreatureData.baseAttack = 52;
                playerCreatureData.baseDefense = 43;
                playerCreatureData.baseSpAtk = 60;
                playerCreatureData.baseSpDef = 50;
                playerCreatureData.baseSpeed = 65;
                
                // 添加技能
                var scratch = CreateSkill("抓", ElementType.Normal, SkillCategory.Physical, 40, 100, 35);
                var ember = CreateSkill("火花", ElementType.Fire, SkillCategory.Special, 40, 100, 25);
                var flameWheel = CreateSkill("火焰轮", ElementType.Fire, SkillCategory.Physical, 60, 100, 25);
                
                playerCreatureData.levelUpSkills = new SkillLevelUp[]
                {
                    new SkillLevelUp { level = 1, skill = scratch },
                    new SkillLevelUp { level = 1, skill = ember },
                    new SkillLevelUp { level = 10, skill = flameWheel }
                };
            }
            
            if (enemyCreatureData == null)
            {
                enemyCreatureData = ScriptableObject.CreateInstance<CreatureData>();
                enemyCreatureData.creatureName = "杰尼龟";
                enemyCreatureData.primaryType = ElementType.Water;
                enemyCreatureData.baseHP = 44;
                enemyCreatureData.baseAttack = 48;
                enemyCreatureData.baseDefense = 65;
                enemyCreatureData.baseSpAtk = 50;
                enemyCreatureData.baseSpDef = 64;
                enemyCreatureData.baseSpeed = 43;
                
                // 添加技能
                var tackle = CreateSkill("撞击", ElementType.Normal, SkillCategory.Physical, 40, 100, 35);
                var waterGun = CreateSkill("水枪", ElementType.Water, SkillCategory.Special, 40, 100, 25);
                var bite = CreateSkill("咬住", ElementType.Dark, SkillCategory.Physical, 60, 100, 25);
                
                enemyCreatureData.levelUpSkills = new SkillLevelUp[]
                {
                    new SkillLevelUp { level = 1, skill = tackle },
                    new SkillLevelUp { level = 1, skill = waterGun },
                    new SkillLevelUp { level = 10, skill = bite }
                };
            }
        }
        
        SkillData CreateSkill(string name, ElementType type, SkillCategory category, 
                              int power, int accuracy, int pp)
        {
            var skill = ScriptableObject.CreateInstance<SkillData>();
            skill.skillName = name;
            skill.skillType = type;
            skill.category = category;
            skill.power = power;
            skill.accuracy = accuracy;
            skill.pp = pp;
            return skill;
        }
    }
}

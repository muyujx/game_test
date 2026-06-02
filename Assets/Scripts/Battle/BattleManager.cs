using System.Collections;
using UnityEngine;

namespace PokemonBattle.Battle
{
    /// <summary>
    /// 战斗状态机
    /// 管理回合制战斗流程
    /// </summary>
    public enum BattleState
    {
        Start,           // 战斗开始
        PlayerTurn,      // 玩家回合
        EnemyTurn,       // 敌人回合
        PerformingMove,  // 技能执行中
        Busy,            // 忙碌状态 (动画播放等)
        Win,             // 胜利
        Lose             // 失败
    }

    /// <summary>
    /// 战斗管理器
    /// 单例模式，控制整个战斗流程
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }
        
        [Header("战斗双方")]
        public BattleCreature playerCreature;
        public BattleCreature enemyCreature;
        
        [Header("UI 引用")]
        public GameObject battleUI;
        public UnityEngine.UI.Text battleLogText;
        public UnityEngine.UI.Slider playerHPBar;
        public UnityEngine.UI.Slider enemyHPBar;
        
        [Header("模型位置")]
        public Transform playerPosition;
        public Transform enemyPosition;
        
        // 当前状态
        public BattleState currentState = BattleState.Start;
        
        // 先手判定
        private bool playerGoesFirst;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            InitializeBattle();
        }
        
        /// <summary>
        /// 初始化战斗
        /// </summary>
        public void InitializeBattle()
        {
            currentState = BattleState.Start;
            
            // 判断先手 (速度比较)
            playerGoesFirst = playerCreature.speed >= enemyCreature.speed;
            
            StartCoroutine(StartBattleSequence());
        }
        
        IEnumerator StartBattleSequence()
        {
            currentState = BattleState.Busy;
            
            YieldMessage($"野生 {enemyCreature.data.creatureName} 出现了!");
            yield return new WaitForSeconds(1.5f);
            
            YieldMessage($"去吧! {playerCreature.data.creatureName}!");
            yield return new WaitForSeconds(1.5f);
            
            // 进入玩家回合
            currentState = playerGoesFirst ? BattleState.PlayerTurn : BattleState.EnemyTurn;
            
            if (!playerGoesFirst)
            {
                StartCoroutine(EnemyAI());
            }
        }
        
        /// <summary>
        /// 玩家选择技能
        /// </summary>
        public void PlayerChooseMove(int moveIndex)
        {
            if (currentState != BattleState.PlayerTurn)
                return;
                
            if (playerCreature.movePP[moveIndex] <= 0)
            {
                YieldMessage("没有 PP 了!");
                return;
            }
            
            StartCoroutine(ExecuteMove(playerCreature, enemyCreature, moveIndex, true));
        }
        
        /// <summary>
        /// 敌人 AI (简单随机)
        /// </summary>
        IEnumerator EnemyAI()
        {
            currentState = BattleState.Busy;
            yield return new WaitForSeconds(1f);
            
            // 选择一个有 PP 的技能
            int availableMove = -1;
            for (int i = 0; i < 4; i++)
            {
                if (enemyCreature.moves[i] != null && enemyCreature.movePP[i] > 0)
                {
                    availableMove = i;
                    break;
                }
            }
            
            if (availableMove == -1)
            {
                // 所有技能都没 PP 了，使用挣扎
                YieldMessage($"{enemyCreature.data.creatureName} 在挣扎!");
                StartCoroutine(ExecuteStruggle(enemyCreature, playerCreature, false));
            }
            else
            {
                StartCoroutine(ExecuteMove(enemyCreature, playerCreature, availableMove, false));
            }
        }
        
        /// <summary>
        /// 执行技能
        /// </summary>
        IEnumerator ExecuteMove(BattleCreature attacker, BattleCreature defender, int moveIndex, bool isPlayer)
        {
            currentState = BattleState.PerformingMove;
            
            SkillData skill = attacker.moves[moveIndex];
            
            // 减少 PP
            attacker.movePP[moveIndex]--;
            
            YieldMessage($"{attacker.data.creatureName} 使用了 {skill.skillName}!");
            yield return new WaitForSeconds(1f);
            
            // 命中判定
            if (!DamageCalculator.CheckAccuracy(attacker, defender, skill))
            {
                YieldMessage($"{attacker.data.creatureName} 的攻击 missed!");
                yield return new WaitForSeconds(1f);
                EndTurn(isPlayer);
                yield break;
            }
            
            // 伤害计算
            if (skill.category != SkillCategory.Status)
            {
                int damage = DamageCalculator.CalculateDamage(attacker, defender, skill, moveIndex);
                defender.TakeDamage(damage);
                
                YieldMessage($"造成了 {damage} 点伤害!");
                
                // 更新血条
                if (isPlayer)
                {
                    UpdateEnemyHPBar();
                }
                else
                {
                    UpdatePlayerHPBar();
                }
                
                yield return new WaitForSeconds(1f);
            }
            else
            {
                YieldMessage("但是没有任何效果...");
                yield return new WaitForSeconds(1f);
            }
            
            // 检查是否倒下
            if (defender.isFainted)
            {
                YieldMessage($"{defender.data.creatureName} 倒下了!");
                yield return new WaitForSeconds(1.5f);
                
                currentState = isPlayer ? BattleState.Win : BattleState.Lose;
                YieldMessage(isPlayer ? "胜利了!" : "失败了...");
                yield break;
            }
            
            EndTurn(isPlayer);
        }
        
        IEnumerator ExecuteStruggle(BattleCreature attacker, BattleCreature defender, bool isPlayer)
        {
            // 挣扎技能简化处理
            int damage = Mathf.Max(1, attacker.attack - defender.defense / 2);
            defender.TakeDamage(damage);
            attacker.TakeDamage(damage / 4); // 反伤
            
            YieldMessage($"挣扎造成了 {damage} 点伤害!");
            yield return new WaitForSeconds(1f);
            
            EndTurn(isPlayer);
        }
        
        /// <summary>
        /// 结束回合
        /// </summary>
        void EndTurn(bool isPlayerTurn)
        {
            if (currentState == BattleState.Win || currentState == BattleState.Lose)
                return;
                
            currentState = isPlayerTurn ? BattleState.EnemyTurn : BattleState.PlayerTurn;
            
            if (currentState == BattleState.EnemyTurn)
            {
                StartCoroutine(EnemyAI());
            }
        }
        
        void UpdatePlayerHPBar()
        {
            if (playerHPBar != null)
            {
                float hpPercent = (float)playerCreature.currentHP / playerCreature.maxHP;
                playerHPBar.value = hpPercent;
            }
        }
        
        void UpdateEnemyHPBar()
        {
            if (enemyHPBar != null)
            {
                float hpPercent = (float)enemyCreature.currentHP / enemyCreature.maxHP;
                enemyHPBar.value = hpPercent;
            }
        }
        
        void YieldMessage(string message)
        {
            Debug.Log(message);
            if (battleLogText != null)
            {
                battleLogText.text = message;
            }
        }
    }
}

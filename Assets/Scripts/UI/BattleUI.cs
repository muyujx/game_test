using UnityEngine;

namespace PokemonBattle.Battle
{
    /// <summary>
    /// 战斗 UI 控制器
    /// 处理技能选择按钮、血条更新等
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        [Header("技能按钮")]
        public UnityEngine.UI.Button[] moveButtons = new UnityEngine.UI.Button[4];
        public UnityEngine.UI.Text[] moveNames = new UnityEngine.UI.Text[4];
        public UnityEngine.UI.Text[] movePPs = new UnityEngine.UI.Text[4];
        
        [Header("血条")]
        public UnityEngine.UI.Slider playerHPBar;
        public UnityEngine.UI.Slider enemyHPBar;
        public UnityEngine.UI.Text playerHPText;
        public UnityEngine.UI.Text enemyHPText;
        
        [Header="战斗文本"]
        public UnityEngine.UI.Text battleLogText;
        
        [Header("动画")]
        public GameObject playerModel;
        public GameObject enemyModel;
        
        private BattleManager battleManager;
        
        private void Awake()
        {
            battleManager = BattleManager.Instance;
            
            // 绑定按钮事件
            for (int i = 0; i < moveButtons.Length; i++)
            {
                int index = i; // 闭包捕获
                moveButtons[i].onClick.AddListener(() => OnMoveSelected(index));
            }
        }
        
        private void Start()
        {
            UpdateMoveButtons();
        }
        
        /// <summary>
        /// 玩家选择技能
        /// </summary>
        void OnMoveSelected(int moveIndex)
        {
            if (battleManager != null)
            {
                battleManager.PlayerChooseMove(moveIndex);
            }
        }
        
        /// <summary>
        /// 更新技能按钮显示
        /// </summary>
        public void UpdateMoveButtons()
        {
            if (battleManager == null || battleManager.playerCreature == null)
                return;
                
            var creature = battleManager.playerCreature;
            
            for (int i = 0; i < 4; i++)
            {
                if (creature.moves[i] != null)
                {
                    moveNames[i].text = creature.moves[i].skillName;
                    movePPs[i].text = $"PP {creature.movePP[i]}/{creature.moveMaxPP[i]}";
                    moveButtons[i].interactable = creature.movePP[i] > 0;
                }
                else
                {
                    moveNames[i].text = "---";
                    movePPs[i].text = "";
                    moveButtons[i].interactable = false;
                }
            }
        }
        
        /// <summary>
        /// 更新血条
        /// </summary>
        public void UpdateHPBars()
        {
            if (battleManager == null)
                return;
                
            if (battleManager.playerCreature != null && playerHPBar != null)
            {
                var pc = battleManager.playerCreature;
                float hpPercent = (float)pc.currentHP / pc.maxHP;
                playerHPBar.value = hpPercent;
                playerHPText.text = $"{pc.currentHP}/{pc.maxHP}";
                
                // 根据血量改变颜色
                playerHPBar.GetComponent<UnityEngine.UI.Image>().color = GetHPColor(hpPercent);
            }
            
            if (battleManager.enemyCreature != null && enemyHPBar != null)
            {
                var ec = battleManager.enemyCreature;
                float hpPercent = (float)ec.currentHP / ec.maxHP;
                enemyHPBar.value = hpPercent;
                enemyHPText.text = $"{ec.currentHP}/{ec.maxHP}";
                
                enemyHPBar.GetComponent<UnityEngine.UI.Image>().color = GetHPColor(hpPercent);
            }
        }
        
        Color GetHPColor(float percent)
        {
            if (percent > 0.5f)
                return new Color(0.3f, 0.9f, 0.3f); // 绿色
            else if (percent > 0.2f)
                return new Color(0.9f, 0.9f, 0.3f); // 黄色
            else
                return new Color(0.9f, 0.3f, 0.3f); // 红色
        }
        
        /// <summary>
        /// 播放攻击动画
        /// </summary>
        public void PlayAttackAnimation(bool isPlayer)
        {
            // TODO: 触发 Animator 动画
            Debug.Log(isPlayer ? "玩家攻击动画" : "敌人攻击动画");
        }
        
        /// <summary>
        /// 播放受击动画
        /// </summary>
        public void PlayHitAnimation(bool isPlayer)
        {
            // TODO: 触发 Animator 受击动画
            Debug.Log(isPlayer ? "玩家受击动画" : "敌人受击动画");
        }
        
        /// <summary>
        /// 播放倒下动画
        /// </summary>
        public void PlayFaintAnimation(bool isPlayer)
        {
            // TODO: 触发 Animator 倒下动画
            Debug.Log(isPlayer ? "玩家倒下动画" : "敌人倒下动画");
        }
    }
}

# 3D 宝可梦风格回合对战游戏技术架构设计文档

## 1. 技术栈总览

基于 Unity 引擎的完整技术栈选型如下：

### 1.1 核心引擎
| 组件 | 选型 | 版本 | 说明 |
|------|------|------|------|
| **游戏引擎** | Unity | 2022.3 LTS | 长期支持版本，稳定性最佳 |
| **编程语言** | C# | .NET Standard 2.1 | Unity 默认支持 |
| **IDE** | Visual Studio 2022 / Rider | 最新版 | Rider 对 Unity 支持更好 |

### 1.2 渲染与图形
| 组件 | 选型 | 说明 |
|------|------|------|
| **渲染管线** | URP (Universal Render Pipeline) | 平衡画质与性能，支持多平台，适合卡通渲染 |
| **Shader 语言** | HLSL / Shader Graph | 可视化 Shader 编辑，实现卡通渲染效果 |
| **光照系统** | URP Lighting + Lightmap | 混合实时光照与烘焙光照 |
| **后处理** | URP Post-Processing | 体积雾、Bloom、色彩校正等 |
| **粒子系统** | VFX Graph / Particle System | 技能特效、精灵登场特效、进化特效等 |
| **卡通渲染** | UnityChanToonShader 2.0 / URP Toon Lit | 统一动漫风格渲染 |

### 1.3 动画系统
| 组件 | 选型 | 说明 |
|------|------|------|
| **角色动画** | Animator + Mecanim | 状态机管理宝可梦/训练师动画 |
| **过场动画** | Timeline + Cinemachine | 技能释放、登场、进化动画序列 |
| **动画融合** | Avatar Mask + Layer | 多层动画叠加（如移动时攻击） |
| **程序化动画** | Animation Rigging | 动态调整朝向、视线追踪 |
| **骨骼重定向** | Retargeting | 复用 Mixamo/第三方动作库 |

### 1.4 UI 系统
| 组件 | 选型 | 说明 |
|------|------|------|
| **主要 UI** | UGUI | 成熟稳定，社区资源丰富 |
| **UI 框架** | UniRx + 自定义 MVC | 响应式 UI 数据绑定 |
| **UI 动画** | DOTween Pro | 流畅的 UI 过渡动画 |
| **字体** | TextMeshPro | 高质量文本渲染，支持富文本 |
| **适配方案** | Canvas Scaler + Reference Resolution | 多分辨率自适应 |

### 1.5 网络同步
| 方案 | 推荐度 | 说明 | 成本 |
|------|--------|------|------|
| **Photon Fusion** | ⭐⭐⭐⭐⭐ | 最新一代，支持帧同步和状态同步 | 免费 20 CCU，超出后付费 |
| **Photon PUN 2** | ⭐⭐⭐⭐ | 成熟稳定，文档丰富 | 免费 20 CCU |
| **Unity Netcode for GameObjects** | ⭐⭐⭐ | Unity 官方方案，免费 | 免费 |
| **Mirror Networking** | ⭐⭐⭐ | 开源免费，社区维护 | 免费 |

**推荐选择：Photon Fusion**
- 理由：专为竞技游戏设计，支持延迟补偿和回滚
- 宝可梦风格回合制对战对实时性要求中等，Fusion 完全满足
- Photon 服务器托管，减少运维成本
- 支持状态同步，适合回合制验证逻辑

### 1.6 架构模式
| 层级 | 模式 | 说明 |
|------|------|------|
| **整体架构** | ECS + 分层架构 | 数据与逻辑分离 |
| **游戏逻辑** | State Pattern | 回合状态、战斗阶段管理 |
| **事件系统** | Observer Pattern | UniRx / C# Event |
| **依赖注入** | VContainer / Zenject | IOC 容器管理依赖 |
| **数据驱动** | ScriptableObject | 宝可梦数据、技能数据、配置表 |

---

## 2. 项目目录结构设计

```
Assets/
├── _Project/                    # 项目主目录（按功能模块组织）
│   ├── Core/                    # 核心系统
│   │   ├── GameManagers/        # 游戏管理器（GameManager, NetworkManager 等）
│   │   ├── Events/              # 全局事件定义
│   │   ├── Constants/           # 常量定义
│   │   └── Utilities/           # 通用工具类
│   │
│   ├── Creatures/               # 宝可梦/生物系统（原 Cards/Minions）
│   │   ├── Data/                # 宝可梦数据（ScriptableObject）
│   │   ├── Models/              # 宝可梦数据模型（种族值、属性、特性）
│   │   ├── Views/               # 宝可梦 3D 模型、预制体
│   │   ├── Controllers/         # 宝可梦行为逻辑
│   │   ├── Animations/          # 宝可梦动画（ idle、attack、hit、faint 等）
│   │   ├── AI/                  # 野生宝可梦 AI
│   │   └── Evolutions/          # 进化逻辑和数据
│   │
│   ├── Skills/                  # 技能系统
│   │   ├── Data/                # 技能数据（威力、命中、效果）
│   │   ├── Effects/             # 技能效果实现
│   │   ├── Animations/          # 技能动画
│   │   └── VFX/                 # 技能特效
│   │
│   ├── Battle/                  # 战斗系统
│   │   ├── Managers/            # 战斗管理器
│   │   ├── States/              # 战斗状态机（回合、天气、场地）
│   │   ├── TurnSystem/          # 回合系统（速度判定、先制度）
│   │   ├── DamageCalc/          # 伤害计算公式
│   │   ├── StatusEffects/       # 异常状态（中毒、麻痹、睡眠等）
│   │   ├── Weather/             # 天气系统
│   │   └── Terrain/             # 场地效果
│   │
│   ├── Trainers/                # 训练师系统
│   │   ├── PlayerData/          # 玩家数据
│   │   ├── Party/               # 队伍管理（6 只宝可梦）
│   │   ├── Bag/                 # 背包系统（道具、精灵球）
│   │   └── Hero/                # 训练师 3D 模型和动画
│   │
│   ├── Items/                   # 道具系统
│   │   ├── Data/                # 道具数据
│   │   ├── Effects/             # 道具效果
│   │   └── PokeBalls/           # 精灵球逻辑
│   │
│   ├── Network/                 # 网络层
│   │   ├── Messages/            # 网络消息定义
│   │   ├── Handlers/            # 消息处理器
│   │   ├── Sync/                # 同步逻辑
│   │   └── Photon/              # Photon SDK 封装
│   │
│   ├── UI/                      # UI 系统
│   │   ├── Screens/             # 界面屏幕（主界面、战斗界面、地图等）
│   │   ├── Windows/             # 弹窗窗口（对话框、菜单）
│   │   ├── HUD/                 # 抬头显示（血条、经验条）
│   │   ├── Components/          # UI 组件
│   │   ├── ViewModels/          # MVVM ViewModel
│   │   └── Animations/          # UI 动画
│   │
│   ├── Audio/                   # 音频系统
│   │   ├── Music/               # 背景音乐（战斗 BGM、城镇 BGM）
│   │   ├── SFX/                 # 音效（技能音效、叫声）
│   │   ├── Voice/               # 语音（可选）
│   │   └── Managers/            # 音频管理器
│   │
│   ├── VFX/                     # 视觉特效
│   │   ├── SkillEffects/        # 技能特效
│   │   ├── CreatureEffects/     # 宝可梦特效（登场、进化）
│   │   ├── BattleEffects/       # 战斗特效
│   │   └── UIEffects/           # UI 特效
│   │
│   ├── Data/                    # 数据配置
│   │   ├── CreatureDatabase/    # 宝可梦数据库
│   │   ├── SkillDatabase/       # 技能数据库
│   │   ├── ItemDatabase/        # 道具数据库
│   │   ├── GameConfig/          # 游戏配置
│   │   └── Localization/        # 本地化文本
│   │
│   └── Resources/               # 可动态加载的资源
│       ├── Prefabs/             # 预制体
│       ├── Models/              # 3D 模型
│       ├── Textures/            # 贴图
│       └── Addressables/        # Addressable 资源
│
├── Plugins/                     # 第三方插件
│   ├── Photon/                  # Photon SDK
│   ├── DOTween/                 # DOTween 动画库
│   ├── UniRx/                   # 响应式扩展
│   └── VContainer/              # IOC 容器
│
├── Scenes/                      # 场景文件
│   ├── Boot/                    # 启动场景
│   ├── Lobby/                   # 大厅/城镇场景
│   ├── Battle/                  # 战斗场景
│   ├── Map/                     # 大地图场景
│   └── Test/                    # 测试场景
│
├── Scripts/                     # 全局脚本（非项目特定）
│
└── StreamingAssets/             # 流式资源
    └── Configs/                 # 热更配置文件
```

---

## 3. 开发工具链选型

### 3.1 版本控制
| 工具 | 选型 | 说明 |
|------|------|------|
| **版本控制系统** | Git | 分布式版本控制 |
| **代码托管** | GitHub / GitLab | 私有仓库 |
| **大文件管理** | Git LFS | 管理美术资源、音频等大文件 |
| **分支策略** | Git Flow | feature/develop/release/hotfix/master |

**.gitignore 关键配置：**
```
# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Unity-specific
*.pidb.meta
*.pdb.meta
*.mdb.meta
sysinfo.txt
*.apk
*.unitypackage
```

### 3.2 项目管理与协作
| 工具 | 用途 |
|------|------|
| **Jira / Trello** | 任务管理和 Sprint 规划 |
| **Confluence / Notion** | 文档协作 |
| **Figma** | UI/UX设计和原型 |
| **Discord / Slack** | 团队沟通 |

### 3.3 CI/CD
| 工具 | 选型 | 说明 |
|------|------|------|
| **CI/CD平台** | GitHub Actions / Jenkins | 自动化构建和测试 |
| **Unity Cloud Build** | 可选 | Unity 官方云构建服务 |
| **测试框架** | NUnit + Unity Test Framework | 单元测试和集成测试 |
| **代码质量** | SonarQube | 代码质量检查 |

**GitHub Actions 示例工作流：**
```yaml
name: Unity Build

on:
  push:
    branches: [ develop, main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Cache Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ hashFiles('Assets/**', 'Packages/**') }}
      
      - name: Run Tests
        run: |
          xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
          Unity-editor -batchmode -runTests -testPlatform editmode
      
      - name: Build Android
        run: |
          Unity-editor -batchmode -buildAndroidApk
```

### 3.4 代码规范与审查
| 工具 | 用途 |
|------|------|
| **EditorConfig** | 统一编辑器配置 |
| **StyleCop / ReSharper** | C# 代码风格检查 |
| **Pull Request** | 代码审查流程 |

---

## 4. 第三方服务选型

### 4.1 后端服务
| 服务类型 | 推荐方案 | 备选方案 | 说明 |
|----------|----------|----------|------|
| **游戏服务器** | Photon Fusion | Unity Netcode | 多人对战同步 |
| **用户认证** | PlayFab / Firebase Auth | 自建 | 账号登录、第三方登录 |
| **数据存储** | PlayFab / Firebase Firestore | MongoDB | 玩家数据、卡组存储 |
| **匹配系统** | PlayFab Matchmaking | 自建 | 天梯匹配 |
| **排行榜** | PlayFab Leaderboards | 自建 | 天梯排名 |

**推荐组合：PlayFab（微软旗下）**
- 一站式游戏后端服务
- 包含用户管理、数据存储、匹配、排行榜、分析等
- 免费额度充足，按需付费
- Unity 官方集成支持好

### 4.2 数据分析与监控
| 服务 | 用途 |
|------|------|
| **Unity Analytics** | 游戏内行为分析 |
| **Firebase Analytics** | 用户分析和留存 |
| **Sentry** | 错误监控和崩溃报告 |
| **GameAnalytics** | 专业游戏数据分析 |

### 4.3 运营服务
| 服务 | 用途 |
|------|------|
| **Firebase Remote Config** | 远程配置（卡牌数值调整） |
| **PlayFab Economy** | 虚拟经济系统（金币、卡包） |
| **Unity Ads / AdMob** | 广告变现（可选） |
| **RevenueCat** | 内购订阅管理 |

### 4.4 开发者服务
| 服务 | 用途 |
|------|------|
| **Bugsnag / Sentry** | 崩溃监控 |
| **Datadog** | 性能监控 |
| **PlayFab Insights** | 游戏运营分析 |

---

## 5. 关键技术实现方案

### 5.1 宝可梦数据驱动设计

**使用 ScriptableObject 定义宝可梦数据：**

```csharp
[CreateAssetMenu(fileName = "NewCreature", menuName = "Pokemon/Creature")]
public class CreatureData : ScriptableObject
{
    public string creatureId;
    public string creatureName;
    public Sprite creatureIcon;
    public GameObject creaturePrefab;
    
    // 基础种族值
    public int baseHP;
    public int baseAttack;
    public int baseDefense;
    public int baseSpAtk;
    public int baseSpDef;
    public int baseSpeed;
    
    // 属性（可双属性）
    public CreatureType primaryType;
    public CreatureType? secondaryType;
    
    // 特性
    public List<AbilityData> abilities;
    public AbilityData hiddenAbility;
    
    // 技能池
    public List<SkillLearnset> learnset;
    
    // 进化链
    public EvolutionData evolutionData;
    
    // 捕捉率
    public float catchRate;
    public float experienceGrowth;
}

public enum CreatureType
{
    Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison,
    Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}

[System.Serializable]
public struct SkillLearnset
{
    public int level;
    public string skillId;
}
```

**技能数据结构：**

```csharp
[CreateAssetMenu(fileName = "NewSkill", menuName = "Pokemon/Skill")]
public class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;
    public string description;
    
    public SkillCategory category; // Physical, Special, Status
    public CreatureType skillType;
    
    public int power;      // 威力 (0 表示变化技能)
    public int accuracy;   // 命中率 (0-100)
    public int pp;         // PP 值
    
    public int priority;   // 先制度
    public SkillTarget target; // 目标选择
    
    public List<SkillEffect> effects;
    public GameObject vfxPrefab;
    public AudioClip soundEffect;
}

public enum SkillCategory { Physical, Special, Status }
public enum SkillTarget { Single, AllOpponents, AllAdjacent, Self, UserSide }
```

### 5.2 回合制状态机

```csharp
public class BattleStateMachine
{
    private IBattleState currentState;
    
    public void ChangeState(IBattleState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    public void Update() => currentState?.Update();
}

public interface IBattleState
{
    void Enter();
    void Update();
    void Exit();
}

// 具体状态 - 宝可梦对战流程
public class StartPhaseState : IBattleState 
{
    // 登场动画、开场台词
}

public class TurnStartPhaseState : IBattleState 
{
    // 回合开始：天气伤害、特性触发
}

public class InputPhaseState : IBattleState 
{
    // 等待玩家输入（战斗/背包/宝可梦/逃跑）
}

public class SkillSelectionState : IBattleState 
{
    // 选择技能
}

public class ActionResolutionState : IBattleState 
{
    // 行动结算：先手判定、技能动画、伤害计算
}

public class TurnEndPhaseState : IBattleState 
{
    // 回合结束：持续效果处理
}

public class FaintCheckState : IBattleState 
{
    // 濒死检查、替换宝可梦
}

public class VictoryCheckState : IBattleState 
{
    // 胜利条件检查
}
```

### 5.3 伤害计算公式

```csharp
public class DamageCalculator
{
    /// <summary>
    /// 宝可梦经典伤害公式
    /// </summary>
    public static int CalculateDamage(Creature attacker, Creature defender, SkillData skill)
    {
        // 基础伤害公式
        float baseDamage = ((2 * attacker.Level / 5f + 2) * skill.Power * 
                           (float)attacker.AttackStat / defender.DefenseStat) / 50f + 2;
        
        // 属性克制倍率
        float typeMultiplier = GetTypeEffectiveness(skill.skillType, defender);
        
        // 一致加成（STAB）
        float stab = HasSameTypeAsSkill(attacker, skill) ? 1.5f : 1.0f;
        
        // 暴击 (6.25% 概率)
        float critical = UnityEngine.Random.value < 0.0625f ? 1.5f : 1.0f;
        
        // 随机因子 (0.85 - 1.0)
        float random = UnityEngine.Random.Range(0.85f, 1.0f);
        
        // 最终伤害
        int damage = Mathf.FloorToInt(baseDamage * typeMultiplier * stab * critical * random);
        
        return Mathf.Max(1, damage); // 至少造成 1 点伤害
    }
    
    private static float GetTypeEffectiveness(CreatureType skillType, Creature defender)
    {
        // 查表获取属性克制关系
        // 例如：火 -> 草 = 2.0, 火 -> 水 = 0.5, 火 -> 龙 = 0.5
        return TypeChart.GetMultiplier(skillType, defender.PrimaryType, defender.SecondaryType);
    }
}
```

### 5.4 网络消息协议

```csharp
// 使用 Photon 的自定义消息
public class NetworkMessages
{
    // 战斗操作
    public const byte SelectSkill = 1;
    public const byte SwitchCreature = 2;
    public const byte UseItem = 3;
    public const byte RunAway = 4;
    
    // 游戏状态同步
    public const byte BattleStateSync = 10;
    public const byte CreatureStatsUpdate = 11;
    public const byte DamageDealt = 12;
    public const byte CreatureFainted = 13;
    public const byte EvolutionStarted = 14;
}

[Serializable]
public class SelectSkillMessage
{
    public string playerId;
    public int creatureIndex;     // 当前出战的宝可梦索引
    public string skillId;        // 选择的技能 ID
    public int targetIndex;       // 目标索引
}

[Serializable]
public class BattleStateMessage
{
    public string battleId;
    public int turnNumber;
    public PlayerBattleData[] players;
    public WeatherType currentWeather;
    public TerrainType currentTerrain;
}
```

### 5.4 地址able 资源管理

```csharp
public class ResourceManager : MonoBehaviour
{
    private AsyncOperationHandle currentHandle;
    
    public async Task<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;
        return handle.Result;
    }
    
    public async Task<GameObject> LoadCreaturePrefab(string creatureId)
    {
        return await LoadAssetAsync<GameObject>($"Creatures/{creatureId}");
    }
    
    public async Task<Sprite> LoadCreatureIcon(string creatureId)
    {
        return await LoadAssetAsync<Sprite>($"CreatureIcons/{creatureId}");
    }
}
```

---

## 6. 性能优化策略

### 6.1 渲染优化
| 优化项 | 措施 |
|--------|------|
| **Draw Call** | 合批（Static/Dynamic/GPU Instancing） |
| **LOD** | 随从模型设置 LOD Group |
| **遮挡剔除** | 启用 Occlusion Culling |
| **纹理压缩** | ASTC (移动端) / BC (PC) |
| **阴影优化** | 限制阴影距离，使用阴影 LOD |

### 6.2 内存优化
| 优化项 | 措施 |
|--------|------|
| **资源加载** | Addressables 异步加载，及时释放 |
| **对象池** | 卡牌、特效、UI 元素使用对象池 |
| **纹理管理** | 使用 Atlas，避免重复加载 |
| **音频压缩** | Vorbis 格式，根据重要性调整质量 |

### 6.3 网络优化
| 优化项 | 措施 |
|--------|------|
| **消息压缩** | Protobuf 序列化 |
| **预测与回滚** | 客户端预测，服务器权威验证 |
| **帧率同步** | 锁定逻辑更新频率（如 30Hz） |
| **断线重连** | 状态快照 + 增量同步 |

---

## 7. 安全考虑

### 7.1 反作弊
- **服务器权威**：所有关键逻辑在服务器端验证
- **消息加密**：Photon 内置加密
- **速率限制**：防止频繁请求
- **数据校验**：客户端上传数据合法性检查

### 7.2 数据安全
- **敏感数据**：不存储在客户端
- **通信加密**：HTTPS/TLS
- **防篡改**：关键资源签名验证

---

## 8. 技术债务管理

| 类别 | 预防措施 |
|------|----------|
| **代码质量** | Code Review + 静态分析 + 单元测试 |
| **文档缺失** | 强制要求 PR 附带文档更新 |
| **技术过时** | 定期评估依赖库版本，制定升级计划 |
| **性能问题** | 持续性能测试，建立性能预算 |

---

## 9. 里程碑与技术验证

### Phase 1: 技术验证（2 周）
- [ ] Unity 环境搭建完成
- [ ] 3D 宝可梦模型加载和基础动画
- [ ] 卡通渲染 Shader 配置
- [ ] Photon 网络连接测试

### Phase 2: 核心玩法原型（4 周）
- [ ] 完整的回合流程（速度判定、先手）
- [ ] 技能选择和效果结算
- [ ] 伤害计算和属性克制
- [ ] 异常状态系统（中毒、麻痹等）
- [ ] 本地双人对战

### Phase 3: 网络对战（4 周）
- [ ] Photon Fusion 集成
- [ ] 战斗状态同步
- [ ] 匹配系统接入
- [ ] 在线对战测试

### Phase 4: Alpha 版本（12 周）
- [ ] 50+ 宝可梦制作
- [ ] 100+ 技能制作
- [ ] 完整 UI 流程（战斗、背包、替换）
- [ ] 进化系统
- [ ] 天气和场地效果
- [ ] 音效和音乐
- [ ] 封闭测试

---

## 10. 附录：推荐学习资源

### Unity 核心
- [Unity Learn - Junior Programmer Pathway](https://learn.unity.com/pathway/junior-programmer)
- [Unity Scriptable Object 教程](https://unity.com/how-to/scriptable-objects)
- [URP 官方文档](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)

### 网络同步
- [Photon Fusion 文档](https://doc.photonengine.com/fusion/current/getting-started/getting-started)
- [GDC 网络同步演讲合集](https://www.gdcvault.com/)

### 架构设计
- [Unity Dependency Injection 指南](https://github.com/VContainer/VContainer)
- [UniRx 官方文档](https://github.com/neuecc/UniRx)

### 性能优化
- [Unity 性能优化最佳实践](https://unity.com/topics/performance-optimization)
- [Mobile Game Optimization](https://learn.unity.com/course/mobile-game-optimization-tips-and-tricks)

---

*文档版本: 1.0*  
*创建日期: 2025-06-02*  
*基于框架选型文档 (framework_selection.md) 延伸*

### 宝可梦游戏开发参考
- [Pokémon Showdown 源码](https://github.com/smogon/pokemon-showdown) - 伤害计算参考
- [OpenPokémon 项目](https://github.com/pret/pokeemerald) - 游戏机制参考

---

*文档版本：1.1 (宝可梦风格版)*  
*创建日期：2025-06-02*  
*更新日期：2025-06-02*  
*基于框架选型文档 (framework_selection.md) 延伸*

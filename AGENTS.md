# AGENTS.md - 京都动画回合制对战项目指南

## 1. 项目概述 (Project Overview)

本项目是一个基于 **Unity 2022.3 LTS** 开发的 **3D 动漫风格回合制对战游戏** Demo。
灵感来源于《宝可梦》系列战斗系统，角色阵容全部来自 **京都动画 (Kyoto Animation)** 的主要作品（如《凉宫春日》、《轻音少女》、《日常》、《玉子市场》等）。

### 核心目标
- 实现一个**完全确定性 (Deterministic)** 的战斗系统，摒弃随机概率（RNG），强调策略与预判。
- 构建数据驱动的角色与技能体系，支持快速扩展新角色。
- 提供统一的卡通渲染架构，兼容多来源的 3D 模型资源。
- 作为后续完整游戏开发的原型验证 (MVP)。

---

## 2. 项目结构 (Project Structure)

```text
Assets/
├── Resources/                  # 动态加载的资源目录
│   ├── Creatures/              # 角色数据 (ScriptableObject: CreatureData)
│   │   ├── HaruhiSeries/       # 按作品分类的子目录
│   │   ├── Nichijou/           # 《日常》角色数据
│   │   └── TamakoMarket/       # 《玉子市场》角色数据
│   ├── Skills/                 # 技能数据 (ScriptableObject: SkillData)
│   └── Prefabs/                # 预制体
│       ├── Creatures/          # 角色 3D 模型预制体
│       └── UI/                 # UI 预制体
├── Scripts/
│   ├── Battle/                 # 核心战斗逻辑
│   │   ├── BattleManager.cs    # 战斗状态机 (回合管理、胜负判定)
│   │   ├── BattleCreature.cs   # 角色实例逻辑 (HP、状态、动画触发)
│   │   ├── DamageCalculator.cs # 确定性伤害计算公式
│   │   ├── SkillExecutor.cs    # 技能效果执行器 (无 RNG)
│   │   └── PassiveHandler.cs   # 特性触发处理器 (确定性条件)
│   ├── Data/                   # 数据定义
│   │   ├── CreatureData.cs     # 种族值、属性、特性定义
│   │   ├── SkillData.cs        # 技能威力、PP、效果定义
│   │   ├── ElementType.cs      # 属性克制枚举
│   │   └── CreaturePassiveType.cs # 特性枚举
│   ├── UI/                     # 界面逻辑
│   │   └── BattleUI.cs         # 血条、菜单、日志更新
│   └── Editor/                 # 编辑器工具
│       ├── CreateKyoAniData.cs # 一键生成 44+ 角色数据的工具
│       └── ModelImportChecker.cs # 模型导入检测工具
├── Scenes/                     # 场景文件
│   └── BattleScene.unity       # 主战斗场景
└── Doc/                        # 设计文档
    ├── KyoAni_Character_Design.md # 角色设计文档
    ├── SkillPassiveDesign.md      # 技能与特性设计规范 (确定性)
    └── ProjectSetup.md            # 环境配置指南
```

---

## 3. 核心开发要求 (Core Requirements)

### 3.1 确定性原则 (NO RNG)
**严禁**在战斗逻辑中使用随机数 (`Random.Range`, `UnityEngine.Random`)。所有机制必须是可预测的。

- **❌ 禁止**: "暴击率 10%", "命中率 90%", "30% 几率中毒", "威力波动 85%-100%"。
- **✅ 允许**: 
  - **固定数值**: 伤害公式固定，威力恒定。
  - **条件触发**: "血量低于 50% 时触发", "每 3 回合触发一次", "连续使用第 2 次时威力翻倍"。
  - **状态互换**: "必定命中", "必定暴击" (作为技能效果而非概率)。
  - **回合计数**: 基于 `TurnCount` 的周期性效果。

### 3.2 数据驱动 (Data-Driven)
- 所有角色属性、技能效果必须通过 `ScriptableObject` 配置。
- `Scripts/` 目录下的代码**不得硬编码**具体角色数值。
- 新增角色只需运行编辑器工具或创建新的 SO 资产，无需修改核心逻辑代码。

### 3.3 架构规范
- **ECS 思想**: 战斗逻辑 (`BattleManager`) 与 表现层 (`BattleCreature`, `Animator`) 分离。
- **状态机**: 战斗流程必须严格遵循 `Start -> PlayerInput -> Execution -> EnemyAI -> End` 的状态流转。
- **URP 兼容**: 所有材质和 Shader 必须兼容 Universal Render Pipeline。

### 3.4 角色与技能设计
- **角色数量**: 目前已包含 **44+** 位京都动画角色。
- **特性 (Passive)**: 必须是被动触发的确定性规则（如：入场增益、血量阈值、回合周期）。
- **技能 (Skill)**: 必须明确描述其固定效果（如：回复固定比例 HP、强制交换位置、清除特定状态）。

---

## 4. AI 协作指南 (Agent Guidelines)

当 AI 助手参与本项目开发时，请遵循以下准则：

### 4.1 代码生成
- **语言**: C# (Unity 2022.3 LTS 标准)。
- **命名**: 使用 PascalCase (类/方法), camelCase (变量), _prefix (私有字段)。
- **注释**: 关键逻辑必须包含 XML 注释，解释确定性机制的实现方式。
- **安全性**: 生成代码前需检查是否隐含了随机逻辑（如 `Random.value`）。

### 4.2 资源处理
- **模型**: 假设用户将从 BOOTH/Sketchfab 获取模型。AI 应提供导入设置建议（Rig 类型, Material 替换）。
- **动画**: 优先使用 Humanoid 重定向机制。若涉及动画状态机，需确保逻辑状态与 Animator 参数同步。

### 4.3 问题排查
- 若遇到编译错误，优先检查 `ScriptableObject` 引用是否为空。
- 若战斗流程卡死，检查 `BattleManager` 的状态机转换条件是否满足（特别是确定性触发条件）。

### 4.4 扩展任务
- **新增角色**: 直接修改 `CreateKyoAniData.cs` 添加新条目，不要手动创建资产。
- **新增机制**: 先在 `Doc/SkillPassiveDesign.md` 中定义规则，再编写代码实现。

---

## 5. 快速开始 (Quick Start)

1. **初始化数据**: 
   - 打开 Unity -> `Tools` -> `KyoAni Demo` -> `Create All Character Data`。
   - 确认 `Assets/Resources` 下生成了 44+ 个角色数据和对应技能。

2. **配置场景**:
   - 打开 `Scenes/BattleScene.unity`。
   - 在 `BattleManager` 组件中，拖入任意两个 `CreatureData` (例如：`Haruhi` vs `Yuko`)。

3. **运行测试**:
   - 点击 Play。
   - 观察战斗日志，验证伤害计算是否符合预期（无随机波动）。
   - 验证特性是否在指定回合或条件下准确触发。

4. **替换模型** (可选):
   - 导入新的 `.fbx` 模型到 `Resources/Creatures`。
   - 在对应的 `CreatureData` 中替换 `Prefab` 引用。
   - 运行 `Tools` -> `Battle Demo` -> `Check Model Import` 确保配置正确。

---

## 6. 版本历史 (Version History)

- **v1.0**: 初始架构，实现基础回合制与伤害计算。
- **v1.1**: 引入 32 位京阿尼角色，确立数据驱动框架。
- **v1.2**: **重构为确定性系统**，移除所有概率机制，补全《日常》与《玉子市场》角色至 44 位。
- **v1.3**: 添加 `AGENTS.md` 规范，完善编辑器工具链。

# Unity 项目配置文件

## 项目设置指南

### 1. 创建 Unity 项目
```bash
# 使用 Unity Hub 创建新项目
项目名称：PokemonBattleDemo
模板：3D (URP)
Unity 版本：2022.3 LTS 或更高
```

### 2. 必需 Package 依赖
在 Package Manager (`Window > Package Manager`) 中安装:

- **Universal RP** (必装) - URP 渲染管线
- **TextMeshPro** (必装) - UI 文本渲染
- **Input System** (推荐) - 新输入系统
- **Cinemachine** (推荐) - 相机控制
- **Timeline** (可选) - 过场动画

### 3. 项目设置

#### Player Settings
- Company Name: `YourName`
- Product Name: `Pokemon Battle Demo`
- Default Orientation: `Landscape Left`
- Color Space: `Linear`

#### Quality Settings
- 删除不必要的质量级别，保留 `Medium` 和 `High`
- 设置 `Universal Render Pipeline Asset`

#### Graphics Settings
- Scriptable Render Pipeline Settings: 创建 URP Asset
- 启用 `SRP Batcher`

### 4. URP 配置

#### 创建 URP Asset
1. `Assets > Create > Rendering > Universal Render Pipeline > Pipeline Asset (Forward Renderer)`
2. 命名为 `URP_Renderer`
3. 再创建 `Pipeline Asset`, 命名为 `URP_Pipeline`
4. 在 `URP_Pipeline` 的 `Renderer List` 中添加 `URP_Renderer`

#### 卡通渲染设置
1. 导入 UnityChanToonShader 2.0 (从 GitHub 或 Booth)
2. 创建 Toon Material
3. 应用到角色模型

### 5. 输入系统设置

#### 创建 Input Action Asset
1. `Assets > Create > Input Actions`
2. 添加以下 Actions:
   - `Battle/SelectMove1` (Keyboard: 1)
   - `Battle/SelectMove2` (Keyboard: 2)
   - `Battle/SelectMove3` (Keyboard: 3)
   - `Battle/SelectMove4` (Keyboard: 4)
   - `Battle/Confirm` (Keyboard: Enter/Space)

### 6. 场景设置

#### 创建 BattleScene
1. 创建新场景 `Assets/Scenes/BattleScene`
2. 添加以下 GameObject:

```
Hierarchy:
├── Directional Light (URP)
├── Main Camera (with Cinemachine Brain)
├── BattleManager (Empty, with BattleManager script)
├── BattleUI (Canvas)
│   ├── PlayerHPBar
│   ├── EnemyHPBar
│   ├── MoveButtons (4 buttons)
│   └── BattleLogText
├── PlayerPosition (Empty)
├── EnemyPosition (Empty)
└── Environment (Optional)
    ├── Ground Plane
    └── Background
```

### 7. ScriptableObject 数据创建

#### 创建生物数据
1. 在 Project 窗口右键 `Create > Pokemon Battle > Creature Data`
2. 命名如 `Charmander`, `Squirtle`
3. 填写种族值、属性、技能等

#### 创建技能数据
1. 右键 `Create > Pokemon Battle > Skill Data`
2. 命名如 `Scratch`, `Ember`, `Water Gun`
3. 填写威力、属性、PP 等

### 8. 测试步骤

1. 打开 `BattleScene`
2. 选择 `BattleSceneSetup` GameObject
3. 在 Inspector 中分配:
   - Player Creature Data (小火龙)
   - Enemy Creature Data (杰尼龟)
   - Battle Manager
   - Battle UI
4. 运行场景
5. 按空格键或使用 UI 按钮测试战斗

### 9. 常见问题

#### Q: 模型显示粉色？
A: 缺少 Shader，确保导入 UnityChanToonShader 或使用 Standard Shader

#### Q: UI 不显示？
A: 检查 Canvas 的 `Canvas Scaler` 设置，推荐使用 `Scale With Screen Size`

#### Q: 脚本报错 "UnityEngine.UI" 找不到？
A: 确保安装了 `TextMeshPro` 和 `UGUI` 包

### 10. 下一步开发

- [ ] 导入 3D 动漫角色模型
- [ ] 设置 Animator Controller 和动画状态机
- [ ] 实现骨骼重定向 (Avatar Mask)
- [ ] 添加技能特效 (VFX Graph)
- [ ] 完善战斗菜单 (背包、替换宝可梦)
- [ ] 添加音效和背景音乐
- [ ] 实现存档系统

## 许可证
MIT License - 仅供学习参考

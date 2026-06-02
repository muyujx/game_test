# Pokemon Battle Demo - 宝可梦风格对战演示项目

## 项目概述
基于 Unity 2022.3 LTS + URP 的 3D 回合制对战游戏演示，支持动漫风格角色、属性克制、技能系统和经典伤害计算。

## 技术栈
- **引擎**: Unity 2022.3 LTS
- **渲染管线**: URP (Universal Render Pipeline)
- **卡通渲染**: UnityChanToonShader 2.0
- **语言**: C# (.NET Standard 2.1)
- **架构**: ECS + 状态机 + 事件驱动

## 目录结构
```
Assets/
├── Scripts/
│   ├── Core/              # 核心系统 (事件总线、对象池)
│   ├── Battle/            # 战斗系统 (状态机、伤害计算)
│   ├── Creatures/         # 生物系统 (种族值、属性)
│   ├── UI/                # 界面系统 (战斗菜单、血条)
│   └── Managers/          # 管理器 (GameManager, AudioManager)
├── Data/
│   ├── Creatures/         # 生物数据 (ScriptableObject)
│   └── Skills/            # 技能数据
├── Animations/            # 动画控制器和动画片段
├── Materials/Shaders/     # 卡通着色器和材质
├── Prefabs/
│   ├── Creatures/         # 生物预制体
│   └── Effects/           # 特效预制体
└── Scenes/                # 场景文件
```

## 核心功能
1. **属性克制系统**: 火→草→水→火循环克制
2. **经典伤害公式**: 基于宝可梦的伤害计算
3. **回合制战斗**: 速度决定出手顺序
4. **卡通渲染**: 统一动漫视觉风格
5. **骨骼重定向**: 支持不同体型模型复用动画

## 快速开始
1. 安装 Unity Hub 和 Unity 2022.3 LTS
2. 导入 URP 包 (Package Manager)
3. 导入 UnityChanToonShader 2.0
4. 打开 `Assets/Scenes/BattleScene.unity`
5. 运行场景体验对战

## 资源获取建议
- **角色模型**: BOOTH.pm (搜索 "Anime Character")
- **动作库**: Mixamo (免费，自动绑定 Humanoid)
- **图标素材**: Kenney.nl (免费商用)

## 许可证
MIT License - 仅供学习参考

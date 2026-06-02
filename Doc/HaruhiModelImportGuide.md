# 凉宫春日 3D 模型导入指南

## ⚠️ 重要版权说明

**凉宫春日**是《凉宫春日系列》的 copyrighted 角色，由谷川流创作、京都动画制作。

### 合法获取途径：
1. **BOOTH.pm** (推荐)
   - 网址：https://booth.pm
   - 搜索关键词：`涼宮ハルヒ 3D` 或 `Haruhi Suzumiya 3D model`
   - 价格范围：¥500 - ¥3000 JPY
   - ✅ 优点：官方授权同人作品多，质量高，支持 Unity
   - ⚠️ 注意：仔细阅读使用条款，部分禁止商用

2. **VRoid Hub**
   - 网址：https://hub.vroid.com
   - 搜索：`Suzumiya Haruhi`
   - ✅ 优点：免费资源多，CC 许可明确
   - ⚠️ 缺点：风格可能不统一

3. **Sketchfab**
   - 网址：https://sketchfab.com
   - 搜索：`Haruhi Suzumiya`
   - ⚠️ 必须筛选 "Downloadable" 且检查 License

### ❌ 禁止行为：
- 从不明来源下载盗版模型
- 未经授权使用官方游戏提取模型
- 用于商业项目未获得授权

---

## 📥 模型导入步骤

### 第一步：下载模型
1. 从上述平台购买/下载凉宫春日模型
2. 确保格式为 `.fbx` 或 `.unitypackage`
3. 检查许可证是否允许游戏使用

### 第二步：导入 Unity
```
1. 将 .fbx 文件拖入 Assets/Resources/Creatures/ 目录
2. 选中模型文件，在 Inspector 中设置：
   - Rig → Animation Type: Humanoid
   - Avatar Definition: Create From This Model
   - Apply 应用设置
```

### 第三步：配置材质
```
1. 如果模型自带材质丢失，重新分配 Shader：
   - 推荐使用 UnityChanToonShader 2.0
   - 或使用 URP Toon Lit
2. 调整纹理贴图（Albedo, Normal, Emission）
```

### 第四步：创建预制体
```
1. 将模型从 Project 拖入 Hierarchy
2. 添加组件：
   - Animator (如果未自动添加)
   - BattleCreature (我们的脚本)
3. 调整位置使脚部对齐地面 (Y=0)
4. 拖回 Assets/Prefabs/Creatures/ 成为预制体
```

### 第五步：创建数据配置
```
方法 A - 手动创建：
1. 右键 → Create → Creatures → CreatureData
2. 命名为 "Haruhi_Data"
3. 在 Inspector 填写属性：
   - Name: 凉宫春日
   - Types: Normal / Psychic (自定义)
   - Base Stats: 根据设定调整
   - Battle Prefab: 拖入上一步的预制体
   - Skills: 添加技能数据

方法 B - 使用工具自动创建：
1. 选中预制体
2. Tools → Battle Demo → Check Model Import
3. 点击 "自动创建 CreatureData 配置"
4. 在生成的数据资产中补充详细信息
```

### 第六步：测试战斗
```
1. 打开 BattleScene
2. 找到 BattleManager GameObject
3. 将 Player Creature Data 替换为 Haruhi_Data
4. 运行场景测试
```

---

## 🔧 常见问题解决

### 问题 1: 模型太大/太小
```
解决方案：
1. 选中模型文件 → Inspector → Model
2. 调整 Scale Factor (通常 0.01 或 1)
3. 确保高度约 1.6m (女性角色标准)
```

### 问题 2: 动画不播放
```
解决方案：
1. 确认 Rig 设置为 Humanoid 并 Apply
2. 检查 Animator Controller 是否分配
3. 使用 Window → Animation 查看是否有动画片段
```

### 问题 3: 材质显示粉色
```
解决方案：
1. 缺少 Shader，安装 UnityChanToonShader
2. 或临时改为 Standard/URP Lit Shader
3. 重新分配纹理贴图
```

### 问题 4: 骨骼方向错误
```
解决方案：
1. 在 Rig 选项卡点击 Configure
2. 检查骨骼映射是否正确
3. 必要时手动调整 T-Pose
```

---

## 🎨 风格统一建议

如果凉宫春日模型与其他角色风格不一致：

### 方案 A: 统一使用卡通渲染
```
1. 安装 UnityChanToonShader 2.0
   - GitHub: https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project
2. 将所有角色材质改为 UTSL/ToonOutline
3. 调整光照参数保持一致
```

### 方案 B: 后处理统一色调
```
1. 在 Post-process Volume 添加：
   - Color Adjustments (统一饱和度/对比度)
   - Lookup Table (应用统一 LUT)
```

---

## 📋 推荐技能配置示例

为凉宫春日设计特色技能：

```yaml
Name: 凉宫春日
Types: [Normal, Psychic]
Ability: 神一般的存在感 (隐藏特性)

Skills:
  - 名字：漫无止境的八月
    Type: Psychic
    Power: 90
    Effect: 连续攻击 2-5 次
    
  - 名字：SOS 团召集令
    Type: Normal
    Power: 0
    Effect: 提升队友攻击力
    
  - 名字：闭锁空间
    Type: Psychic
    Power: 80
    Effect: 30% 概率使对手混乱
    
  - 名字：竹叶狂想曲
    Type: Normal
    Power: 70
    Effect: 先制攻击 +1
```

---

## ✅ 检查清单

- [ ] 模型来源合法，许可证允许游戏使用
- [ ] 格式为 .fbx 或 .unitypackage
- [ ] Rig 设置为 Humanoid
- [ ] 高度适中 (1.5m - 1.7m)
- [ ] 材质正常显示
- [ ] Animator 组件存在
- [ ] 创建了 CreatureData 配置
- [ ] 技能配置完成
- [ ] 战斗测试通过

---

## 📚 相关资源

- **UnityChanToonShader**: https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project
- **Mixamo 动画**: https://www.mixamo.com (免费动画库)
- **VRoid Studio**: https://vroid.pixiv.net (自制动漫角色)
- **BOOTH 标签**: https://booth.pm/ja/tags/3D%E3%83%A2%E3%83%87%E3%83%AB

---

**祝你在 SOS 团的冒险愉快！🎮**

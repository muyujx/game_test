using UnityEngine;
using UnityEditor;

public class ModelImportChecker : EditorWindow
{
    private GameObject selectedModel;

    [MenuItem("Tools/Battle Demo/Check Model Import")]
    public static void ShowWindow()
    {
        GetWindow<ModelImportChecker>("Model Import Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("凉宫春日模型导入检查", EditorStyles.boldLabel);
        GUILayout.Space(10);

        selectedModel = (GameObject)EditorGUILayout.ObjectField("选中的模型预制体", selectedModel, typeof(GameObject), true);

        if (selectedModel != null)
        {
            GUILayout.Space(10);
            
            // 检查 Animator
            var animator = selectedModel.GetComponent<Animator>();
            if (animator == null)
            {
                EditorGUILayout.HelpBox("❌ 缺少 Animator 组件！\n请在模型根节点添加 Animator，并将 Avatar 设置为生成的 Humanoid Avatar。", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("✅ Animator 组件存在", MessageType.None);
                
                if (animator.avatar == null)
                {
                    EditorGUILayout.HelpBox("⚠️ Avatar 未分配！\n请确保模型 Rig 类型设置为 Humanoid 并应用。", MessageType.Warning);
                }
                else if (!animator.avatar.isHuman)
                {
                    EditorGUILayout.HelpBox("⚠️ Avatar 不是 Humanoid 类型！\n这可能导致动画重定向失败。", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("✅ Avatar 配置正确 (Humanoid)", MessageType.Success);
                }
            }

            // 检查缩放
            float height = GetModelHeight(selectedModel);
            if (height < 1.0f || height > 2.5f)
            {
                EditorGUILayout.HelpBox($"⚠️ 模型高度异常 ({height:F2}m)。\n标准角色高度应在 1.5m - 1.8m 之间。请检查导入缩放设置。", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox($"✅ 模型高度正常 ({height:F2}m)", MessageType.None);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("自动创建 CreatureData 配置"))
            {
                CreateCreatureDataForModel(selectedModel);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请在Hierarchy或Project窗口选择一个模型预制体。", MessageType.Info);
        }
    }

    private float GetModelHeight(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0;

        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (var r in renderers)
        {
            if (r.bounds.min.y < min) min = r.bounds.min.y;
            if (r.bounds.max.y > max) max = r.bounds.max.y;
        }

        return max - min;
    }

    private void CreateCreatureDataForModel(GameObject model)
    {
        string savePath = "Assets/Resources/Creatures";
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Creatures");
        }

        string assetPath = $"{savePath}/{model.name}_Data.asset";
        
        CreatureData data = ScriptableObject.CreateInstance<CreatureData>();
        data.creatureName = model.name.Replace("(Clone)", "").Trim();
        data.baseHP = 80;
        data.baseAttack = 70;
        data.baseDefense = 60;
        data.baseSpAtk = 80;
        data.baseSpDef = 60;
        data.baseSpeed = 90;
        data.primaryType = ElementType.Normal; // 默认普通系，可手动修改
        
        // 这里需要你在Inspector中手动关联预制体，因为ScriptableObject不能直接依赖场景对象
        AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✅ 已创建数据资产：{assetPath}\n请在 Inspector 中将 '{model.name}' 拖入该资产的 'Model Prefab' 字段。");
        Selection.activeObject = data;
    }
}

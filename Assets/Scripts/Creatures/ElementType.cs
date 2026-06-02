namespace PokemonBattle.Creatures
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum ElementType
    {
        Normal,   // 一般
        Fire,     // 火
        Water,    // 水
        Grass,    // 草
        Electric, // 电
        Ice,      // 冰
        Fighting, // 格斗
        Poison,   // 毒
        Ground,   // 地面
        Flying,   // 飞行
        Psychic,  // 超能力
        Bug,      // 虫
        Rock,     // 岩石
        Ghost,    // 幽灵
        Dragon,   // 龙
        Steel,    // 钢
        Dark,     // 恶
        Fairy     // 妖精
    }

    /// <summary>
    /// 属性克制关系数据
    /// 倍率说明：2.0=效果绝佳，1.0=一般，0.5=效果不好，0=无效
    /// </summary>
    public static class TypeChart
    {
        // 简化版克制关系 (火草水循环 + 部分扩展)
        private static readonly float[,] effectiveness = new float[25, 25];

        static TypeChart()
        {
            // 初始化所有为 1.0
            for (int i = 0; i < 25; i++)
                for (int j = 0; j < 25; j++)
                    effectiveness[i, j] = 1.0f;

            // 火系克制
            SetEffectiveness(ElementType.Fire, ElementType.Grass, 2.0f);
            SetEffectiveness(ElementType.Fire, ElementType.Ice, 2.0f);
            SetEffectiveness(ElementType.Fire, ElementType.Bug, 2.0f);
            SetEffectiveness(ElementType.Fire, ElementType.Steel, 2.0f);
            SetEffectiveness(ElementType.Fire, ElementType.Water, 0.5f);
            SetEffectiveness(ElementType.Fire, ElementType.Rock, 0.5f);
            SetEffectiveness(ElementType.Fire, ElementType.Dragon, 0.5f);

            // 水系克制
            SetEffectiveness(ElementType.Water, ElementType.Fire, 2.0f);
            SetEffectiveness(ElementType.Water, ElementType.Ground, 2.0f);
            SetEffectiveness(ElementType.Water, ElementType.Rock, 2.0f);
            SetEffectiveness(ElementType.Water, ElementType.Grass, 0.5f);
            SetEffectiveness(ElementType.Water, ElementType.Electric, 0.5f);
            SetEffectiveness(ElementType.Water, ElementType.Dragon, 0.5f);

            // 草系克制
            SetEffectiveness(ElementType.Grass, ElementType.Water, 2.0f);
            SetEffectiveness(ElementType.Grass, ElementType.Ground, 2.0f);
            SetEffectiveness(ElementType.Grass, ElementType.Rock, 2.0f);
            SetEffectiveness(ElementType.Grass, ElementType.Fire, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Grass, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Poison, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Flying, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Bug, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Dragon, 0.5f);
            SetEffectiveness(ElementType.Grass, ElementType.Steel, 0.5f);

            // 电系克制
            SetEffectiveness(ElementType.Electric, ElementType.Water, 2.0f);
            SetEffectiveness(ElementType.Electric, ElementType.Flying, 2.0f);
            SetEffectiveness(ElementType.Electric, ElementType.Electric, 0.5f);
            SetEffectiveness(ElementType.Electric, ElementType.Grass, 0.5f);
            SetEffectiveness(ElementType.Electric, ElementType.Dragon, 0.5f);
            SetEffectiveness(ElementType.Electric, ElementType.Ground, 0.0f);

            // 冰系克制
            SetEffectiveness(ElementType.Ice, ElementType.Grass, 2.0f);
            SetEffectiveness(ElementType.Ice, ElementType.Ground, 2.0f);
            SetEffectiveness(ElementType.Ice, ElementType.Flying, 2.0f);
            SetEffectiveness(ElementType.Ice, ElementType.Dragon, 2.0f);
            SetEffectiveness(ElementType.Ice, ElementType.Fire, 0.5f);
            SetEffectiveness(ElementType.Ice, ElementType.Water, 0.5f);
            SetEffectiveness(ElementType.Ice, ElementType.Ice, 0.5f);
            SetEffectiveness(ElementType.Ice, ElementType.Steel, 0.5f);
        }

        private static void SetEffectiveness(ElementType attack, ElementType defense, float multiplier)
        {
            effectiveness[(int)attack, (int)defense] = multiplier;
        }

        /// <summary>
        /// 获取属性克制倍率
        /// </summary>
        public static float GetEffectiveness(ElementType attackType, ElementType defenseType)
        {
            return effectiveness[(int)attackType, (int)defenseType];
        }

        /// <summary>
        /// 获取双重属性的克制倍率 (相乘)
        /// </summary>
        public static float GetEffectiveness(ElementType attackType, ElementType defenseType1, ElementType defenseType2)
        {
            return GetEffectiveness(attackType, defenseType1) * GetEffectiveness(attackType, defenseType2);
        }
    }
}

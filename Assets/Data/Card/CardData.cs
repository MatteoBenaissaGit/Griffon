using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum ConsumptionType
    {
        Food,
        Drink,
    }
    
    public enum BadHabitType
    {
        Fight,
        Loud,
        Smell,
        None 
    }

    
    [Serializable]
    public struct Condition
    {
        [field:SerializeField] public ConditionType Type { get; private set; }
        [field:SerializeField] public Operator Operator { get; private set; }
        [field:SerializeField] public int Value { get; private set; }
    }
    
    public enum ConditionType
    {
        DrinkOnBar,
        FoodOnBar,
        DrinkInHostel,
        FoodInHostel,
        GoblinInHostel,
        DrinkUpOrDown,
        DrinkUpAndDown,
        FoodUpOrDown,
        FoodUpAndDown,
        LoudUpAndDown,
        LoudUpOrDown,
        DrinkComparedToFoodInHostel,
        DrinkComparedToFoodOnBar,
        FloorInHostel,
    }

    public enum Operator
    {
        Less,
        LessOrEqual,
        Equal,
        GreaterOrEqual,
        Greater,
        First,
        BeforeLast,
        Last,
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardData", order = 1)]
    public class CardData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public ConsumptionType Consumption { get; private set; }
        [field: SerializeField] public BadHabitType BadHabit { get; private set; }
        [field: SerializeField] public int BadHabitAmount { get; private set; } = 1;
        [field: SerializeField] public List<Condition> HostelConditions { get; private set; }
        [field: SerializeField] public BadHabitType BarCondition { get; private set; } = BadHabitType.None;
    }
}

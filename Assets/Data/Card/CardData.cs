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

    public enum LeaveWithCondition
    {
        Nobody = 0,
        ThoseWhoDontLikeFight = 1,
        ThoseWhoDontLikeLoud = 2,
        ThoseWhoDontLikeSmell = 3,
        AllGoblins = 4,
        ClientBeneath = 5,
        ClientBeneath2Floor = 6,
        DrawCardFromTankAndSwitch = 7,
        Take3ClientBeneathAndReplaceThem = 8,
        ShuffleBarAndPlaceBackToTank = 9,
        DrawFromTankToTopOfHostel = 10,
        DrawFromBarToTopOfHostel = 11,
        DrawFirstInBarToCardHostelFlor = 12,
        DestroyNextCardInTank = 13
    }
    
    public enum CardLeaveEffect
    {
        Destroyed = 0,
        GoInTank = 1,
        GoInBar = 2,
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
        [field: Header("Leave effect"), SerializeField] public LeaveWithCondition LeaveWith { get; private set; } = LeaveWithCondition.Nobody;
        [field: SerializeField] public bool InvokeEffectOfLeavers { get; private set; }
        [field: SerializeField] public CardLeaveEffect CardWhenLeave { get; private set; } = CardLeaveEffect.Destroyed;
    }
}

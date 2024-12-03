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
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardData", order = 1)]
    public class CardData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public ConsumptionType Consumption { get; private set; }
        [field: SerializeField] public BadHabitType BadHabit { get; private set; }
        [field: SerializeField] public int BadHabitAmount { get; private set; } = 1;
    }
}

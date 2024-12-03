using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameSpritesData", order = 1)]
    public class GameSpritesData : ScriptableObject
    {
        [field:SerializeField] public Sprite Food { get; private set; }
        [field:SerializeField] public Sprite Drink { get; private set; }
        [field:SerializeField] public Sprite Fight { get; private set; }
        [field:SerializeField] public Sprite Loud { get; private set; }
        [field:SerializeField] public Sprite Smell { get; private set; }
        
        public Sprite GetConsumptionSprite(ConsumptionType consumptionType)
        {
            return consumptionType switch
            {
                ConsumptionType.Food => Food,
                ConsumptionType.Drink => Drink,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
        
        public Sprite GetBadHabitSprite(BadHabitType badHabitType)
        {
            return badHabitType switch
            {
                BadHabitType.Fight => Fight,
                BadHabitType.Loud => Loud,
                BadHabitType.Smell => Smell,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
    }
}
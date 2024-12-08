using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public class Hostel : MonoBehaviour
{
    [SerializeField] private Transform _hostelStartPoint;
    [SerializeField] private float _cardsHeightDifference = 1f;

    private List<Card> _cards = new();
    
    public async void AddCard(Card card)
    {
        _cards.Add(card);
        card.SetOrderInLayer(_cards.Count);
        card.transform.parent = transform;

        Vector3 position = _hostelStartPoint.position + Vector3.up * (_cards.Count - 1) * _cardsHeightDifference;
        card.transform.DOComplete();
        card.transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);

        SetOrdersInLayer();
        
        await Task.Delay(500);
        await CheckCardsHostelConditions();
    }
    
    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
        Destroy(card);
        
        PlaceAllCards();
    }

    private void PlaceAllCards()
    {
        Card[] cardArray = _cards.ToArray();
        
        float heightDifference = _cardsHeightDifference;
        if (cardArray.Length > 7)
        {
            heightDifference *= 0.75f;
        }
        
        for (int i = 0; i < _cards.Count; i++)
        {
            Vector3 position = _hostelStartPoint.position + Vector3.up * i * heightDifference;
            
            cardArray[i].SetOrderInLayer(_cards.Count-i);
            cardArray[i].transform.DOComplete();
            cardArray[i].transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);
        }
    }

    private void SetOrdersInLayer()
    {
        Card[] cardArray = _cards.ToArray();
        for (int i = 0; i < _cards.Count; i++)
        {
            cardArray[i].SetOrderInLayer(_cards.Count-i);
        }
    }

    private async Task CheckCardsHostelConditions()
    {
        Debug.Log("check hostel conditions");
        
        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            var card = _cards[i];
            
            card.transform.DOComplete();
            card.transform.DOPunchScale(Vector3.one * 0.2f, 0.35f);

            if (ShouldCardLeaveFromHostel(_cards[i], i))
            {
                card.LeaveFromHostel();
                _cards.Remove(card);
                PlaceAllCards();
                await Task.Delay(1000);
                await CheckCardsHostelConditions();
                return;
            }
            
            await Task.Delay(100);
        }
        
        await Task.Delay(1000);
        
        GameManager.Instance.EndHostelTurn();
    }

    private bool ShouldCardLeaveFromHostel(Card card, int floor)
    {
        //drink & food in hostel
        int amountOfFoodInHostel = 0;
        int amountOfDrinkInHostel = 0;
        int amountOfGoblinsInHostel = 0;
        int drinkUpsAndDowns = 0;
        int foodUpsAndDowns = 0;
        int loudUpsAndDowns = 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].Data.Consumption == ConsumptionType.Food)
            {
                amountOfFoodInHostel++;
                if (i == floor-1 || i == floor+1)
                {
                    foodUpsAndDowns++;
                }
            }
            else
            {
                amountOfDrinkInHostel++;
                if (i == floor-1 || i == floor+1)
                {
                    drinkUpsAndDowns++;
                }
            }

            if (card.Data.BadHabit == BadHabitType.Loud && (i == floor - 1 || i == floor + 1))
            {
                loudUpsAndDowns += _cards[i].Data.BadHabitAmount;
            }

            if (_cards[i].Data.HostelConditions.Any(x => x.Type == ConditionType.GoblinInHostel))
            {
                amountOfGoblinsInHostel++;
            }
        }
        
        //drink & food in bar
        int amountOfFoodInBar = 0;
        int amountOfDrinkInBar = 0;
        Bar bar = GameManager.Instance.CardBar;
        foreach (Card cardInBar in bar.Cards)
        {
            if (cardInBar.Data.Consumption == ConsumptionType.Food)
            {
                amountOfFoodInBar++;
            }
            else
            {
                amountOfDrinkInBar++;
            }
        }

        foreach (var condition in card.Data.HostelConditions)
        {
            int value = condition.Value;
            switch (condition.Type)
            {
                case ConditionType.DrinkOnBar:
                    return IsOperatorConditionMet(amountOfDrinkInBar, condition.Operator, value);
                case ConditionType.FoodOnBar:
                    return IsOperatorConditionMet(amountOfFoodInBar, condition.Operator, value);
                case ConditionType.DrinkInHostel:
                    return IsOperatorConditionMet(amountOfDrinkInHostel, condition.Operator, value);
                case ConditionType.FoodInHostel:
                    return IsOperatorConditionMet(amountOfFoodInHostel, condition.Operator, value);
                case ConditionType.GoblinInHostel:
                    return IsOperatorConditionMet(amountOfGoblinsInHostel, condition.Operator, value);
                case ConditionType.DrinkUpOrDown:
                    return drinkUpsAndDowns > 0;
                case ConditionType.DrinkUpAndDown:
                    return drinkUpsAndDowns >= value;
                case ConditionType.FoodUpOrDown:
                    return foodUpsAndDowns > 0;
                case ConditionType.FoodUpAndDown:
                    return foodUpsAndDowns >= value;
                case ConditionType.LoudUpOrDown:
                    return loudUpsAndDowns > 0;
                case ConditionType.LoudUpAndDown:
                    return loudUpsAndDowns >= value;
                case ConditionType.DrinkComparedToFoodInHostel:
                    return IsOperatorConditionMet(amountOfDrinkInHostel, condition.Operator, amountOfFoodInHostel);
                case ConditionType.DrinkComparedToFoodOnBar:
                    return IsOperatorConditionMet(amountOfDrinkInBar, condition.Operator, amountOfFoodInBar);
                case ConditionType.FloorInHostel:
                    int floorAmount = _cards.Count;
                    switch (condition.Operator)
                    {
                        case Operator.First:
                            return floor == 0;
                        case Operator.BeforeLast:
                            return floor == floorAmount - 2;
                        case Operator.Last:
                            return floor == floorAmount - 1;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return false;
    }

    private static bool IsOperatorConditionMet(int value, Operator @operator, int amount)
    {
        switch (@operator)
        {
            case Operator.Less:
                return value < amount;
            case Operator.LessOrEqual:
                return value <= amount;
            case Operator.Equal:
                return value == amount;
            case Operator.GreaterOrEqual:
                return value >= amount;
            case Operator.Greater:
                return value > amount;
        }
        throw new ArgumentOutOfRangeException($"{value} {@operator} {amount}");
    }

    public async Task CheckCardsBarConditions(BadHabitType badHabit)
    {
        Debug.Log("check bar conditions");
        
        await Task.Delay(1000);
        
        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            var card = _cards[i];
            
            card.transform.DOComplete();
            card.transform.DOPunchScale(Vector3.one * 0.2f, 0.35f);

            await Task.Delay(100);
            
            if (card.Data.BarCondition == BadHabitType.None) continue;

            if (badHabit == card.Data.BarCondition)
            {
                _cards.Remove(card);
                card.LeaveFromHostel();
                PlaceAllCards();
                await Task.Delay(500);
            }
        }
        
        await Task.Delay(1000);
    }
}
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
                await MakeCardLeave(card, i);
                return;
            }
            
            await Task.Delay(100);
        }
        
        await Task.Delay(1000);
        
        GameManager.Instance.EndHostelTurn();
    }

    private async Task MakeCardLeave(Card card, int floor)
    {
        GameManager.Instance.CanPlayerMakeAction = false;

        Debug.Log($"{card.Data.Name} leaved");

        //apply effect that make other leaves
        List<(Card cardToRemove, int floor)> toRemove = new();
        for (int i = 0; i < _cards.Count; i++)
        {
            if (i == floor) continue;
            
            var cardInHostel = _cards[i];
            switch (cardInHostel.Data.BarCondition)
            {
                case BadHabitType.Fight when card.Data.LeaveWith == LeaveWithCondition.ThoseWhoDontLikeFight:
                case BadHabitType.Loud when card.Data.LeaveWith == LeaveWithCondition.ThoseWhoDontLikeLoud:
                case BadHabitType.Smell when card.Data.LeaveWith == LeaveWithCondition.ThoseWhoDontLikeSmell:
                    toRemove.Add((cardInHostel, i));
                    break;
            }
            if (cardInHostel.Data.HostelConditions.Any(x => x.Type == ConditionType.GoblinInHostel) && card.Data.LeaveWith == LeaveWithCondition.AllGoblins)
            {
                toRemove.Add((cardInHostel, i));
            }
            if (i == floor - 1 && card.Data.LeaveWith == LeaveWithCondition.ClientBeneath)
            {
                toRemove.Add((cardInHostel, i));
            }
            if (i == floor - 2 && card.Data.LeaveWith == LeaveWithCondition.ClientBeneath2Floor)
            {
                toRemove.Add((cardInHostel, i));
            }
        }
        
        //other effects
        var tank = GameManager.Instance.Tank;
        var bar = GameManager.Instance.CardBar;
        bool cancelLeave = false;
        switch (card.Data.LeaveWith)
        {
            case LeaveWithCondition.DrawCardFromTankAndSwitch:
                //get from tank in hostel
                var c1 = tank.GetRandomCard();
                tank.RemoveCard(c1);
                c1.CanBePreviewed = true;
                AddCardAtPosition(c1,floor+1);
                //this one goes in tank
                cancelLeave = true;
                tank.AddCardInTank(card);
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.Take3ClientBeneathAndReplaceThem:
                //get 3 cards beneath
                List<Card> beneathCards = new();
                for (int i = floor - 1; i >= 0; i--)
                {
                    beneathCards.Add(_cards[i]);
                    if (beneathCards.Count == 3)
                    {
                        break;
                    }
                }
                //shuffle and replace
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.ShuffleBarAndPlaceBackToTank:
                bar.PlaceBackAllCardsInTank();
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.DrawFromTankToTopOfHostel:
                var c2 = tank.GetRandomCard();
                c2.CanBePreviewed = true;
                tank.PlaceCardInHostel(c2, this);
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.DrawFromBarToTopOfHostel:
                var c3 = bar.GetRandomCard();
                bar.PlaceCardInHostel(c3, this);
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.DrawFirstInBarToCardHostelFlor:
                var c4 = bar.GetFirstCard();
                bar.RemoveCard(c4);
                AddCardAtPosition(c4, floor+1);
                await Task.Delay(1000);
                break;
            case LeaveWithCondition.DestroyNextCardInTank:
                if (tank.ExtractCardFromTank(out Card c5))
                {
                    c5.LeaveFromTank();
                }
                await Task.Delay(1000);
                break;
        }

        //remove card
        if (cancelLeave == false)
        {
            card.LeaveFromHostel(true);
        }
        _cards.Remove(card);
        PlaceAllCards();

        //remove to leave cards
        foreach ((Card cardToRemove, int floor) toRemoveCard in toRemove)
        {
            if (card.Data.InvokeEffectOfLeavers)
            {
                await MakeCardLeave(toRemoveCard.cardToRemove, toRemoveCard.floor);
            }
            else
            {
                _cards.Remove(toRemoveCard.cardToRemove);
                toRemoveCard.cardToRemove.LeaveFromHostel(false);
            }
        }

        //check conditions again
        PlaceAllCards();
        await Task.Delay(1000);
        await CheckCardsHostelConditions();
        
        GameManager.Instance.CanPlayerMakeAction = true;
    }

    private void AddCardAtPosition(Card card, int floor)
    {
        _cards.Insert(floor, card);
        PlaceAllCards();
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

        bool shouldLeave = false;
        foreach (var condition in card.Data.HostelConditions)
        {
            int value = condition.Value;
            switch (condition.Type)
            {
                case ConditionType.DrinkOnBar:
                    shouldLeave = IsOperatorConditionMet(amountOfDrinkInBar, condition.Operator, value); break;
                case ConditionType.FoodOnBar:
                    shouldLeave = IsOperatorConditionMet(amountOfFoodInBar, condition.Operator, value); break;
                case ConditionType.DrinkInHostel:
                    shouldLeave = IsOperatorConditionMet(amountOfDrinkInHostel, condition.Operator, value); break;
                case ConditionType.FoodInHostel:
                    shouldLeave = IsOperatorConditionMet(amountOfFoodInHostel, condition.Operator, value); break;
                case ConditionType.GoblinInHostel:
                    shouldLeave = IsOperatorConditionMet(amountOfGoblinsInHostel, condition.Operator, value); break;
                case ConditionType.DrinkUpOrDown:
                    shouldLeave = drinkUpsAndDowns > 0; break;
                case ConditionType.DrinkUpAndDown:
                    shouldLeave = drinkUpsAndDowns >= value; break;
                case ConditionType.FoodUpOrDown:
                    shouldLeave = foodUpsAndDowns > 0; break;
                case ConditionType.FoodUpAndDown:
                    shouldLeave = foodUpsAndDowns >= value; break;
                case ConditionType.LoudUpOrDown:
                    shouldLeave = loudUpsAndDowns > 0; break;
                case ConditionType.LoudUpAndDown:
                    shouldLeave = loudUpsAndDowns >= value; break;
                case ConditionType.DrinkComparedToFoodInHostel:
                    shouldLeave = IsOperatorConditionMet(amountOfDrinkInHostel, condition.Operator, amountOfFoodInHostel); break;
                case ConditionType.DrinkComparedToFoodOnBar:
                    shouldLeave = IsOperatorConditionMet(amountOfDrinkInBar, condition.Operator, amountOfFoodInBar); break;
                case ConditionType.FloorInHostel:
                    int floorAmount = _cards.Count;
                    bool checkOperator = true;
                    switch (condition.Operator)
                    {
                        case Operator.First:
                            shouldLeave = floor == 0;
                            checkOperator = false;
                            break;
                        case Operator.BeforeLast:
                            checkOperator = false;
                            shouldLeave = floor == floorAmount - 2; 
                            break;
                        case Operator.Last:
                            checkOperator = false;
                            shouldLeave = floor == floorAmount - 1; 
                            break;
                    }

                    if (checkOperator)
                    {
                        shouldLeave = IsOperatorConditionMet(floor+1, condition.Operator, value); break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (shouldLeave)
            {
                return true;
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
                await MakeCardLeave(card, i);
            }
        }
        
        await Task.Delay(1000);
    }
}
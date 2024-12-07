using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
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
        card.transform.DOKill();
        card.transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);
        
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
        for (int i = 0; i < _cards.Count; i++)
        {
            Vector3 position = _hostelStartPoint.position + Vector3.up * i * _cardsHeightDifference;
            
            cardArray[i].SetOrderInLayer(_cards.Count-i);
            cardArray[i].transform.DOKill();
            cardArray[i].transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);
        }
    }

    private async Task CheckCardsHostelConditions()
    {
        Debug.Log("check hostel conditions");
        
        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            _cards[i].transform.DOKill();
            _cards[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.35f);
         
            //CHECK THE CONDITIONS HERE
            
            await Task.Delay(100);
        }
        
        await Task.Delay(1000);
        
        GameManager.Instance.EndHostelTurn();
    }
    
    public async Task CheckCardsBarConditions(BadHabitType badHabit)
    {
        Debug.Log("check bar conditions");
        
        await Task.Delay(1000);
        
        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            var card = _cards[i];
            
            card.transform.DOKill();
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
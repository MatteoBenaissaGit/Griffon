using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] private Transform _tankStartPoint;
    [SerializeField] private float _cardsHeightDifference = 1f;

    private Queue<Card> _tank = new();

    public void AddCardInTank(Card card)
    {
        _tank.Enqueue(card);
        card.transform.position = _tankStartPoint.position + Vector3.up * 20f;
        PlaceCardInTank(card, _tank.Count);
    }

    public bool ExtractCardFromTank(out Card card)
    {
        card = null;
        if (_tank.Count <= 0) return false;

        card = _tank.Dequeue();
        card.transform.parent = null;
        PlaceAllCards();
        return true;
    }
    
    private void PlaceCardInTank(Card card, int count)
    {
        Vector3 position = _tankStartPoint.position + (Vector3.up * count * _cardsHeightDifference);

        card.SetOrderInLayer(count);
        
        card.transform.parent = transform;
        card.transform.DOComplete();
        card.transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);
    }

    private void PlaceAllCards()
    {
        int count = 0;
        foreach (Card card in _tank)
        {
            PlaceCardInTank(card, count);
            count++;
        }
    }
}

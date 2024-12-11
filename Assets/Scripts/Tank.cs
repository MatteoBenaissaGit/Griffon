using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] private Transform _tankStartPoint;
    [SerializeField] private float _cardsHeightDifference = 1f;

    private Queue<Card> _tank = new();

    public void AddCardInTank(Card card, bool atRandomPosition = false)
    {
        card.CanBePreviewed = false;
        
        if (atRandomPosition)
        {
            var list = _tank.ToList();
            list.Insert(Random.Range(0, list.Count), card);
            _tank = new Queue<Card>(list);
        }
        else
        {
            _tank.Enqueue(card);
            card.transform.position = _tankStartPoint.position + Vector3.up * 20f;
            PlaceCardInTank(card, _tank.Count);
        }
        
        PlaceAllCards();
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

    public void RemoveCard(Card card)
    {
        List<Card> list = _tank.ToList();
        list.Remove(card);
        _tank = new Queue<Card>(list);
        
        PlaceAllCards();
    }
    
    public Card GetRandomCard()
    {
        return _tank.ElementAt(Random.Range(0, _tank.Count));
    }
    
    public void PlaceCardInHostel(Card cardInHostel, Hostel hostel)
    {
        RemoveCard(cardInHostel);
        
        PlaceAllCards();
            
        hostel.AddCard(cardInHostel);
    }
}

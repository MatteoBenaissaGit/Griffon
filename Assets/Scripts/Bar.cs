using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private float _cardsWidthDifference = 1f;
        
        public const int Capacity = 4;
        public bool IsFull => _cards.Count >= Capacity;

        private List<Card> _cards = new();
        
        public void AddCard(Card card)
        {
            _cards.Add(card);
            PlaceAllCards();

            card.CanBePreviewed = true;
        }

        private void PlaceAllCards()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                Vector3 position = transform.position + Vector3.right * i * _cardsWidthDifference;
                _cards[i].transform.DOKill();
                _cards[i].transform.DOMove(position, 0.5f).SetEase(Ease.OutSine);
            }
        }

        public void StartBarTurn()
        {
            foreach (Card card in _cards)
            {
                card.SetSelectable(() =>
                {
                    PlaceCardInHostel(card, GameManager.Instance.CardHostel);
                    GameManager.Instance.StartHostelTurn();
                });
            }
        }

        public void PlaceCardInHostel(Card cardInHostel, Hostel hostel)
        {
            foreach (Card card in _cards)
            {
                card.SetUnselectable();
            }
            
            _cards.Remove(cardInHostel);
            PlaceAllCards();
            
            hostel.AddCard(cardInHostel);
        }
    }
}
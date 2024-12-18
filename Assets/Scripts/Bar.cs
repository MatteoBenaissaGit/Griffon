﻿using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private float _cardsWidthDifference = 1f;
        
        public const int Capacity = 4;
        public bool IsFull => _cards.Count >= Capacity;

        public List<Card> Cards => _cards;
        private List<Card> _cards = new();
        
        public void AddCard(Card card)
        {
            _cards.Add(card);
            PlaceAllCards();

            card.CanBePreviewed = true;

            List<Card> loud = new();
            List<Card> smell = new();
            List<Card> fight = new();
            int loudAmount = 0;
            int smellAmount = 0;
            int fightAmount = 0;
            foreach (Card c in _cards)
            {
                switch (c.Data.BadHabit)
                {
                    case BadHabitType.Loud:
                        loudAmount += c.Data.BadHabitAmount;
                        loud.Add(c);
                        break;
                    case BadHabitType.Smell:
                        smell.Add(c);
                        smellAmount += c.Data.BadHabitAmount;
                        break;
                    case BadHabitType.Fight:
                        fight.Add(c);
                        fightAmount += c.Data.BadHabitAmount;
                        break;
                }
            }
            if (loudAmount >= 3)
            {
                loud.ForEach(x => x.MakeBadHabitFeedback());
                GameManager.Instance.CheckBarConditionsFor(BadHabitType.Loud);
            }
            if (smellAmount >= 3)
            {
                smell.ForEach(x => x.MakeBadHabitFeedback());
                GameManager.Instance.CheckBarConditionsFor(BadHabitType.Smell);
            }
            if (fightAmount >= 3)
            {
                fight.ForEach(x => x.MakeBadHabitFeedback());
                GameManager.Instance.CheckBarConditionsFor(BadHabitType.Fight);
            }
        }

        private void PlaceAllCards()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                Vector3 position = transform.position + Vector3.right * i * _cardsWidthDifference;
                _cards[i].transform.DOComplete();
                _cards[i].transform.DOMove(position, 0.3f).SetEase(Ease.OutSine);
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

        public void PlaceCardInHostel(Card cardInHostel, Hostel hostel, bool checkConditions = true)
        {
            foreach (Card card in _cards)
            {
                card.SetUnselectable();
            }
            
            _cards.Remove(cardInHostel);
            PlaceAllCards();

            hostel.AddCard(cardInHostel, checkConditions); 
        }
        
        public void PlaceBackAllCardsInTank()
        {
            foreach (Card card in _cards)
            {
                GameManager.Instance.Tank.AddCardInTank(card, true);
            }
            _cards.Clear();
        }

        public Card GetRandomCard() => _cards[Random.Range(0, _cards.Count)];
        public Card GetFirstCard() => _cards[0];

        public void RemoveCard(Card cardToRemove)
        {
            _cards.Remove(cardToRemove);
        }
    }
}
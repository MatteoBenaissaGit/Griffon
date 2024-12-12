using System;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _cardShadow;
    [SerializeField] private SpriteRenderer _cardFeedback;
    [SerializeField] private SpriteRenderer _consumptionSprite;
    [SerializeField] private SpriteRenderer _badHabitSprite;
    [SerializeField] private SpriteRenderer _secondBadHabitSprite;
    [SerializeField] private SpriteRenderer _characterSprite;
    [SerializeField] private Color _feedbackColor;

    public bool CanBePreviewed { get; set; }
    public CardData Data { get; private set; }
    
    private bool _canBeSelected = false;
    private Action _onSelectedAction;
    
    private void Start()
    {
        SetFeedback(false, 0);
    }

    public void Initialize(CardData data)
    {
        Data = data;
        
        _consumptionSprite.sprite = GameManager.Instance.SpritesData.GetConsumptionSprite(Data.Consumption);
        _badHabitSprite.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(Data.BadHabit);
        if (Data.BadHabitAmount > 1)
        {
            _secondBadHabitSprite.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(Data.BadHabit);
        }
        else
        {
            _secondBadHabitSprite.gameObject.SetActive(false);
        }

        _characterSprite.sprite = Data.Sprite;
    }

    public void SetSelectable(Action onSelectedAction)
    {
        _onSelectedAction = onSelectedAction;
        _canBeSelected = true;
    }

    public void SetUnselectable()
    {
        _canBeSelected = false;
    }
    
    public void SetOrderInLayer(int order)
    {
        order *= 10;
        
        _cardFeedback.sortingOrder = 100 + order;
        _cardFront.sortingOrder = 2 + order;
        _consumptionSprite.sortingOrder = 3 + order;
        _badHabitSprite.sortingOrder = 4 + order;
        _secondBadHabitSprite.sortingOrder = 3 + order;
        _characterSprite.sortingOrder = 3 + order;
        _cardShadow.sortingOrder = 0 + order;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.CanPlayerMakeAction == false || GameManager.Instance.EndGame) return;
        
        SetFeedback(false, 0);
        GameManager.Instance.UI.SetPreview(false, Data);

        if (_canBeSelected == false) return;
        _canBeSelected = false;
        _onSelectedAction?.Invoke();
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.CanPlayerMakeAction == false || GameManager.Instance.EndGame) return;

        if (_canBeSelected)
        {
            SetFeedback(true, 0f);
        }
        if (CanBePreviewed)
        {
            GameManager.Instance.UI.SetPreview(true, Data);
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.CanPlayerMakeAction == false || GameManager.Instance.EndGame) return;

        if (_canBeSelected)
        {
            SetFeedback(false, 0f);
        }
        
        if (CanBePreviewed)
        {
            GameManager.Instance.UI.SetPreview(false);
        }
    }

    private void SetFeedback(bool doShow, float time)
    {
        _cardFeedback.DOKill();
        _cardFeedback.DOColor(doShow ? _feedbackColor : new Color(_feedbackColor.r, _feedbackColor.g, _feedbackColor.b, 0f), time);
    }

    public void LeaveFromHostel(bool withLeaveEffect)
    {
        _canBeSelected = false;
        _onSelectedAction = null;
        SetFeedback(false, 0);

        if (withLeaveEffect)
        {
            switch (Data.CardWhenLeave)
            {
                case CardLeaveEffect.GoInTank:
                    GameManager.Instance.Tank.AddCardInTank(this);
                    return;
                case CardLeaveEffect.GoInBar:
                    GameManager.Instance.CardBar.AddCard(this);
                    return;
            }
        }

        transform.DOMoveX(transform.position.x + 5, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void LeaveFromTank()
    {
        transform.DOMoveX(transform.position.x - 5, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
    public void MakeBadHabitFeedback()
    {
         List<SpriteRenderer> sprites = new List<SpriteRenderer> { _badHabitSprite, _secondBadHabitSprite };
         foreach (var sprite in sprites)
         {
             sprite.transform.parent.DOComplete();
             var sequence = DOTween.Sequence();
                sequence.Append(sprite.transform.parent.DOScale(Vector3.one * 1.2f, 0.8f).SetEase(Ease.OutSine));
                sequence.Append(sprite.transform.parent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutSine));
         }
    }
}

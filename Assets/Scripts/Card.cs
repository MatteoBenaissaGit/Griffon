using System;
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

    private void Awake()
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
        _onSelectedAction = null;
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
        SetFeedback(false, 0);
        _canBeSelected = false;
        GameManager.Instance.UI.SetPreview(false, Data);
        
        _onSelectedAction?.Invoke();
    }

    private void OnMouseEnter()
    {
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

    public void LeaveFromHostel()
    {
        _canBeSelected = false;
        _onSelectedAction = null;
        SetFeedback(false, 0);
        
        transform.DOMoveX(transform.position.x + 5, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}

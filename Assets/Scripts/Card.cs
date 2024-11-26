using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _cardShadow;
    [SerializeField] private SpriteRenderer _cardFeedback;
    [SerializeField] private Color _feedbackColor;

    public bool CanBePreviewed { get; set; }
    
    private bool _canBeSelected = false;
    private Action _onSelectedAction;

    private void Awake()
    {
        SetFeedback(false, 0);
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
        
        _cardFront.sortingOrder = 2 + order;
        _cardFeedback.sortingOrder = 100 + order;
        _cardShadow.sortingOrder = 0 + order;
    }

    private void OnMouseDown()
    {
        SetFeedback(false, 0);
        _canBeSelected = false;
        GameManager.Instance.UI.SetPreview(false);
        
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
            GameManager.Instance.UI.SetPreview(true);
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
}

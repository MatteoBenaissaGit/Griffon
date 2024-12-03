using System;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameStateText;
        [SerializeField] private CanvasGroup _previewCanvasGroup;
        [SerializeField] private TMP_Text _previewNameText;
        [SerializeField] private Image _consumptionImage;
        [SerializeField] private Image _badHabitImage;
        [SerializeField] private Image _secondBadHabitImage;

        private void Start()
        {
            _previewCanvasGroup.alpha = 0;
            
            SetPreview(false);
        }

        public void SetGameStateText(string text)
        {
            _gameStateText.text = text;
        }

        public void SetPreview(bool doShow, CardData data = null)
        {
            _previewCanvasGroup.DOKill();
            _previewCanvasGroup.DOFade(doShow ? 1 : 0, 0.2f);

            if (doShow == false || data == null) return;
            
            _previewNameText.text = data.Name;
            _badHabitImage.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(data.BadHabit);
            if (data.BadHabitAmount > 1)
            {
                _secondBadHabitImage.gameObject.SetActive(true);
                _secondBadHabitImage.sprite = GameManager.Instance.SpritesData.GetBadHabitSprite(data.BadHabit);
            }
            else
            {
                _secondBadHabitImage.gameObject.SetActive(false);
            }
            _consumptionImage.sprite = GameManager.Instance.SpritesData.GetConsumptionSprite(data.Consumption);
        }
    }
}
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameStateText;
        [SerializeField] private Image _previewImage;

        private void Start()
        {
            SetPreview(false);
        }

        public void SetGameStateText(string text)
        {
            _gameStateText.text = text;
        }

        public void SetPreview(bool doShow)
        {
            _previewImage.DOKill();
            _previewImage.DOFade(doShow ? 1 : 0, 0.2f);
        }
    }
}
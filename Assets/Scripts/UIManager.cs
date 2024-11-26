using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameStateText;

        public void SetGameStateText(string text)
        {
            _gameStateText.text = text;
        }
    }
}
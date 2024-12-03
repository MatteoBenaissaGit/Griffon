using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }

    #endregion
    
    [field:SerializeField] public UIManager UI { get; private set; }
    [field:SerializeField] public Tank Tank { get; private set; }
    [field:SerializeField] public Card CardPrefab { get; private set; }
    [field:SerializeField] public Bar CardBar { get; private set; }
    [field:SerializeField] public Hostel CardHostel { get; private set; }
    [field:SerializeField] public GameSpritesData SpritesData { get; private set; }

    [SerializeField] private List<CardData> _cardsInTank = new();

    private async void Start()
    {
        await FillTank();
        await Task.Delay(1000);
        await SetBarTurn();
    }

    private async Task FillTank()
    {
        UI.SetGameStateText("Les clients arrivent");

        Common.Shuffle(_cardsInTank);
        foreach (CardData cardData in _cardsInTank)
        {
            if (Application.isPlaying == false) return;
            
            Card card = Instantiate(CardPrefab);
            card.Initialize(cardData);
            Tank.AddCardInTank(card);
            
            await Task.Delay(150);
        }
    }

    private async Task SetBarTurn()
    {
        UI.SetGameStateText("Choisissez un client");
        
        while (CardBar.IsFull == false && Tank.ExtractCardFromTank(out Card card))
        {
            CardBar.AddCard(card);
            await Task.Delay(300);
        }
        CardBar.StartBarTurn();
    }

    public async void EndHostelTurn()
    {
        await SetBarTurn();
    }

    public void StartHostelTurn()
    {
        UI.SetGameStateText("Vont-ils s'entendre ?");
    }
}

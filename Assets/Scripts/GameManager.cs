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

    public bool CanPlayerMakeAction { get; set; } = false;
    public bool EndGame { get; set; }
    public int ClientsLeave { get; private set; }
    
    private async void Start()
    {
        if (_currentTask != null) await _currentTask;
        
        await FillTank();
        await Task.Delay(1000);
        await SetBarTurn();

        CanPlayerMakeAction = true;
    }

    private async Task FillTank()
    {
        if (_currentTask != null) await _currentTask;
        
        UI.SetGameStateText("Clients are coming in");

        Common.Shuffle(_cardsInTank);
        foreach (CardData cardData in _cardsInTank)
        {
            if (Application.isPlaying == false) return;
            
            Card card = Instantiate(CardPrefab);
            card.Initialize(cardData);
            Tank.AddCardInTank(card);
            
            await Task.Delay(100);
        }
    }

    private async Task SetBarTurn()
    {
        if (EndGame) return;
        
        if (_currentTask != null) await _currentTask;
        
        CanPlayerMakeAction = false;

        UI.SetGameStateText("Choose a client");
        
        while (CardBar.IsFull == false && Tank.ExtractCardFromTank(out Card card))
        {
            CardBar.AddCard(card);
            await Task.Delay(300);
        }
        CardBar.StartBarTurn();
        
        if (Tank.CardCount == 0)
        {
            UI.SetEndGame(CardHostel.CardCount >= 7, false);
            EndGame = true;
            CanPlayerMakeAction = false;
            return;
        }
        
        CanPlayerMakeAction = true;
    }

    public async void EndHostelTurn()
    {
        if (EndGame) return;

        if (_currentTask != null) await _currentTask;

        await SetBarTurn();
    }

    public void StartHostelTurn()
    {
        if (EndGame) return;

        UI.SetGameStateText("Will they go along ?");
    }

    private Task _currentTask = null;
    public async void CheckBarConditionsFor(BadHabitType badHabit)
    {
        CanPlayerMakeAction = false;

        _currentTask = CardHostel.CheckCardsBarConditions(badHabit);
        await _currentTask;
        
        CanPlayerMakeAction = true;
    }

    public void AddLeaveClient()
    {
        ClientsLeave++;
        if (ClientsLeave >= 8)
        {
            UI.SetEndGame(false, true);
            EndGame = true;
            CanPlayerMakeAction = false;
        }
    }
}

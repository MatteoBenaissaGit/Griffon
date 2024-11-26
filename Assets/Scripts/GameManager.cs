using System;
using System.Threading.Tasks;
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

    private async void Start()
    {
        await FillTank(20);
        await Task.Delay(1000);
        await SetBarTurn();
    }

    private async Task FillTank(int fillAmount)
    {
        for (int i = 0; i < fillAmount; i++)
        {
            if (Application.isPlaying == false) return;
            
            Card card = Instantiate(CardPrefab);
            Tank.AddCardInTank(card);
            await Task.Delay(150);
        }
    }

    private async Task SetBarTurn()
    {
        UI.SetGameStateText("Bar phase");
        
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
        UI.SetGameStateText("Hostel phase");
    }
}

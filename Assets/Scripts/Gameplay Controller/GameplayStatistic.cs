using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayStatistic : MonoBehaviour
{
    public static GameplayStatistic Instance;
    public GameplayController gameplayController;

    [Header("Gameplay Condition")]
    public int nextKillCountToSelectNewModifier = 2;
    public float killCountIncreasePerSelect = 2;

    [Header("Player Statistic")]
    public int playerKillCount = 0;
    public int totalPlayerKillCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void Add_PlayerKillCount()
    {
        playerKillCount += 1;
        totalPlayerKillCount += 1;

        if(playerKillCount >= nextKillCountToSelectNewModifier)
        {
            playerKillCount -= nextKillCountToSelectNewModifier;
            nextKillCountToSelectNewModifier = Mathf.RoundToInt( nextKillCountToSelectNewModifier * killCountIncreasePerSelect );

            gameplayController.SetupAndShow_SkillModifierCardSelection();
        }
    }

    public void GameOver_ByPlayerDeath()
    {
        gameplayController.Show_GameoverScreen(totalPlayerKillCount);
    }
}

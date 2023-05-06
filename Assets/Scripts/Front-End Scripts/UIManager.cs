using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

[DefaultExecutionOrder(2)]
public class UIManager : NetworkBehaviour
{
    public static UIManager Instance = null;

    [Header("Common UI elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Server Only UI elements")]
    [SerializeField] private List<GameObject> serverOnlyElements;


    [Header("Create Lobby Object")]
    [SerializeField] private GameObject createObject;

    [Header("Join Lobby Object")]
    [SerializeField] private GameObject joinObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Start()
    {
        HideServerObjects();
    }

    private void HideServerObjects()
    {
        if (!OverallGameManager.Instance.isCreator)
        {
            foreach(GameObject obj in serverOnlyElements)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in serverOnlyElements)
            {
                obj.SetActive(true);
            }
        }
    }

    public void ShowServerObjects()
    {
        if (!OverallGameManager.Instance.isCreator) return;

        foreach (GameObject obj in serverOnlyElements)
        {
            obj.SetActive(true);
        }
    }

    public void SetTimerText(int time)
    {
        SetText(timerText, time.ToString());
    }
    private void SetText(TextMeshProUGUI text, string message)
    {
        text.text = message;
    }

    public void SetHeaderText(string message, RESULT gameResult)
    {
        if (gameResult == RESULT.PLAYER1WON)
        {
            if (LocalPlayerData.Instance.GetPlayerID() == 0)
            {
                SetText(gameOverText, MetaStringConstants.VICTORYTEXT);
            }
            else if (LocalPlayerData.Instance.GetPlayerID() == 1)
            {
                SetText(gameOverText, MetaStringConstants.LOSSTEXT);
            }
        }
        else if (gameResult == RESULT.PLAYER2WON)
        {
            if (LocalPlayerData.Instance.GetPlayerID() == 0)
            {
                SetText(gameOverText, MetaStringConstants.LOSSTEXT);
            }
            else if (LocalPlayerData.Instance.GetPlayerID() == 1)
            {
                SetText(gameOverText, MetaStringConstants.VICTORYTEXT);
            }
        }
        else if (gameResult == RESULT.DRAW)
        {
            SetText(gameOverText, MetaStringConstants.DRAWTEXT);
        }
        else if(gameResult == RESULT.RESTART)
        {
            SetText(gameOverText, "");
        }
    }

    public void RestartGame()
    {
        RPSGameSystem.instance.RestartGame();

        foreach (GameObject obj in serverOnlyElements)
        {
            obj.SetActive(false);
        }
    }

    public void ShowJoinLobbyObjects()
    {
        createObject.SetActive(false);
        joinObject.SetActive(true);
    }

    public void ShowCreateLobbyObjects()
    {
        createObject.SetActive(true);
        joinObject.SetActive(false);
    }
}

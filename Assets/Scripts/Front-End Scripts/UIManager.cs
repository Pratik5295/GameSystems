using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField] private TextMeshProUGUI text1; 

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
                SetText(text1, MetaStringConstants.VICTORYTEXT);
            }
            else if (LocalPlayerData.Instance.GetPlayerID() == 1)
            {
                SetText(text1, MetaStringConstants.LOSSTEXT);
            }
        }
        else if (gameResult == RESULT.PLAYER2WON)
        {
            if (LocalPlayerData.Instance.GetPlayerID() == 0)
            {
                SetText(text1, MetaStringConstants.LOSSTEXT);
            }
            else if (LocalPlayerData.Instance.GetPlayerID() == 1)
            {
                SetText(text1, MetaStringConstants.VICTORYTEXT);
            }
        }
        else if (gameResult == RESULT.DRAW)
        {
            if (LocalPlayerData.Instance.GetPlayerID() == 0)
            {
                SetText(text1, MetaStringConstants.DRAWTEXT);
            }
            else if (LocalPlayerData.Instance.GetPlayerID() == 1)
            {

            }
        }
    }
}

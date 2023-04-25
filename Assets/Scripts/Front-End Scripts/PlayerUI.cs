using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Options Parent")]
    //Options will be hidden if not local player
    public GameObject playerOptionsParent;

    [Header("Reference to the text displaying player Move")]
    [SerializeField]
    private TextMeshProUGUI moveText;

    [Header("Player choices")]
    //Player Button options : Rock, Paper, Scissors respectively for A,B,C
    public Button optionA;
    public Button optionB; 
    public Button optionC;

    [SerializeField] private RPSNetworkPlayer player;

    public void SetMoveText(string move)
    {
        moveText.text = move;
    }
    public void OnOptionASelected()
    {
        if(player.IsOwner)
            player.MoveSelected(MOVE.ROCK);
    }   

    public void OnOptionBSelected()
    {
        if (player.IsOwner)
            player.MoveSelected(MOVE.PAPER);
    }

    public void OnOptionCSelected()
    {
        if (player.IsOwner)
            player.MoveSelected(MOVE.SCISSORS);
    }

    public void ShowChoices()
    {
        playerOptionsParent.SetActive(true);
    }

    public void HideChoices()
    {
        playerOptionsParent.SetActive(false);
    }
}

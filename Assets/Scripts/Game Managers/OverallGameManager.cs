using System;
using UnityEngine;

public enum GAMESTATE
{
    NONE = 0,
    PLAYERCREATE = 1,
    LOBBY = 2,
    INGAME = 3,
}

[DefaultExecutionOrder(-1)]
public class OverallGameManager : MonoBehaviour
{
    public static OverallGameManager Instance;

    [SerializeField]private GAMESTATE gameState;

    public Action<GAMESTATE> onGameStateAction;

    //Set to true if the player is creator of the relay/room/game
    public bool isCreator;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            OnPlayerCreationState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Only to be use at time of manager starts
    //In future to be changed to detect main state after all loading is done
    public GAMESTATE GetCurrentState()
    {
        return gameState;
    }

    private void SetState(GAMESTATE _state)
    {
        gameState = _state;
        onGameStateAction?.Invoke(gameState);
    }

    public void OnPlayerCreationState()
    {
        SetState(GAMESTATE.PLAYERCREATE);
    }

    public void OnLobbyState()
    {
        SetState(GAMESTATE.LOBBY);
    }

    //public void InRoomState()
    //{
    //    SetState(GAMESTATE.INROOM);
    //}

    public void InGameState()
    {
        SetState(GAMESTATE.INGAME);
    }

}

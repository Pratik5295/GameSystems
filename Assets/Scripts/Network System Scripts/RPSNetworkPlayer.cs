using Unity.Netcode;
using UnityEngine;


public struct PlayerInformation : INetworkSerializable, System.IEquatable<PlayerInformation>
{
    public ulong playerSessionId;
    public MOVE playerMove;

    public bool Equals(PlayerInformation other)
    {
        return playerSessionId == other.playerSessionId && playerMove == other.playerMove;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out playerSessionId);
            reader.ReadValueSafe(out playerMove);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(playerSessionId);
            writer.WriteValueSafe(playerMove);
        }
    }
}

public class RPSNetworkPlayer : NetworkBehaviour
{
    public enum PLAYERSTATE
    {
        WAITING = 0,
        DECIDING = 1,
        DECIDED = 2
    }
    private MOVE playerMove;    // This is a local reference to verify syncing with client

    [SerializeField] private NetworkVariable<MOVE> NetworkMove = new NetworkVariable<MOVE>();

    private ulong playerID;

    [SerializeField] private PLAYERSTATE State;

    [SerializeField] private RPSGameSystem gameSystem;

    public NetworkVariable<RESULT> endResult = new NetworkVariable<RESULT>();

    [SerializeField] private PlayerUI playerUI;

    public override void OnNetworkSpawn()
    {
        playerID = OwnerClientId;
        State = PLAYERSTATE.WAITING;

        NetworkMove.OnValueChanged += (MOVE previousValue, MOVE currentValue) =>
        {
            Debug.Log($"{playerID} has Selected Move: {currentValue.ToString()}");
            SetLocalValue(currentValue);
        };

        gameSystem = RPSGameSystem.instance;

        if (IsOwner)
        {
            LocalPlayerData.Instance.SetPlayerID(playerID);
        }
            playerUI.HideChoices();

        OverallGameManager.Instance.onGameStateAction += IsGameRunning;

        if(gameSystem != null)
        {
            gameSystem.OnGameOverEvent += OnPlayerMoveResponseEventHandler;

            gameSystem.networkGameState.OnValueChanged += OnGameRestartedEventHandler;
        }
        
    }

    public override void OnDestroy()
    {
        OverallGameManager.Instance.onGameStateAction -= IsGameRunning;
        gameSystem.OnGameOverEvent -= OnPlayerMoveResponseEventHandler;

        gameSystem.networkGameState.OnValueChanged -= OnGameRestartedEventHandler;
        base.OnDestroy();
    }

    private void IsGameRunning(GAMESTATE state)
    {
        if(state == GAMESTATE.INGAME)
        {
            StartGame();
        }
    }

    private void OnPlayerMoveResponseEventHandler()
    {
        if (!IsOwner)
        {
            //If local player is not the owner of the object, i.e its the opponent
            UpdateOpponentMoveUIText();
        }
    }


    public ulong GetPlayerId()
    {
        return playerID;
    }
    private void SetLocalValue(MOVE moveFromNetwork)
    {
        playerMove = moveFromNetwork;
        Debug.Log($"Setting local move info for {playerID} and Network Move: {moveFromNetwork.ToString()}");

        if (IsOwner)
        {
            //For owner, show the move selected at the time of selection
            SetMoveUIText(playerMove);
        }
    }

    private void Update()
    {
        if(State == PLAYERSTATE.DECIDING)
        {
            //Take input only if the player is in deciding state
            if (Input.GetKeyDown(KeyCode.R))
            {
                //Rock
                MoveSelected(MOVE.ROCK);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //Scissors
                MoveSelected(MOVE.SCISSORS);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                //Paper
                MoveSelected(MOVE.PAPER);
            }
        }
        else if(State == PLAYERSTATE.WAITING ||  State == PLAYERSTATE.DECIDED)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }
    }

    public void SetMove(MOVE playerMove)
    {
        SetMoveUIText(playerMove);
    }
    public void SetMoveUIText(MOVE playerMove)
    {
        if (IsOwner)
        {
            playerUI.SetMoveText($"You have selected {playerMove.ToString()}");
        }
    }

    public void UpdateOpponentMoveUIText()
    {
        if (!IsOwner)
        {
            playerUI.SetMoveText($"Opponent has selected {playerMove.ToString()}");
        }
    }
    public void MoveSelected(MOVE _playerMove)
    {
        OnPlayerDecided();
        gameSystem.OnPlayerMoveSentToServerRPC(_playerMove);
        OnPlayerMoveSentToServerRPC(_playerMove);
    }


    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerMoveSentToServerRPC(MOVE playerMove, ServerRpcParams serverRpcParams = default)
    {
        NetworkMove.Value = playerMove;
    }

    private void SetState(PLAYERSTATE _state)
    {
        State = _state;
    }

    public void StartGame()
    {
        SetState(PLAYERSTATE.DECIDING);
        gameSystem.FindLocalPlayersConnected();
        gameSystem.AddPlayerToDictionaryServerRpc(playerID, MOVE.NONE);

        ShowPlayerChoices();

    }

    private void ShowPlayerChoices()
    {
        if (IsOwner)
            playerUI.ShowChoices();
    }

    public void OnPlayerDecided()
    {
        SetState(PLAYERSTATE.DECIDED);
    }

    public void OnGameOver()
    {
        SetState(PLAYERSTATE.WAITING);
    }


    private void OnGameRestartedEventHandler(RPSGameSystem.STATE previous, RPSGameSystem.STATE current)
    {
        if (current == RPSGameSystem.STATE.INGAME)
        {
            MoveSelected(MOVE.NONE);
            SetState(PLAYERSTATE.DECIDING);
            ShowPlayerChoices();
            playerUI.SetMoveText("");
        }
    }
}

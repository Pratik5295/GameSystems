using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum MOVE
{
    NONE = 0, //By Default its none, or if player doesnt make a decision
    ROCK = 1,
    PAPER = 2,
    SCISSORS = 3
}

public enum RESULT
{
    DRAW = 0,
    PLAYER1WON = 1,
    PLAYER2WON = 2
}

public class RPSGameSystem : NetworkBehaviour
{
    public static RPSGameSystem instance = null;
    public enum STATE
    {
        NONE = 0,
        START = 1, //Represents the start phase of the game (Searching for players phase)
        INGAME = 2, // Represents the players ready phase of the game
        END = 3 //End result and winner calculation phase
    }

    //For now sync local players across all platform
    [SerializeField]
    private List<RPSNetworkPlayer> localPlayers = new List<RPSNetworkPlayer>();

    [SerializeField]
    public Dictionary<ulong, MOVE> localMoveReference = new Dictionary<ulong, MOVE>();

    [SerializeField]
    public NetworkList<PlayerInformation> playersInfo;

    public List<RPSNetworkPlayer> localPlayersData;

    [SerializeField] private NetworkVariable<RESULT> result;

    [SerializeField] private GameObject leftPos;
    [SerializeField] private GameObject rightPos;


    [SerializeField] private NetworkVariable<float> gameTime = new NetworkVariable<float>();
    public float GameTime = 0f;
    public STATE gameState;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            playersInfo = new NetworkList<PlayerInformation>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playersInfo.OnListChanged += OnServerListChanged;
        }
        if(IsClient)
        {
            playersInfo.OnListChanged += OnClientListChanged;
        }

        result.OnValueChanged += OnResultDeclared;

        gameTime.Value = 0f;
        gameTime.OnValueChanged += OnUpdateGameTime;
    }


    void OnUpdateGameTime(float previous, float result)
    {

    }
    void OnResultDeclared(RESULT previous, RESULT current)
    {
        Debug.Log($"New result declared: {current.ToString()}");    
    }

    void OnServerListChanged(NetworkListEvent<PlayerInformation> changeEvent)
    {
        Debug.Log($"[S] The list changed and now has {playersInfo.Count} elements");

        DisplayPlayerInformation();
    }

    void OnClientListChanged(NetworkListEvent<PlayerInformation> changeEvent)
    {
        Debug.Log($"[C] The list changed and now has {playersInfo.Count} elements");
        DisplayPlayerInformation();
    }

    public void FindLocalPlayersConnected()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var player in players)
        {
            if(!localPlayers.Contains(player.GetComponent<RPSNetworkPlayer>()))
                localPlayers.Add(player.GetComponent<RPSNetworkPlayer>());
        }
        SetPlayersPosition();
    }

    private void SetPlayersPosition()
    {
        foreach(var player in localPlayers)
        {
            ulong id = player.GetPlayerId();
            if(id == 0)
            {
                //Set at left position
                player.gameObject.transform.position = leftPos.transform.position;
            }
            else
            {
                //Set at right position
                player.gameObject.transform.position = rightPos.transform.position;
            }
        }
    }

    void DisplayPlayerInformation()
    {
        foreach (var player in playersInfo)
        {
            Debug.Log($"Player ID: {player.playerSessionId} and Move: {player.playerMove}");
        }
    }


    private void Update()
    {
        if (IsServer)
        {
            if (gameState == STATE.INGAME)
            {
                if (GameTime > 10f)
                {
                    gameState = STATE.END;
                    OnGameEnds();
                }
                GameTime += Time.deltaTime;

                gameTime.Value += Time.deltaTime;
            }
        }
    }
    public void MoveSentByPlayer(ulong clientID,MOVE playerMove)
    {
        if(clientID == 0)
        {
            localMoveReference[0] = playerMove;
        }
        else
        {
           localMoveReference[1] = playerMove;
        }
    }

    public void OnGameStart()
    {
        gameState = STATE.INGAME;
    }

    [ServerRpc]
    public void OnGameStartServerRPC()
    {
        Debug.Log("Starting game from client");
        gameState = STATE.INGAME;
    }

    public void OnGameEnds()
    {
        MOVE p1Move = MOVE.NONE, p2Move = MOVE.NONE;

        if (localMoveReference.ContainsKey(0))
        {
            localMoveReference.TryGetValue(0, out p1Move);
        }
        if (localMoveReference.ContainsKey(1))
        {
            localMoveReference.TryGetValue(1, out p2Move);
        }
        switch (p1Move, p2Move)
        {
            case (MOVE.ROCK,MOVE.ROCK):
                MatchDraw();
                break;
            case (MOVE.ROCK, MOVE.PAPER):
                Player2Wins();
                break;
            case (MOVE.ROCK, MOVE.SCISSORS):
                Player1Wins();
                break;
            case (MOVE.PAPER, MOVE.ROCK):
                Player1Wins();
                break;
            case (MOVE.PAPER, MOVE.PAPER):
                MatchDraw();
                break;
            case (MOVE.PAPER, MOVE.SCISSORS):
                Player2Wins();
                break;
            case (MOVE.SCISSORS, MOVE.ROCK):
                Player2Wins();
                break;
            case (MOVE.SCISSORS, MOVE.PAPER):
                Player1Wins();
                break;
            case (MOVE.SCISSORS, MOVE.SCISSORS):
                MatchDraw();
                break;
        }

        PrepareToSendGameOver();
    }

    private void Player1Wins()
    {
        Debug.Log("Player 1 wins");
        result.Value = RESULT.PLAYER1WON;
    }
    private void Player2Wins()
    {
        Debug.Log("Player 2 wins");
        result.Value = RESULT.PLAYER2WON;
    }
    private void MatchDraw()
    {
        Debug.Log("Nobody wins! Its a draw");
        result.Value = RESULT.DRAW;
    }

    private void PrepareToSendGameOver()
    {
        OnGameOverSentToClientRPC(result.Value);
    }

    //Network Behavior Code


    /// <summary>
    /// Only server side can perform this check
    /// The server side will determine how many players are connected and add them all at once
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="playerMove"></param>
    [ServerRpc]
    public void AddPlayerToDictionaryServerRpc(ulong clientId, MOVE playerMove)
    {
        if (!localMoveReference.ContainsKey(clientId))
            localMoveReference.Add(clientId, playerMove);

        if (localMoveReference.Count == 2)
        {
            OnGameStart();
        }

        PlayerInformation playerInfo = new PlayerInformation
        {
            playerSessionId = clientId,
            playerMove = playerMove
        };

        playersInfo.Add(playerInfo);

        Debug.Log("[S] Added player on server!");
    }

    //Just add me to server's copy of player info
    public void AddPlayerToData(RPSNetworkPlayer player)
    {
        if (IsServer)
        {
            if (!localPlayersData.Contains(player))
                localPlayersData.Add( player);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerMoveSentToServerRPC(MOVE playerMove, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"[S] received response {playerMove.ToString()}");
        var senderClientId = serverRpcParams.Receive.SenderClientId;
        MoveSentByPlayer(senderClientId, playerMove);

        OnPlayerMoveReceivedClientRpc(senderClientId,playerMove);
    }


    //Inform client that a server call was received
    [ClientRpc]
    public void OnPlayerMoveReceivedClientRpc(ulong senderId, MOVE playerMove)
    {
        Debug.Log($"[C]Input received from player {senderId} and {playerMove}");
    }

    [ClientRpc]
    public void OnGameOverSentToClientRPC(RESULT result)
    {
        UIManager.Instance.SetHeaderText("Game Over: ", result);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerConnectionRequestToServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ClientRpcSendParams sendParams = new ClientRpcSendParams
        {
            TargetClientIds = new List<ulong>
            {
                serverRpcParams.Receive.SenderClientId
            }
        };

        ClientRpcReceiveParams recieveParams = default;

        ClientRpcParams clientParams = new ClientRpcParams { 
            Receive = recieveParams, 
            Send = sendParams
        };
        
        OnPlayerConnectedClientRPC(clientParams);
    }

    [ClientRpc]
    public void OnPlayerConnectedClientRPC(ClientRpcParams clientParams)
    {
        var clientIdList = clientParams.Send.TargetClientIds;
        foreach (var clientId in clientIdList)
        {
            Debug.Log($"{clientId} is the ClientID who connected");
        }
    }

}
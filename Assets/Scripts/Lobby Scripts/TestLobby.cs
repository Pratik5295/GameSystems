using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class TestLobby : MonoBehaviour
{
    public string lobbyName;
    [SerializeField] private bool hasLobby = false;
    [SerializeField] private Lobby mainLobby;

    [SerializeField] private Lobby currentLobby;

    [SerializeField] private string playerName;

    private float heartBeatTimer;
    private float heartPollTimer;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => { Debug.Log("Logged  in"); };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        //if (!hasLobby)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        CreateLobby();
        //        hasLobby = true;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            ListLobbies();
        }

        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdate();
    }

    private async void HandleLobbyHeartbeat()
    {
        if(currentLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;

            if(heartBeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartBeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdate()
    {
        if (currentLobby != null)
        {
            heartPollTimer -= Time.deltaTime;

            if (heartPollTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                heartPollTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = lobby;
            }
        }
    }

    private async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "Game Mode", new DataObject(DataObject.VisibilityOptions.Public,"Not Started")
                    }
                        
                }
            };
            mainLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, lobbyOptions);

            Debug.Log($"Created  lobby with: {mainLobby.Name} and code: {mainLobby.LobbyCode}");

            currentLobby = mainLobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName",
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,
                            LocalPlayerData.Instance.GetPlayerName())
                        },

                    }
        };
    }

    private async void ListLobbies()
    {
        try
        {

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log($"Lobbies found: {queryResponse.Results.Count}");

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log($"{lobby.Name} exists");
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    private async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void KickFromLobby(int index)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, currentLobby.Players[index].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerName(string _playerName)
    {
        try
        {
            playerName = _playerName;
            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId,
                new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}
                    }

                });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdateGameMode(string _gameMode)
    {
        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, 
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "Game Mode", new DataObject(DataObject.VisibilityOptions.Public,_gameMode)
                        }

                    }
                });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerData : MonoBehaviour
{
    //Hold local player related information
    public static LocalPlayerData Instance;

    [SerializeField] private ulong playerId;

    [SerializeField] private string playerName;

    [SerializeField] private TMP_InputField playerNameInput;

    [SerializeField] private Button onCreateButton;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        onCreateButton.onClick.AddListener(SavePlayerName);
    }
    public void SetPlayerID(ulong id)
    {
        playerId = id;
    }

    public ulong GetPlayerID()
    {
        return playerId;
    }

    public void SavePlayerName()
    {
        if (string.IsNullOrEmpty(playerNameInput.text)) return;

        playerName = playerNameInput.text;
        OnPlayerCreated();
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    private void OnPlayerCreated()
    {
        OverallGameManager.Instance.OnLobbyState();
    }
}

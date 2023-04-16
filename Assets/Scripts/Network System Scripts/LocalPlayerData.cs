using UnityEngine;

public class LocalPlayerData : MonoBehaviour
{
    //Hold local player related information
    public static LocalPlayerData Instance;

    [SerializeField] private ulong playerId;

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

    public void SetPlayerID(ulong id)
    {
        playerId = id;
    }

    public ulong GetPlayerID()
    {
        return playerId;
    }
}

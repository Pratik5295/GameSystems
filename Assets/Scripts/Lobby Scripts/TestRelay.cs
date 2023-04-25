using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    [SerializeField] private bool hasRelay = false;
    [SerializeField] private TMP_InputField relayCodeInput;
    [SerializeField] private TextMeshProUGUI showRelayText;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => { Debug.Log("Logged  in" + AuthenticationService.Instance.PlayerId); };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }

    public async void CreateRelay()
    {
        if (!hasRelay)
        { 
            try
             {
                 Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

                    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                    Debug.Log("Allocation code is: " + joinCode);
                    showRelayText.text = $"Join code for relay is {joinCode}";
                    hasRelay = true;

                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                            allocation.RelayServer.IpV4,
                            (ushort)allocation.RelayServer.Port,
                            allocation.AllocationIdBytes,
                            allocation.Key,
                            allocation.ConnectionData);

                        NetworkManager.Singleton.StartHost();

                OverallGameManager.Instance.isCreator = true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void JoinRelayWithCode()
    {
        string code = relayCodeInput.text;
        if (string.IsNullOrEmpty(code)) return;

        JoinRelay(code);
    }
    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
           JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Joined the relay");


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
               allocation.RelayServer.IpV4,
               (ushort)allocation.RelayServer.Port,
               allocation.AllocationIdBytes,
               allocation.Key,
               allocation.ConnectionData,
               allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();

            OverallGameManager.Instance.isCreator = false;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}

using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public enum State
    {
        GAME = 0,
        END = 1
    }

    public struct Move : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public State gameState;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref gameState);
        }
    }

    private NetworkVariable<Move> move = new NetworkVariable<Move>(
        new Move
        {
            _int = 0,
            _bool = true,
            gameState = State.GAME
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        move.OnValueChanged += (Move previousMove, Move currentMove) =>
        {
            Debug.Log($"{OwnerClientId} Value: {currentMove._int} and {currentMove.gameState.ToString()}");
        };
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            move.Value = new Move
            {
                _int = Random.Range(0, 100),
                _bool = move.Value._bool,
                gameState = move.Value.gameState == State.GAME ? State.END : State.GAME
            };
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TestServerRpc($"Test Sending from {OwnerClientId}");
        }
    }

    [ServerRpc]
    public void TestServerRpc(string message)
    {
        Debug.Log(message);

        TestClientRpc();
    }

    [ClientRpc]
    public void TestClientRpc()
    {
        Debug.Log("received by all");
    }
}

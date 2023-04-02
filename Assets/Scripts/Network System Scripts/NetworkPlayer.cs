using UnityEngine;
using Unity.Netcode;

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

public class NetworkPlayer : NetworkBehaviour
{
    private NetworkVariable<Move> move = new NetworkVariable<Move>(
        new Move
        {
            _int = 0,
            _bool = true,
            gameState = State.GAME
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    private Move networkMove;
    public override void OnNetworkSpawn()
    {
        move.OnValueChanged += (Move previousMove, Move currentMove) =>
        {
            Debug.Log($"{OwnerClientId} Value: {currentMove._int} and {currentMove.gameState.ToString()}");
            networkMove._int = currentMove._int;
            networkMove._bool = currentMove._bool;
            networkMove.gameState = currentMove.gameState;
        };

        SetLocalValues(move);

    }

    private void SetLocalValues(NetworkVariable<Move> _move)
    {
        networkMove._int = _move.Value._int;
        networkMove._bool = _move.Value._bool;
        networkMove.gameState = _move.Value.gameState;
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
            TestServerRpc(networkMove, $"Test Sending from {OwnerClientId}");
        }
    }

    [ServerRpc]
    public void TestServerRpc(Move move, string message)
    {
        Debug.Log($"{message} and more data: {move.gameState.ToString()}");

        TestClientRpc(move);
    }

    [ClientRpc]
    public void TestClientRpc(Move move)
    {
        Debug.Log($"{move.gameState.ToString()} received by all");
    }
}

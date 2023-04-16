using UnityEngine;
using Unity.Netcode;

public enum TestState
{
    GAME = 0,
    END = 1
}
public struct TestMove : INetworkSerializable
{
    public int _int;
    public bool _bool;
    public TestState gameState;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _int);
        serializer.SerializeValue(ref _bool);
        serializer.SerializeValue(ref gameState);
    }
}

public class NetworkPlayer : NetworkBehaviour
{
    private NetworkVariable<TestMove> move = new NetworkVariable<TestMove>(
        new TestMove
        {
            _int = 0,
            _bool = true,
            gameState = TestState.GAME
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    private TestMove networkMove;
    public override void OnNetworkSpawn()
    {
        move.OnValueChanged += (TestMove previousMove, TestMove currentMove) =>
        {
            Debug.Log($"{OwnerClientId} Value: {currentMove._int} and {currentMove.gameState.ToString()}");
            networkMove._int = currentMove._int;
            networkMove._bool = currentMove._bool;
            networkMove.gameState = currentMove.gameState;
        };

        SetLocalValues(move);

    }

    private void SetLocalValues(NetworkVariable<TestMove> _move)
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
            move.Value = new TestMove
            {
                _int = Random.Range(0, 100),
                _bool = move.Value._bool,
                gameState = move.Value.gameState == TestState.GAME ? TestState.END : TestState.GAME
            };
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TestServerRpc(networkMove, $"Test Sending from {OwnerClientId}");
        }
    }

    [ServerRpc]
    public void TestServerRpc(TestMove move, string message)
    {
        Debug.Log($"{message} and more data: {move.gameState.ToString()}");

        TestClientRpc(move);
    }

    [ClientRpc]
    public void TestClientRpc(TestMove move)
    {
        Debug.Log($"{move.gameState.ToString()} received by all");
    }
}

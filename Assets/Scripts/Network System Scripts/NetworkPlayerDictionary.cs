using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking;

public class NetworkPlayerDictionary<T1,T2> : NetworkVariableBase 
    where T1 : unmanaged, IEquatable<T1>
    where T2 : unmanaged, IEquatable<T2>
{
    public delegate void OnDictionaryChangedDelegate(NetworkListEvent<T1> changeEvent);

    private Dictionary<T1, T2> m_dict = new Dictionary<T1, T2>();

    public int Count => m_dict.Count;

    public int LastModifiedTick => int.MinValue;

    public event OnDictionaryChangedDelegate OnDictChanged;


    public override void WriteDelta(FastBufferWriter writer)
    {
        
    }

    public override void WriteField(FastBufferWriter writer)
    {
        throw new NotImplementedException();
    }

    public override void ReadField(FastBufferReader reader)
    {
        throw new NotImplementedException();
    }

    public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    {
        throw new NotImplementedException();
    }
}

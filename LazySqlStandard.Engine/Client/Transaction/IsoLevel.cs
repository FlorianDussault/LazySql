namespace LazySql.Transaction;

/// <summary>Specifies the transaction locking behavior for the connection.</summary>
public enum IsoLevel
{
    /// <summary>A different isolation level than the one specified is being used, but the level cannot be determined.</summary>
    Unspecified = -1, // 0xFFFFFFFF
    /// <summary>The pending changes from more highly isolated transactions cannot be overwritten.</summary>
    Chaos = 16, // 0x00000010
    /// <summary>A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored.</summary>
    ReadUncommitted = 256, // 0x00000100
    /// <summary>Shared locks are held while the data is being read to avoid dirty reads, but the data can be changed before the end of the transaction, resulting in non-repeatable reads or phantom data.</summary>
    ReadCommitted = 4096, // 0x00001000
    /// <summary>Locks are placed on all data that is used in a query, preventing other users from updating the data. Prevents non-repeatable reads but phantom rows are still possible.</summary>
    RepeatableRead = 65536, // 0x00010000
    /// <summary>A range lock is placed on the <see cref="T:System.Data.DataSet" />, preventing other users from updating or inserting rows into the dataset until the transaction is complete.</summary>
    Serializable = 1048576, // 0x00100000
    /// <summary>Reduces blocking by storing a version of data that one application can read while another is modifying the same data. Indicates that from one transaction you cannot see changes made in other transactions, even if you requery.</summary>
    Snapshot = 16777216, // 0x01000000
}
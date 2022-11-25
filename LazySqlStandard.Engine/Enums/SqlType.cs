namespace LazySql;

/// <summary>
/// Sql Type
/// </summary>
public enum SqlType
{
    Default  = -1,
    BigInt = 0,
    Binary = 1,
    Bit = 2,
    Char = 3,
    DateTime = 4,
    Decimal = 5,
    Float = 6,
    Image = 7,
    Int = 8,
    Money = 9,
    NChar = 10, // 0x0000000A
    NText = 11, // 0x0000000B
    NVarChar = 12, // 0x0000000C
    Real = 13, // 0x0000000D
    UniqueIdentifier = 14, // 0x0000000E
    SmallDateTime = 15, // 0x0000000F
    SmallInt = 16, // 0x00000010
    SmallMoney = 17, // 0x00000011
    Text = 18, // 0x00000012
    Timestamp = 19, // 0x00000013
    TinyInt = 20, // 0x00000014
    VarBinary = 21, // 0x00000015
    VarChar = 22, // 0x00000016
    Variant = 23, // 0x00000017
    Xml = 25, // 0x00000019
    Udt = 29, // 0x0000001D
    Structured = 30, // 0x0000001E
    Date = 31, // 0x0000001F
    Time = 32, // 0x00000020
    DateTime2 = 33, // 0x00000021
    DateTimeOffset = 34, // 0x00000022

    Children = 999
}
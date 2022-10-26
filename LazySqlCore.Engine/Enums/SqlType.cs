﻿namespace LazySql.Engine.Enums
{
    public enum SqlType
    {
        Default  = -1,
        AnsiString = 0,
        /// <summary>A variable-length stream of binary data ranging between 1 and 8,000 bytes.</summary>
        Binary = 1,
        /// <summary>An 8-bit unsigned integer ranging in value from 0 to 255.</summary>
        Byte = 2,
        /// <summary>A simple type representing Boolean values of <see langword="true" /> or <see langword="false" />.</summary>
        Boolean = 3,
        /// <summary>A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63 -1 (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of a currency unit.</summary>
        Currency = 4,
        /// <summary>A type representing a date value.</summary>
        Date = 5,
        /// <summary>A type representing a date and time value.</summary>
        DateTime = 6,
        /// <summary>A simple type representing values ranging from 1.0 x 10 -28 to approximately 7.9 x 10 28 with 28-29 significant digits.</summary>
        Decimal = 7,
        /// <summary>A floating point type representing values ranging from approximately 5.0 x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.</summary>
        Double = 8,
        /// <summary>A globally unique identifier (or GUID).</summary>
        Guid = 9,
        /// <summary>An integral type representing signed 16-bit integers with values between -32768 and 32767.</summary>
        Int16 = 10, // 0x0000000A
        /// <summary>An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.</summary>
        Int32 = 11, // 0x0000000B
        /// <summary>An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.</summary>
        Int64 = 12, // 0x0000000C
        /// <summary>A general type representing any reference or value type not explicitly represented by another <see langword="DbType" /> value.</summary>
        Object = 13, // 0x0000000D
        /// <summary>An integral type representing signed 8-bit integers with values between -128 and 127.</summary>
        SByte = 14, // 0x0000000E
        /// <summary>A floating point type representing values ranging from approximately 1.5 x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.</summary>
        Single = 15, // 0x0000000F
        /// <summary>A type representing Unicode character strings.</summary>
        String = 16, // 0x00000010
        /// <summary>A type representing a SQL Server <see langword="DateTime" /> value. If you want to use a SQL Server <see langword="time" /> value, use <see cref="F:System.Data.SqlDbType.Time" />.</summary>
        Time = 17, // 0x00000011
        /// <summary>An integral type representing unsigned 16-bit integers with values between 0 and 65535.</summary>
        UInt16 = 18, // 0x00000012
        /// <summary>An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.</summary>
        UInt32 = 19, // 0x00000013
        /// <summary>An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.</summary>
        UInt64 = 20, // 0x00000014
        /// <summary>A variable-length numeric value.</summary>
        VarNumeric = 21, // 0x00000015
        /// <summary>A fixed-length stream of non-Unicode characters.</summary>
        AnsiStringFixedLength = 22, // 0x00000016
        /// <summary>A fixed-length string of Unicode characters.</summary>
        StringFixedLength = 23, // 0x00000017
        /// <summary>A parsed representation of an XML document or fragment.</summary>
        Xml = 25, // 0x00000019
        /// <summary>Date and time data. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.</summary>
        DateTime2 = 26, // 0x0000001A
        /// <summary>Date and time data with time zone awareness. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through +14:00.</summary>
        DateTimeOffset = 27, // 0x0000001B

        Children = 999

    }
}
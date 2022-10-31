
using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("types")]
    public class TypesTable : LazyBase
    {
        [LazyColumn("id", SqlType.Int32)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn("type_bigint", SqlType.Int64)]
        public long? TypeBigint { get; set; }

        [LazyColumn("type_binary", SqlType.Binary)]
        public Byte[]? TypeBinary { get; set; }

        [LazyColumn("type_bit", SqlType.Boolean)]
        public bool? TypeBit { get; set; }

        [LazyColumn("type_char", SqlType.String)]
        public string? TypeChar { get; set; }

        [LazyColumn("type_date", SqlType.Date)]
        public DateTime? TypeDate { get; set; }

        [LazyColumn("type_datetime2", SqlType.DateTime2)]
        public DateTime? TypeDateTime2 { get; set; }

        [LazyColumn("type_datetimeoffset", SqlType.DateTimeOffset)]
        public DateTimeOffset? TypeDatetimeoffset { get; set; }

        [LazyColumn("type_decimal", SqlType.Decimal)]
        public decimal? TypeDecimal { get; set; }

        [LazyColumn("type_float", SqlType.Double)]
        public double? TypeFloat { get; set; }

        //[LazyColumn("type_geography", SqlType.g)]
        //public long TypeGeography { get; set; }

        //[LazyColumn("type_hierarchyid", SqlType.BigInt)]
        //public long TypeHierarchyid { get; set; }

        [LazyColumn("type_image", SqlType.Binary)]
        public byte[]? TypeImage { get; set; }

        [LazyColumn("type_int", SqlType.Int32)]
        public int? TypeInt { get; set; }

        [LazyColumn("type_money", SqlType.Decimal)]
        public decimal? TypeMoney { get; set; }

        [LazyColumn("type_ntext", SqlType.String)]
        public string? TypeNtext { get; set; }

        [LazyColumn("type_numeric", SqlType.Decimal)]
        public decimal? TypeNumeric { get; set; }

        [LazyColumn("type_real", SqlType.Single)]
        public Single? TypeReal { get; set; }

        [LazyColumn("type_smalldatime", SqlType.Date)]
        public DateTime? TypeSmalldatime { get; set; }

        [LazyColumn("type_smallint", SqlType.Int16)]
        public short? TypeSmallint { get; set; }

        [LazyColumn("type_smallmoney", SqlType.Decimal)]
        public decimal? TypeSmallmoney { get; set; }

        [LazyColumn("type_sql_variant", SqlType.String)]
        public string? TypeSqlVariant { get; set; }

        [LazyColumn("type_text", SqlType.String)]
        public string? TypeText { get; set; }

        [LazyColumn("type_time", SqlType.Time)]
        public DateTime? TypeTime { get; set; }

        //[LazyColumn("type_timestamp", SqlType.Binary)]
        //public byte[]? TypeTimestamp { get; set; }

        [LazyColumn("type_tinyint", SqlType.Byte)]
        public byte? TypeTinyint { get; set; }

        [LazyColumn("type_uniqueidentifier", SqlType.Guid)]
        public Guid? TypeUniqueidentifier { get; set; }

        //[LazyColumn("type_xml", SqlType.Xml)]
        //public long TypeXml { get; set; }
    }
}

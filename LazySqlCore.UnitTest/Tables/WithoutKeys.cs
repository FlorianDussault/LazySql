using LazySql;

namespace LazySqlCore.UnitTest.Tables;

[LazyTable("lazys", "WithoutKeys")]
internal class WithoutKeys : LazyBase
{
    [LazyColumn(SqlType.VarChar)]
    public string Name { get; set; }
    [LazyColumn(SqlType.Int)]
    public int Age { get; set; }
}
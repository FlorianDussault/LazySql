using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest
{
    internal static class ClientTest
    {
        public static void Initialize()
        {
            if (!LazyClient.Initialized)
                LazyClient.Initialize("Server=localhost\\sqlexpress;Database=LazySql;TrustServerCertificate=Yes;Trusted_Connection=True", typeof(TypesTable), typeof(SimpleTable), typeof(ChildTable), typeof(ExtendedTable));
        }
    }
}

using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Client.StoredProcedure;
using LazySql.Engine.Enums;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

public class StoredProcedureTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    private void AddSimpleTables()
    {
        const int COUNT_SIMPLE_TABLE = 20;
        const int COUNT_CHILD_TABLE = 20;
        // Clear Table
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);
        // Add values
        Assert.IsEmpty(LazyClient.Get<SimpleTable>());
        int bot_id = 0;
        for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
        {
            SimpleTable st = new SimpleTable()
            {
                Username = $"U{i+1}",
                Password = $"P{i + 1}"
            };
            st.Insert();

            for (int j = 0; j < COUNT_CHILD_TABLE; j++)
            {
                new ChildTable()
                {
                    Id = ++bot_id,
                    ParentId = st.Id,
                    TypeChar = "hello"
                }.Insert();
            }
        }
        // Check
        Assert.That(COUNT_SIMPLE_TABLE == LazyClient.Get<SimpleTable>().ToList().Count());
        Assert.That(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE == LazyClient.Get<ChildTable>().ToList().Count());
    }

    [Test]
    public void SimpleProcedure()
    {
        AddSimpleTables();

        StoredProcedureResult result = LazyClient.StoredProcedure("simple_procedure",
            new SqlArguments().Add("@Count", SqlType.Int, 10)
                .Add("@Prefix", SqlType.NVarChar, "UT_")
                .AddOut("@IdMax", SqlType.Int)
                .AddOut("@IdMin", SqlType.Int)
        );


        Assert.That(-678, Is.EqualTo(result.ReturnValue));
        Assert.That(20, Is.EqualTo(result.Output.IdMin));
        Assert.That(30, Is.EqualTo(result.Output.IdMax));

        List<dynamic> values = result.Tables[0].Parse().ToList();
            
        Assert.That(10, Is.EqualTo(values.Count));
        int lastId = int.MinValue;
        foreach (dynamic value in values)
        {
            Assert.That(lastId, Is.LessThan(value.user_id));

            string username = value.username;
            string id = username.Substring(3);
            Assert.IsTrue(Guid.TryParse(value.username.Substring(3), out Guid _));
            Assert.That("pwd", Is.EqualTo(value.password));
            Assert.IsNull(value.extended_key);
            lastId = value.user_id;
        }


        List<SimpleTable> list = result.Tables[1].Parse<SimpleTable>().ToList();
        Assert.That(10, Is.EqualTo(list.Count));
        lastId = int.MaxValue;
        foreach (SimpleTable value in list)
        {
            Assert.That(lastId, Is.GreaterThan(value.Id));

            string username = value.Username;
            string id = username.Substring(3);
            Assert.IsTrue(Guid.TryParse(value.Username.Substring(3), out Guid _));
            Assert.IsNull(value.Password);
            Assert.IsNull(value.ExtendedKey);
            lastId = value.Id;
        }
    }

    [Test]
    public void SimpleProcedureWithNull()
    {
        StoredProcedureResult result = LazyClient.StoredProcedure("simple_procedure",
            new SqlArguments().Add("@Count", SqlType.Int, null)
                .Add("@Prefix", SqlType.NVarChar, "UT_")
                .AddOut("@IdMax", SqlType.Int)
                .AddOut("@IdMin", SqlType.Int)
        );

        Assert.That(0, Is.EqualTo(result.ReturnValue));
        Assert.IsNull(result.Output.IdMin);
        Assert.IsNull(result.Output.IdMax);
        Assert.IsEmpty(result.Tables);
    }

}
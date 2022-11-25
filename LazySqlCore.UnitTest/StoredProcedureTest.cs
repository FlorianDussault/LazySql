using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Stored Procedure")]
public class StoredProcedureTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }


    [Test]
    public void SimpleProcedure()
    {
        ClientTest.AddSimpleTables();

        StoredProcedureResult result = LazyClient.StoredProcedure("simple_procedure",
            new SqlArguments().Add("@Count", SqlType.Int, 10)
                .Add("@Prefix", SqlType.NVarChar, "UT_")
                .AddOut("@IdMax", SqlType.Int)
                .AddOut("@IdMin", SqlType.Int)
        );


        Assert.That(result.ReturnValue, Is.EqualTo(-678));
        Assert.That(19, Is.EqualTo(result.Output.IdMin));
        Assert.That(29, Is.EqualTo(result.Output.IdMax));

        List<dynamic> values = result.Tables[0].Parse().ToList();
            
        Assert.That(values.Count, Is.EqualTo(10));
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
        Assert.That(list.Count, Is.EqualTo(10));
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

        Assert.That(result.ReturnValue, Is.EqualTo(0));
        Assert.IsNull(result.Output.IdMin);
        Assert.IsNull(result.Output.IdMax);
        Assert.IsEmpty(result.Tables);
    }

}
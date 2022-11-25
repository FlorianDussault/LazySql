using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Update")]
public class UpdateTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void UpdateLazy()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>())
        {
            simpleTable.Username = $"USERNAME {simpleTable.Id}";
            simpleTable.Password = $"Password {simpleTable.Id}";
            Assert.That(simpleTable.Update(), Is.EqualTo(1));
        }

        List<SimpleTable> updated = LazyClient.Select<SimpleTable>().ToList();
        for (int index = 0; index < updated.Count; index++)
        {
            SimpleTable simpleTable = updated[index];
            Assert.That(simpleTable.Username.Split(' ')[0], Is.EqualTo("USERNAME"));
            Assert.That(int.Parse(simpleTable.Username.Split(' ')[1]), Is.EqualTo(index));
            Assert.That(simpleTable.Password.Split(' ')[0], Is.EqualTo("Password"));
            Assert.That(int.Parse(simpleTable.Password.Split(' ')[1]), Is.EqualTo(index));
        }

        SimpleTable updateSimpleTable = new() {Username = "UPDATE 1", Password = null};

        Assert.That(updateSimpleTable.Update(s=>s.Id == 3), Is.EqualTo(1));
        List<SimpleTable> updatedList = LazyClient.Select<SimpleTable>(s => s.Id == 3).ToList();
        Assert.That(updatedList[0].Username, Is.EqualTo("UPDATE 1"));
        Assert.That(updatedList[0].Password, Is.Null);
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s=>s.Id != 3))
        {
            Assert.That(simpleTable.Username, Is.Not.EqualTo("UPDATE 1"));
        }

        Assert.That(updateSimpleTable.Update(new SqlQuery("User_Id = @Id").Add("@Id",4), nameof(SimpleTable.Password)), Is.EqualTo(1));
         updatedList = LazyClient.Select<SimpleTable>(s => s.Id == 4).ToList();
        Assert.That(updatedList[0].Username, Is.EqualTo("UPDATE 1"));
        Assert.That(updatedList[0].Password, Is.Not.Null);
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id != 3 && s.Id != 4))
        {
            Assert.That(simpleTable.Username, !Is.EqualTo("UPDATE 1"));
            Assert.That(simpleTable.Username, !Is.EqualTo("UPDATE 1"));
        }
    }

    [Test]
    public void UpdateObject()
    {
        ClientTest.AddSimpleTables();

        foreach (Simple_Table simpleTable in LazyClient.Select<Simple_Table >())
        {
            simpleTable.Username = $"USERNAME {simpleTable.User_Id}";
            simpleTable.Password = $"Password {simpleTable.User_Id}";
            int id = simpleTable.User_Id;
            Assert.That(simpleTable.Update(s=>s.User_Id == id, nameof(Simple_Table.User_Id),nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType)), Is.EqualTo(1));
        }

        List<SimpleTable> updated = LazyClient.Select<SimpleTable>().ToList();
        for (int index = 0; index < updated.Count; index++)
        {
            SimpleTable simpleTable = updated[index];
            Assert.That(simpleTable.Username.Split(' ')[0], Is.EqualTo("USERNAME"));
            Assert.That(int.Parse(simpleTable.Username.Split(' ')[1]), Is.EqualTo(index));
            Assert.That(simpleTable.Password.Split(' ')[0], Is.EqualTo("Password"));
            Assert.That(int.Parse(simpleTable.Password.Split(' ')[1]), Is.EqualTo(index));
        }

        Simple_Table updateSimpleTable = new() { Username = "UPDATE 1", Password = null };

        Assert.That(updateSimpleTable.Update(s => s.User_Id == 3, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType)), Is.EqualTo(1));
        List<SimpleTable> updatedList = LazyClient.Select<SimpleTable>(s => s.Id == 3).ToList();
        Assert.That(updatedList[0].Username, Is.EqualTo("UPDATE 1"));
        Assert.That(updatedList[0].Password, Is.Null);
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id != 3))
        {
            Assert.That(simpleTable.Username, Is.Not.EqualTo("UPDATE 1"));
        }

        Assert.That(updateSimpleTable.Update(new SqlQuery("User_Id = 4"), nameof(SimpleTable.Password), nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType)), Is.EqualTo(1));
        updatedList = LazyClient.Select<SimpleTable>(s => s.Id == 4).ToList();
        Assert.That(updatedList[0].Username, Is.EqualTo("UPDATE 1"));
        Assert.That(updatedList[0].Password, Is.Not.Null);
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id != 3 && s.Id != 4))
        {
            Assert.That(simpleTable.Username, !Is.EqualTo("UPDATE 1"));
            Assert.That(simpleTable.Username, !Is.EqualTo("UPDATE 1"));
        }
    }

    [Test]
    public void UpdateDynamic()
    {
        ClientTest.AddSimpleTables();

        dynamic value = new {Username = "DYNA U"};

        Assert.That(LazyClient.Update(value, "simple_table", new SqlQuery("User_Id = 4 OR User_Id = 5")), Is.EqualTo(2));

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s=>s.Id == 4 || s.Id == 5))
        {
            Assert.That(simpleTable.Username, Is.EqualTo(value.Username));
        }

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id != 4 && s.Id != 5))
        {
            Assert.That(simpleTable.Username, Is.Not.EqualTo(value.Username));
        }
    }

    [Test]
    public void UpdateList()
    {
        List<SimpleTable> values = LazyClient.Select<SimpleTable>().ToList();

        foreach (SimpleTable simpleTable in values)
            simpleTable.Password = simpleTable.Id.ToString();

        Assert.That(values.Update(), Is.EqualTo(values.Count));
        values = LazyClient.Select<SimpleTable>().ToList();

        foreach (SimpleTable simpleTable in values)
            Assert.That(simpleTable.Password, Is.EqualTo(simpleTable.Id.ToString()));
    }

}
using System.Linq.Expressions;
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

        // Update 1 object
        // Lazy
        // LazyClient.Update(obj);
        // 
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

        Assert.That(updateSimpleTable.Update(new SqlQuery("User_Id = @Id").Bind("@Id",4), nameof(SimpleTable.Password)), Is.EqualTo(1));
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
    public void UpdateLazyOnSchema()
    {
        ClientTest.AddSchemaRows();

        foreach (PrimaryValue simpleTable in LazyClient.Select<PrimaryValue>())
        {
            simpleTable.Value = $"USERNAME {simpleTable.Key}";
            Assert.That(simpleTable.Update(), Is.EqualTo(1));
        }

        List<PrimaryValue> updated = LazyClient.Select<PrimaryValue>().ToList();
        for (int index = 0; index < updated.Count; index++)
        {
            PrimaryValue simpleTable = updated[index];
            Assert.That(simpleTable.Value.Split(' ')[0], Is.EqualTo("USERNAME"));
            Assert.That(int.Parse(simpleTable.Value.Split(' ')[1]), Is.EqualTo(index+1));
        }

        PrimaryValue updateSimpleTable = new() { Value = null };

        Assert.That(updateSimpleTable.Update(s => s.Key == 3), Is.EqualTo(1));
        List<PrimaryValue> updatedList = LazyClient.Select<PrimaryValue>(s => s.Key == 3).ToList();
        Assert.That(updatedList[0].Value, Is.Null);

        foreach (PrimaryValue simpleTable in LazyClient.Select<PrimaryValue>(s => s.Key != 3))
        {
            Assert.That(simpleTable.Value, Is.Not.Null);
        }
    }

    [Test]
    public void UpdateMultipleRows()
    {
        ClientTest.AddSimpleTables();

        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s=>s.Username == "X" || s.Password == "Y"));

        SimpleTable simpleTable = new() { Username = "X", Password = "Y"};
        LazyClient.Update<dynamic>("Simple_Table", new {simpleTable.Username, simpleTable.Password});
        
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => s.Username != "X" &&
                                                           s.Password != "Y"));
    }

    [Test]
    public void UpdateMultipleRowsOnSchema()
    {
        ClientTest.AddSchemaRows();

        PrimaryValue simpleTable = new() { Value = "HELLO" };
        Assert.That(LazyClient.Update<dynamic>("lazys","tablePrimary", new { simpleTable.Value }, new SqlQuery("Id = 10 OR Id = 15")), Is.EqualTo(2));

        Assert.That(LazyClient.Select<PrimaryValue>(s => s.Key == 10 || s.Key == 15).Count, Is.EqualTo(2));
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
    public void UpdateObjectOnSchema()
    {
        ClientTest.AddSchemaRows();

        foreach (TablePrimary simpleTable in LazyClient.Select<TablePrimary>("lazys", "tablePrimary"))
        {
            simpleTable.Value = $"USERNAME {simpleTable.Id}";
            int id = simpleTable.Id;
            Assert.That(
                LazyClient.Update("lazys", "tablePrimary", simpleTable, s => s.Id == id, nameof(TablePrimary.Id)),
                Is.EqualTo(1));
        }

        List<TablePrimary> updated = LazyClient.Select<TablePrimary>("lazys", "tablePrimary").ToList();
        for (int index = 0; index < updated.Count; index++)
        {
            TablePrimary simpleTable = updated[index];
            Assert.That(simpleTable.Value.Split(' ')[0], Is.EqualTo("USERNAME"));
            Assert.That(int.Parse(simpleTable.Value.Split(' ')[1]), Is.EqualTo(index + 1));
            
        }
    }

    [Test]
    public void UpdateDynamic()
    {
        ClientTest.AddSimpleTables();

        dynamic value = new {Username = "DYNA U"};

        Assert.That(LazyClient.Update("simple_table", value, new SqlQuery("User_Id = 4 OR User_Id = 5")), Is.EqualTo(2));

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
    public void UpdateDynamicOnSchema()
    {
        ClientTest.AddSchemaRows();

        dynamic value = new { Value = "DYNA U" };

        Assert.That(LazyClient.Update<dynamic>("lazys", "tablePrimary", value, new SqlQuery("Id = 4 OR Id = 5")), Is.EqualTo(2));

        foreach (PrimaryValue simpleTable in LazyClient.Select<PrimaryValue>(s => s.Key == 4 || s.Key == 5))
        {
            Assert.That(simpleTable.Value, Is.EqualTo(value.Value));
        }

        foreach (PrimaryValue simpleTable in LazyClient.Select<PrimaryValue>(s => s.Key != 4 && s.Key != 5))
        {
            Assert.That(simpleTable.Value, Is.Not.EqualTo(value.Value));
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
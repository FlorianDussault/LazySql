using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Lambda Expression")]
public class LambdaTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void GetSimple()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id < 10))
        {
            Assert.Less(simpleTable.Id, 10);
        }

    }

    [Test]
    public void Linq()
    {
        ClientTest.AddSimpleTables();

        int valInt = 1;
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id < 9 + valInt))
        {
            Assert.Less(simpleTable.Id, 10);
        }

        {
            string valString = "U";

            List<SimpleTable> simpleTables =
                LazyClient.Select<SimpleTable>(s => s.Username == valString + "10").ToList();

            Assert.That(simpleTables.Count, Is.EqualTo(1));
            Assert.That(simpleTables[0].Username, Is.EqualTo("U10"));
        }
    }



    [Test]
    public void FunctionLike()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s =>
                     LzFunctions.Like(s.Id, "%1%") && LzFunctions.Like(s.Id, "%5")))
        {
            Assert.IsTrue(simpleTable.Id.ToString().Contains("1"));
            Assert.IsTrue(simpleTable.Id.ToString().Contains("5"));
        }
    }

    [Test]
    public void FunctionNotLike()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s =>
                     LzFunctions.NotLike(s.Id, "%1%") && LzFunctions.NotLike(s.Id, "%5")))
        {
            Assert.IsFalse(simpleTable.Id.ToString().Contains("1"));
            Assert.IsFalse(simpleTable.Id.ToString().Contains("5"));
        }
    }

    [Test]
    public void FunctionIsDate()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "NOT A DATE"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.IsDate(s.Username) == 1);
            Assert.IsEmpty(values);
        }

        simpleTable = new()
        {
            Username = "2022-09-08"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.IsDate(s.Username) == 1);
            Assert.That(values.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public void FunctionGetDate()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.IsDate(LzFunctions.GetDate()) == 1);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionDay()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int day = DateTime.Now.Day;
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.Day(LzFunctions.GetDate()) == day);
            Assert.IsNotEmpty(values);
        }
    }



    [Test]
    public void FunctionMonth()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int month = DateTime.Now.Month;
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.Month(LzFunctions.GetDate()) == month);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionYear()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int year = DateTime.Now.Year;
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.Year(LzFunctions.GetDate()) == year);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionDateAdd()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int year = DateTime.Now.Year + 1;
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s =>
                LzFunctions.Year(LzFunctions.DateAdd(LzDatePart.Year, 1, LzFunctions.GetDate())) == year);
            Assert.IsNotEmpty(values);

            year = DateTime.Now.Year - 1;
            ILazyEnumerable<SimpleTable> values2 = LazyClient.Select<SimpleTable>(s =>
                LzFunctions.Year(LzFunctions.DateAdd(LzDatePart.Year, -1, LzFunctions.GetDate())) == year);
            Assert.IsNotEmpty(values2);
        }
    }

    [Test]
    public void FunctionDateDiff()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO WORLD"
        };
        simpleTable.Insert();

        {
            DateTime start = DateTime.Now;
            DateTime end = start.AddYears(8);
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => LzFunctions.DateDiff(LzDatePart.Year, start, end) == 8);
            Assert.IsNotEmpty(values);
            values = LazyClient.Select<SimpleTable>(s => LzFunctions.DateDiff(LzDatePart.Year, end, start) == -8);
            Assert.IsNotEmpty(values);

            //year = DateTime.Now.Year - 1;
            //var values2 = LazyClient.Get<SimpleTable>((s) => LzFunctions.Year(LzFunctions.DateAdd(LzFunctions.LzDatePart.Year, -1, LzFunctions.GetDate())) == 8);
            //Assert.IsNotEmpty(values2);
        }
    }

    [Test]
    public void FunctionConcat()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s =>
            LzFunctions.Concat(s.Username, " ", s.Password) == "HELLO WORLD"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s =>
            LzFunctions.Concat(s.Username, " ", s.Password) == "HELLOWORLD"));
    }

    [Test]
    public void FunctionConcatPlus()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => s.Username + " " + s.Password == "HELLO WORLD"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => s.Username + " " + s.Password == "HELLO2WORLD"));
    }

    [Test]
    public void FunctionConcatWs()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s =>
            LzFunctions.ConcatWs("@", s.Username, s.Password, "!") == "HELLO@WORLD@!"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s =>
            LzFunctions.ConcatWs("@", s.Username, s.Password, "!") == "HELLOWORLD@!"));
    }

    [Test]
    public void FunctionAscii()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Ascii(s.Username) == 72));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Ascii(s.Username) == 12));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Ascii(s.Password) == 87));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Ascii(s.Password) == 93));
    }

    [Test]
    public void FunctionChar()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Char(72) == "H"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Char(72) == "W"));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Char(87) == "W"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.Char(87) == "H"));
    }

    [Test]
    public void FunctionCharIndex()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex(null, "world", 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("hello", null, 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("hello", "world", null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("world", "hello world", 0) == 7));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("world", "hello world", 0) == 8));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("hello", "hello world", 0) == 1));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.CharIndex("hello", "hello world", 2) == 0));
    }

    [Test]
    public void FunctionDataLength()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.DataLength(null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.DataLength(-1) == 4));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.DataLength(s.Username) == 5));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => LzFunctions.DataLength(s.Username) == 4));
    }

    [Test]
    public void CSharpStringJoin()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s=>string.Join("@", s.Username, s.Password) == "HELLO@WORLD"));
    }


    [Test]
    public void CSharpStringFormat()
    {
        ClientTest.CleanTables();

        SimpleTable simpleTable = new()
        {
            Username = "HELLO WORLD, HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        simpleTable.Username += $" {simpleTable.Id}!";
        simpleTable.Update();

        string hello = "HELLO";

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => s.Username == $"{hello} WORLD, {hello} {simpleTable.Id}!"));
    }

    //[Test]
    //public void CSharpStringJoin()
    //{
    //    LazyClient.Truncate<SubChildTable>(true);
    //    LazyClient.Delete<ChildTable>();
    //    LazyClient.Truncate<SimpleTable>(true);

    //    SimpleTable simpleTable = new()
    //    {
    //        Username = "HELLO",
    //        Password = "WORLD"
    //    };
    //    simpleTable.Insert();

    //    Assert.IsNotEmpty(LazyClient.Get<SimpleTable>(s => string.Join("@", s.Username, s.Password) == "HELLO@WORLD"));
    //}
}
using LazySql;
using LazySqlCore.UnitTest.Tables;
namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Functions")]
public class FunctionsTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }
    
    [Test]
    public void FunctionLike()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s =>
                     Lf.Like(s.Id, "%1%") && Lf.Like(s.Id, "%5")))
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
                     Lf.NotLike(s.Id, "%1%") && Lf.NotLike(s.Id, "%5")))
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.IsDate(s.Username) == 1);
            Assert.IsEmpty(values);
        }

        simpleTable = new()
        {
            Username = "2022-09-08"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.IsDate(s.Username) == 1);
            Assert.That(values.Count, Is.EqualTo(1));
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.IsDate(Lf.GetDate()) == 1);
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.Day(Lf.GetDate()) == day);
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.Month(Lf.GetDate()) == month);
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.Year(Lf.GetDate()) == year);
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
                Lf.Year(Lf.DateAdd(LzDatePart.Year, 1, Lf.GetDate())) == year);
            Assert.IsNotEmpty(values);

            year = DateTime.Now.Year - 1;
            ILazyEnumerable<SimpleTable> values2 = LazyClient.Select<SimpleTable>(s =>
                Lf.Year(Lf.DateAdd(LzDatePart.Year, -1, Lf.GetDate())) == year);
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
            ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.DateDiff(LzDatePart.Year, start, end) == 8);
            Assert.IsNotEmpty(values);
            values = LazyClient.Select<SimpleTable>(s => Lf.DateDiff(LzDatePart.Year, end, start) == -8);
            Assert.IsNotEmpty(values);
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
            Lf.Concat(s.Username, " ", s.Password) == "HELLO WORLD"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s =>
            Lf.Concat(s.Username, " ", s.Password) == "HELLOWORLD"));
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
            Lf.ConcatWs("@", s.Username, s.Password, "!") == "HELLO@WORLD@!"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s =>
            Lf.ConcatWs("@", s.Username, s.Password, "!") == "HELLOWORLD@!"));
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

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.Ascii(s.Username) == 72));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.Ascii(s.Username) == 12));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.Ascii(s.Password) == 87));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.Ascii(s.Password) == 93));
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

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.Char(72) == "H"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.Char(72) == "W"));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.Char(87) == "W"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.Char(87) == "H"));
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

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex(null, "world", 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("hello", null, 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("hello", "world", null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("world", "hello world", 0) == 7));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("world", "hello world", 0) == 8));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("hello", "hello world", 0) == 1));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.CharIndex("hello", "hello world", 2) == 0));
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

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.DataLength(null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.DataLength(-1) == 4));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>(s => Lf.DataLength(s.Username) == 5));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => Lf.DataLength(s.Username) == 4));
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

    [Test]
    public void FunctionLower()
    {
        ClientTest.AddSimpleTables();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>(s=>Lf.Ascii(Lf.Lower("A")) == Lf.Ascii("a")).ToList();
        Assert.IsNotEmpty(values);
    }

    [Test]
    public void FunctionUpper()
    {
        ClientTest.AddSimpleTables();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>(s => Lf.Ascii(Lf.Upper("a")) == Lf.Ascii("A")).ToList();
        Assert.IsNotEmpty(values);
    }

    [Test]
    public void As()
    {
        ClientTest.AddSimpleTables();

        ILazyEnumerable<SimpleTable> values = LazyClient.Select<SimpleTable>()
            .Columns(s => Lf.Lower(s.Username).As("Password"), s => s.Password.As("Username"));

        foreach (SimpleTable simpleTable in values)
        {
            Assert.That(simpleTable.Username.Contains("P"));
            Assert.That(simpleTable.Password.Contains("u"));
        }
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
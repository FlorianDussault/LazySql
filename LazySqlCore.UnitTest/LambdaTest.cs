using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Functions;
using LazySql.Engine.Client.Query;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

public class LambdaTest
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
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());
        int bot_id = 0;
        for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
        {
            SimpleTable? st = new SimpleTable()
            {
                Username = $"U{i + 1}",
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
        Assert.That(LazyClient.Select<SimpleTable>().ToList().Count(), Is.EqualTo(COUNT_SIMPLE_TABLE));
        Assert.That(LazyClient.Select<ChildTable>().ToList().Count(),
            Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE));
    }

    [Test]
    public void GetSimple()
    {
        AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>().Where(s => s.Id < 10))
        {
            Assert.Less(simpleTable.Id, 10);
        }

    }

    [Test]
    public void Linq()
    {
        AddSimpleTables();

        int valInt = 1;
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>().Where(s => s.Id < 9 + valInt))
        {
            Assert.Less(simpleTable.Id, 10);
        }

        {
            string valString = "U";

            List<SimpleTable> simpleTables =
                LazyClient.Select<SimpleTable>().Where(s => s.Username == valString + "10").ToList();

            Assert.That(1, Is.EqualTo(simpleTables.Count));
            Assert.That("U10", Is.EqualTo(simpleTables[0].Username));
        }
    }



    [Test]
    public void FunctionLike()
    {
        AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>().Where(s =>
                     LzFunctions.Like(s.Id, "%1%") && LzFunctions.Like(s.Id, "%5")))
        {
            Assert.IsTrue(simpleTable.Id.ToString().Contains("1"));
            Assert.IsTrue(simpleTable.Id.ToString().Contains("5"));
        }
    }

    [Test]
    public void FunctionNotLike()
    {
        AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>().Where(s =>
                     LzFunctions.NotLike(s.Id, "%1%") && LzFunctions.NotLike(s.Id, "%5")))
        {
            Assert.IsFalse(simpleTable.Id.ToString().Contains("1"));
            Assert.IsFalse(simpleTable.Id.ToString().Contains("5"));
        }
    }

    [Test]
    public void FunctionIsDate()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "NOT A DATE"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where(s => LzFunctions.IsDate(s.Username) == 1);
            Assert.IsEmpty(values);
        }

        simpleTable = new()
        {
            Username = "2022-09-08"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where(s => LzFunctions.IsDate(s.Username) == 1);
            Assert.That(1, Is.EqualTo(values.Count()));
        }
    }

    [Test]
    public void FunctionGetDate()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.IsDate(LzFunctions.GetDate()) == 1);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionDay()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int day = DateTime.Now.Day;
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.Day(LzFunctions.GetDate()) == day);
            Assert.IsNotEmpty(values);
        }
    }



    [Test]
    public void FunctionMonth()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int month = DateTime.Now.Month;
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.Month(LzFunctions.GetDate()) == month);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionYear()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int year = DateTime.Now.Year;
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.Year(LzFunctions.GetDate()) == year);
            Assert.IsNotEmpty(values);
        }
    }

    [Test]
    public void FunctionDateAdd()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "U"
        };
        simpleTable.Insert();

        {
            int year = DateTime.Now.Year + 1;
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) =>
                LzFunctions.Year(LzFunctions.DateAdd(LzDatePart.Year, 1, LzFunctions.GetDate())) == year);
            Assert.IsNotEmpty(values);

            year = DateTime.Now.Year - 1;
            ILazyEnumerable<SimpleTable>? values2 = LazyClient.Select<SimpleTable>().Where((s) =>
                LzFunctions.Year(LzFunctions.DateAdd(LzDatePart.Year, -1, LzFunctions.GetDate())) == year);
            Assert.IsNotEmpty(values2);
        }
    }

    [Test]
    public void FunctionDateDiff()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO WORLD"
        };
        simpleTable.Insert();

        {
            DateTime start = DateTime.Now;
            DateTime end = start.AddYears(8);
            ILazyEnumerable<SimpleTable>? values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.DateDiff(LzDatePart.Year, start, end) == 8);
            Assert.IsNotEmpty(values);
            values = LazyClient.Select<SimpleTable>().Where((s) => LzFunctions.DateDiff(LzDatePart.Year, end, start) == -8);
            Assert.IsNotEmpty(values);

            //year = DateTime.Now.Year - 1;
            //var values2 = LazyClient.Get<SimpleTable>((s) => LzFunctions.Year(LzFunctions.DateAdd(LzFunctions.LzDatePart.Year, -1, LzFunctions.GetDate())) == 8);
            //Assert.IsNotEmpty(values2);
        }
    }

    [Test]
    public void FunctionConcat()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s =>
            LzFunctions.Concat(s.Username, " ", s.Password) == "HELLO WORLD"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s =>
            LzFunctions.Concat(s.Username, " ", s.Password) == "HELLOWORLD"));
    }

    [Test]
    public void FunctionConcatPlus()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => s.Username + " " + s.Password == "HELLO WORLD"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => s.Username + " " + s.Password == "HELLO2WORLD"));
    }

    [Test]
    public void FunctionConcatWs()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s =>
            LzFunctions.ConcatWs("@", s.Username, s.Password, "!") == "HELLO@WORLD@!"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s =>
            LzFunctions.ConcatWs("@", s.Username, s.Password, "!") == "HELLOWORLD@!"));
    }

    [Test]
    public void FunctionAscii()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Ascii(s.Username) == 72));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Ascii(s.Username) == 12));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Ascii(s.Password) == 87));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Ascii(s.Password) == 93));
    }

    [Test]
    public void FunctionChar()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Char(72) == "H"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Char(72) == "W"));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Char(87) == "W"));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.Char(87) == "H"));
    }

    [Test]
    public void FunctionCharIndex()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "H",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex(null, "world", 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("hello", null, 2) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("hello", "world", null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("world", "hello world", 0) == 7));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("world", "hello world", 0) == 8));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("hello", "hello world", 0) == 1));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.CharIndex("hello", "hello world", 2) == 0));
    }

    [Test]
    public void FunctionDataLength()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "W"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.DataLength(null) == null));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.DataLength(-1) == 4));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.DataLength(s.Username) == 5));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>().Where(s => LzFunctions.DataLength(s.Username) == 4));
    }

    [Test]
    public void CSharpStringJoin()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s=>string.Join("@", s.Username, s.Password) == "HELLO@WORLD"));
    }


    [Test]
    public void CSharpStringFormat()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        SimpleTable simpleTable = new()
        {
            Username = "HELLO WORLD, HELLO",
            Password = "WORLD"
        };
        simpleTable.Insert();

        simpleTable.Username += $" {simpleTable.Id}!";
        simpleTable.Update();

        string hello = "HELLO";

        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>().Where(s => s.Username == $"{hello} WORLD, {hello} {simpleTable.Id}!"));
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
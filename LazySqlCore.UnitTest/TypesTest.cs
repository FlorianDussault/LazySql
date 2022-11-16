using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;
using System.Xml.Linq;
using static System.Single;

namespace LazySqlCore.UnitTest;

public class TypesTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    private void Reset()
    {
        LazyClient.Truncate<TypesTable>();
    }

    [Test]
    public void TypeBigint()
    {
        Reset();
        long val = long.MaxValue;
        new TypesTable() {TypeBigint = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeBigint));
    }

    [Test]
    public void TypeBinary()
    {
        Reset();
        byte[] val = new byte[50];
        Random rand = new Random(DateTime.Now.Millisecond);
        for (int i = 0; i < val.Length; i++)
        {
            val[i] = (byte)rand.Next(0, 255);
        }
        new TypesTable() { TypeBinary = val }.Insert();
        byte[] result = LazyClient.Select<TypesTable>().First().TypeBinary!;

        for (int i = 0; i < val.Length; i++)
        {
            Assert.That(val[i], Is.EqualTo(result[i]));
        }
    }
    [Test]
    public void TypeBit()
    {
        Reset();
        bool val = true;
        new TypesTable() { TypeBit = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeBit));
    }

    [Test]
    public void TypeChar()
    {
        Reset();
        string val = "hello";
        new TypesTable() { TypeChar = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeChar?.Trim()));
    }

    [Test]
    public void TypeDate()
    {
        Reset();
        DateTime val = DateTime.Today;
        new TypesTable() { TypeDate = val }.Insert();
        Assert.That(LazyClient.Select<TypesTable>().First().TypeDate,Is.EqualTo(val));
    }

    [Test]
    public void TypeDateTime2()
    {
        Reset();
        DateTime val = DateTime.Now;
        new TypesTable() { TypeDateTime2 = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeDateTime2));
    }

    [Test]
    public void TypeDatetimeoffset()
    {
        Reset();
        DateTimeOffset val = DateTimeOffset.Now;
        new TypesTable() { TypeDatetimeoffset = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeDatetimeoffset));
    }

    [Test]
    public void TypeDecimal()
    {
        Reset();
        decimal val = 1.5M;
        new TypesTable() { TypeDecimal = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeDecimal));
    }

    [Test]
    public void TypeFloat()
    {
        Reset();
        double val = 1.14F;
        new TypesTable() { TypeFloat = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeFloat));
    }
       
    [Test]
    public void TypeImage()
    {
        Reset();
        byte[] val = new byte[50];
        Random rand = new Random(DateTime.Now.Millisecond);
        for (int i = 0; i < val.Length; i++)
        {
            val[i] = (byte)rand.Next(0, 255);
        }
        new TypesTable() { TypeImage = val }.Insert();
        byte[] result = LazyClient.Select<TypesTable>().First().TypeImage!;

        for (int i = 0; i < val.Length; i++)
        {
            Assert.That(val[i], Is.EqualTo(result![i]));
        }
    }
 
    [Test]
    public void TypeInt()
    {
        Reset();
        int val = int.MaxValue;
        new TypesTable() { TypeInt = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeInt));
    }
    [Test]
    public void TypeMoney()
    {
        Reset();
        decimal val = 1.14M;
        new TypesTable() { TypeMoney = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeMoney));
    }

    [Test]
    public void TypeNtext()
    {
        Reset();
        string val = "ntext value";
        new TypesTable() { TypeNtext = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeNtext));
    }

    [Test]
    public void TypeNumeric()
    {
        Reset();
        decimal val = 1.14M;
        new TypesTable() { TypeDecimal = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeDecimal));
    }


    [Test]
    public void TypeReal()
    {
        Reset();
        float val = Epsilon;
        new TypesTable() { TypeReal = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeReal));
    }

    [Test]
    public void TypeSmalldatime()
    {
        Reset();
        DateTime val = DateTime.Today;
        new TypesTable() { TypeSmalldatime = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeSmalldatime));
    }
    [Test]
    public void TypeSmallint()
    {
        Reset();
        short val = short.MaxValue;
        new TypesTable() { TypeSmallint = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeSmallint));
    }
    [Test]
    public void TypeSmallmoney()
    {
        Reset();
        decimal val = 1.14M;
        new TypesTable() { TypeSmallmoney = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeSmallmoney));
    }
    [Test]
    public void TypeSqlVariant()
    {
        Reset();
        string? val ="VARIANT";
        new TypesTable() { TypeSqlVariant = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeSqlVariant));
    }
    [Test]
    public void TypeText()
    {
        Reset();
        string? val = "TYPE TEXT";
        new TypesTable() { TypeText = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeText));
    }

    [Test]
        
    public void TypeTime()
    {
        Reset();
        TimeSpan val = DateTime.Now.TimeOfDay;
        new TypesTable() { TypeTime = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeTime));
    }


    [Test]
    public void TypeTinyint()
    {
        Reset();
        byte val = byte.MaxValue;
        new TypesTable() { TypeTinyint = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeTinyint));
    }

    [Test]
    public void TypeUniqueidentifier()
    {
        Reset();
        Guid val = Guid.NewGuid();
        new TypesTable() { TypeUniqueidentifier = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeUniqueidentifier));
    }

    [Test]
    public void TypeXml()
    {
        Reset();
        string val = XDocument.Parse("<hello><unit><test>123</test></unit></hello>").ToString(SaveOptions.DisableFormatting);
        new TypesTable() { TypeXml = val }.Insert();
        Assert.That(val, Is.EqualTo(LazyClient.Select<TypesTable>().First().TypeXml));
    }



}
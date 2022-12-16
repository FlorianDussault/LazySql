using LazySql;
using LazySqlCore.UnitTest.Tables;
using Microsoft.Data.SqlClient;

namespace LazySqlCore.UnitTest;

internal static class ClientTest
{
#if APPVEYOR
    private const string DatabaseName = "LazySql";
    private const string DbConnectionString = "Server=(local)\\SQL2019;Database=LazySql;User ID=sa;Password=Password12!;TrustServerCertificate=True";
    private const string MasterConnectionString = "Server=(local)\\SQL2019;Database=master;User ID=sa;Password=Password12!;TrustServerCertificate=True";
#else
    private const string DatabaseName = "LazySql";
    private const string DbConnectionString = "Server=localhost\\sqlexpress;Database=LazySql;TrustServerCertificate=Yes;Trusted_Connection=True";
    private const string MasterConnectionString = "Server=localhost\\sqlexpress;Database=master;TrustServerCertificate=Yes;Trusted_Connection=True";
#endif

    private static readonly string[] Tables = new[] {"dbo.child_table", "dbo.extended_table", "dbo.simple_table", "dbo.subchild_table", "dbo.types", "dbo.hierarchy_table", "lazys.WithoutKeys", "lazys.tablePrimary", "lazys.hierarchy_schema" };

    public static void Initialize()
    {
        BuildTestDb();

        if (!LazyClient.Initialized)
            LazyClient.Initialize(DbConnectionString, typeof(TypesTable), typeof(SimpleTable), typeof(ChildTable),
                typeof(ExtendedTable), typeof(SubChildTable), typeof(PrimaryValue), typeof(WithoutKeys));
    }

    private static void BuildTestDb()
    {
        using (SqlConnection sqlConnection = new(MasterConnectionString))
        {
            sqlConnection.Open();
            CreateDatabase(sqlConnection);
        }

        using (SqlConnection sqlConnection = new(DbConnectionString))
        {
            sqlConnection.Open();

            DeleteStoredProcedures(sqlConnection);
            DeleteTables(sqlConnection);
            CreateSchemas(sqlConnection);
            CreateTables(sqlConnection);
            CreateStoredProcedures(sqlConnection);
        }
    }

    private static void CreateSchemas(SqlConnection sqlConnection)
    {
        Execute(sqlConnection, "IF NOT EXISTS ( SELECT  *\r\n                FROM    sys.schemas\r\n                WHERE   name = N'lazys' )\r\n    EXEC('CREATE SCHEMA [lazys]');");
    }

    private static void DeleteTables(SqlConnection sqlConnection)
    {
        bool hasFailed = true;
        while (hasFailed)
        {
            hasFailed = false;
            foreach (string table in Tables)
            {
                try
                {
                    Execute(sqlConnection, $"IF OBJECT_ID('{table}', 'U') IS NOT NULL \r\n  DROP TABLE {table};");
                }
                catch (Exception)
                {
                    hasFailed = true;
                }
            }

        }

    }

    private static void DeleteStoredProcedures(SqlConnection sqlConnection)
    {
        Execute(sqlConnection, "IF OBJECT_ID('simple_procedure', 'P') IS NOT NULL\r\nDROP PROC simple_procedure");
        Execute(sqlConnection, "IF OBJECT_ID('[lazys].simple_procedureSchema', 'P') IS NOT NULL\r\nDROP PROC [lazys].simple_procedureSchema");
    }

    private static void CreateTables(SqlConnection sqlConnection)
    {
        Execute(sqlConnection,
            "CREATE TABLE [dbo].[child_table]([id] [int] NOT NULL,[simple_table_id] [int] NOT NULL, [texte] [nchar](10) NULL, CONSTRAINT [PK_child_table] PRIMARY KEY CLUSTERED ([id] ASC) ON [PRIMARY]);");

        Execute(sqlConnection, "CREATE TABLE [dbo].[extended_table](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[key] [varchar](50) NOT NULL,\r\n\t[value] [int] NULL,\r\n CONSTRAINT [PK_extended_table] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n))");

        Execute(sqlConnection,
            "CREATE TABLE [dbo].[simple_table](\r\n\t[user_id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[username] [varchar](50) NULL,\r\n\t[password] [varchar](50) NULL,\r\n\t[extended_key] [varchar](50) NULL,\r\n CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED \r\n(\r\n\t[user_id] ASC\r\n))");

        Execute(sqlConnection, "CREATE TABLE [dbo].[subchild_table](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[parent_id] [int] NULL,\r\n\t[value] [varchar](100) NOT NULL,\r\n CONSTRAINT [PK_hierarchy_table] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n))");

        Execute(sqlConnection, "CREATE TABLE [dbo].[types](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[type_bigint] [bigint] NULL,\r\n\t[type_binary] [binary](50) NULL,\r\n\t[type_bit] [bit] NULL,\r\n\t[type_char] [char](10) NULL,\r\n\t[type_date] [date] NULL,\r\n\t[type_datetime2] [datetime2](7) NULL,\r\n\t[type_datetimeoffset] [datetimeoffset](7) NULL,\r\n\t[type_decimal] [decimal](18, 2) NULL,\r\n\t[type_float] [float] NULL,\r\n\t[type_geography] [geography] NULL,\r\n\t[type_hierarchyid] [hierarchyid] NULL,\r\n\t[type_image] [image] NULL,\r\n\t[type_int] [int] NULL,\r\n\t[type_money] [money] NULL,\r\n\t[type_ntext] [ntext] NULL,\r\n\t[type_numeric] [numeric](18, 4) NULL,\r\n\t[type_real] [real] NULL,\r\n\t[type_smalldatime] [smalldatetime] NULL,\r\n\t[type_smallint] [smallint] NULL,\r\n\t[type_smallmoney] [smallmoney] NULL,\r\n\t[type_sql_variant] [sql_variant] NULL,\r\n\t[type_text] [text] NULL,\r\n\t[type_time] [time](7) NULL,\r\n\t[type_timestamp] [timestamp] NULL,\r\n\t[type_tinyint] [tinyint] NULL,\r\n\t[type_uniqueidentifier] [uniqueidentifier] NULL,\r\n\t[type_xml] [xml] NULL,\r\n CONSTRAINT [PK_types] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n))");

        Execute(sqlConnection, "CREATE TABLE [dbo].[hierarchy_table](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[parent_id] [int] NULL,\r\n\t[Name] [nvarchar](50) NULL,\r\n CONSTRAINT [PK_hierarchy_table_1] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]");
        Execute(sqlConnection, "ALTER TABLE [dbo].[hierarchy_table]  WITH CHECK ADD  CONSTRAINT [FK_hierarchy_table_hierarchy_table] FOREIGN KEY([parent_id])\r\nREFERENCES [dbo].[hierarchy_table] ([id])");
        Execute(sqlConnection, "ALTER TABLE [dbo].[hierarchy_table] CHECK CONSTRAINT [FK_hierarchy_table_hierarchy_table]");

        Execute(sqlConnection, "ALTER TABLE [dbo].[child_table]  WITH NOCHECK ADD  CONSTRAINT [FK_child_table_simple_table] FOREIGN KEY([simple_table_id]) REFERENCES [dbo].[simple_table] ([user_id])");
        Execute(sqlConnection, "ALTER TABLE [dbo].[child_table] CHECK CONSTRAINT [FK_child_table_simple_table]");
        Execute(sqlConnection, "ALTER TABLE [dbo].[subchild_table]  WITH CHECK ADD  CONSTRAINT [FK_subchild_table_child_table] FOREIGN KEY([parent_id]) REFERENCES [dbo].[child_table] ([id])");
        Execute(sqlConnection, "ALTER TABLE [dbo].[subchild_table] CHECK CONSTRAINT [FK_subchild_table_child_table]");

        Execute(sqlConnection, "CREATE TABLE [lazys].[tablePrimary](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[value] [varchar](50),\r\n CONSTRAINT [PK_tablePrimary] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]");
        Execute(sqlConnection, "CREATE TABLE [lazys].[WithoutKeys](\r\n\t[Name] [varchar](50) NULL,\r\n\t[Age] [int] NULL\r\n) ON [PRIMARY]");
        Execute(sqlConnection, "CREATE TABLE [lazys].[hierarchy_schema](\r\n\t[id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[parent_id] [int] NULL,\r\n\t[Name] [nvarchar](50) NULL,\r\n CONSTRAINT [PK_hierarchy_table_1] PRIMARY KEY CLUSTERED \r\n(\r\n\t[id] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]\r\n\r\nALTER TABLE [lazys].[hierarchy_schema]  WITH CHECK ADD  CONSTRAINT [FK_hierarchy_table_hierarchy_schema] FOREIGN KEY([parent_id])\r\nREFERENCES [lazys].[hierarchy_schema] ([id])\r\n\r\n\r\nALTER TABLE [lazys].[hierarchy_schema] CHECK CONSTRAINT [FK_hierarchy_table_hierarchy_schema]");
    }

    private static void CreateStoredProcedures(SqlConnection sqlConnection)
    {
        Execute(sqlConnection, "CREATE PROCEDURE [dbo].[simple_procedure] \r\n\t-- Bind the parameters for the stored procedure here\r\n\t@Count INT, \r\n\t@Prefix NVARCHAR(MAX),\r\n\t@IdMax INT OUTPUT,\r\n\t@IdMin INT OUTPUT\r\nAS\r\nBEGIN\r\n\t-- SET NOCOUNT ON added to prevent extra result sets from\r\n\t-- interfering with SELECT statements.\r\n\tSET NOCOUNT ON;\r\n\r\n\tIF @Count IS NULL\r\n\tBEGIN\r\n\t\tgoto eos;\r\n\tEND\r\n\r\n\tDECLARE @cnt INT = 0;\r\n\r\n\tSELECT @IdMin = MAX(User_id) FROM simple_table\r\n\r\n\tWHILE @cnt < @Count\r\n\tBEGIN\r\n\t   INSERT INTO dbo.simple_table (username, [password]) VALUES (CONCAT(@Prefix, CAST(NEWID() AS NVARCHAR(36))), 'pwd')\r\n\t   SET @cnt = @cnt + 1\r\n\tEND\r\n\tSELECT @idMax = MAX(User_id) FROM simple_table\r\n    -- Insert statements for procedure here\r\n\tSELECT * FROM simple_table where [user_id] > @IdMin ORDER BY [user_id] ASC\r\n\tSELECT [user_id], username FROM simple_table where [user_id] > @IdMin ORDER BY [user_id] DESC\r\n\treturn -678\r\n\teos:\r\n\treturn null\r\nEND");
        Execute(sqlConnection, "CREATE PROCEDURE [lazys].[simple_procedureSchema] \r\n\t-- Bind the parameters for the stored procedure here\r\n\t@Count INT, \r\n\t@Prefix NVARCHAR(MAX),\r\n\t@IdMax INT OUTPUT,\r\n\t@IdMin INT OUTPUT\r\nAS\r\nBEGIN\r\n\t-- SET NOCOUNT ON added to prevent extra result sets from\r\n\t-- interfering with SELECT statements.\r\n\tSET NOCOUNT ON;\r\n\r\n\tIF @Count IS NULL\r\n\tBEGIN\r\n\t\tgoto eos;\r\n\tEND\r\n\r\n\tDECLARE @cnt INT = 0;\r\n\r\n\tSELECT @IdMin = MAX(User_id) FROM simple_table\r\n\r\n\tWHILE @cnt < @Count\r\n\tBEGIN\r\n\t   INSERT INTO dbo.simple_table (username, [password]) VALUES (CONCAT(@Prefix, CAST(NEWID() AS NVARCHAR(36))), 'pwd')\r\n\t   SET @cnt = @cnt + 1\r\n\tEND\r\n\tSELECT @idMax = MAX(User_id) FROM simple_table\r\n    -- Insert statements for procedure here\r\n\tSELECT * FROM simple_table where [user_id] > @IdMin ORDER BY [user_id] ASC\r\n\tSELECT [user_id], username FROM simple_table where [user_id] > @IdMin ORDER BY [user_id] DESC\r\n\treturn -678\r\n\teos:\r\n\treturn null\r\nEND");
    }

    private static void Execute(SqlConnection sqlConnection, string sql)
    {
        using SqlCommand sqlCommand = new(sql, sqlConnection);
        sqlCommand.ExecuteNonQuery();
    }

    private static void CreateDatabase(SqlConnection sqlConnection)
    {
        Execute(sqlConnection, $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{DatabaseName}') BEGIN CREATE DATABASE {DatabaseName}; END;");
    }

    internal static void AddSimpleTables()
    {
        const int COUNT_SIMPLE_TABLE = 20;
        const int COUNT_CHILD_TABLE = 20;
        // Clear Table
        CleanTables();
        // Bind values
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());
        int bot_id = 0;
        for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
        {
            SimpleTable st = new()
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
        Assert.That(COUNT_SIMPLE_TABLE == LazyClient.Select<SimpleTable>().ToList().Count());
        Assert.That(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE == LazyClient.Select<ChildTable>().ToList().Count());
    }

    internal static void CleanSimpleTable()
    {
        LazyClient.Delete<ChildTable>(null, SqlQuery.Empty);
        LazyClient.Delete<SimpleTable>(null, SqlQuery.Empty);
        LazyClient.ExecuteNonQuery("DBCC CHECKIDENT ('simple_table', RESEED, 0);");
    }

    internal static void CleanTables()
    {
        CleanSimpleTable();
        LazyClient.Truncate<PrimaryValue>();
        LazyClient.Truncate<WithoutKeys>();
        LazyClient.Delete<SubChildTable>(null, SqlQuery.Empty);
        LazyClient.ExecuteNonQuery("DBCC CHECKIDENT ('subchild_table', RESEED, 0);");
        LazyClient.Truncate<ExtendedTable>();
    }

    public static void AddSchemaRows()
    {
        CleanTables();
        for (int i = 0; i < 20; i++)
        {
            Assert.That(new PrimaryValue {Value = "U_" + i}.Insert(), Is.EqualTo(1));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(LazyClient.Insert("lazys", "WithoutKeys", new WithoutKeys(){Age = i, Name = $"N_{i}"}), Is.EqualTo(1));
        }
    }
}
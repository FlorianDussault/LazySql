﻿
using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("simple_table")]
    public class SimpleTable : LazyBase
    {
        [LazyColumn("user_id", SqlType.Int32)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn("username", SqlType.String)]
        public string Username { get; set; }

        [LazyColumn("password", SqlType.String)]
        public string Password { get; set; }

        [LazyColumn("extended_key", SqlType.String)]
        public string ExtendedKey { get; set; }
        public List<ChildTable> ChildTables { get; set; }
        public ExtendedTable Extended { get; set; }

        public override void InitializeTable()
        {
            AddOneToMany<SimpleTable, ChildTable>(nameof(ChildTables), expression: (p, c) => p.Id == c.ParentId);
            AddOneToOne<SimpleTable, ExtendedTable>(nameof(Extended), (p, e) => p.ExtendedKey == e.Key);
        }
    }
}

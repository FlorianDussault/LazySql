﻿namespace LazySql.Engine.Client.Query;

/// <summary>
/// Query
/// </summary>
internal interface IQuery
{
    void Build(QueryBase queryBase);
}
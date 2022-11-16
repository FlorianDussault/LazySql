// Global using directives

global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Data;
global using System.Dynamic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text;
global using System.Text.RegularExpressions;
global using LazySql.Engine.Attributes;
global using LazySql.Engine.Client;

#if NETCORE
global using Microsoft.Data.SqlClient;
#elif NETSTANDARD
global using Microsoft.Data.SqlClient;
#else
global using System.Data.SqlClient;
#endif

global using LazySql.Engine.Client.Functions;
global using LazySql.Engine.Client.Lambda;
global using LazySql.Engine.Client.Query;
global using LazySql.Engine.Connector;
global using LazySql.Engine.Definitions;
global using LazySql.Engine.Enums;
global using LazySql.Engine.Exceptions;
global using LazySql.Engine.Helpers;


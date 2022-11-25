// Global using directives

global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Data;
global using System.Dynamic;
global using System.Globalization;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text;
global using System.Text.RegularExpressions;

#if NETCORE
global using Microsoft.Data.SqlClient;
#elif NETSTANDARD
global using Microsoft.Data.SqlClient;
#else
global using System.Data.SqlClient;
#endif
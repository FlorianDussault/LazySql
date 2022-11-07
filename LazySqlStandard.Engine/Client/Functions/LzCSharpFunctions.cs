
using LazySql.Engine.Client.Lambda;
using LazySql.Engine.Client.Query;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using LazySql.Engine.Enums;
using LazySql.Engine.Helpers;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace LazySql.Engine.Client.Functions
{
    internal class LzCSharpFunctions : LambdaFunctionParser
    {
        internal override void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            MethodInfo method = GetType().GetTypeInfo()
                .GetDeclaredMethod($"{expression.Method.DeclaringType.Name}_{expression.Method.Name}");
            if (method == null)
                throw new NotImplementedException(
                    $"Function '{expression.Method.DeclaringType.Name}.{expression.Method.Name}' is missing");
            method.Invoke(this, new object[] {expression, lambdaParser, builder});
        }

        private void String_Join(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            new LzFunctions().ParseConcatWs(expression, lambdaParser, builder);
        }

        private void String_Format(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            Regex regex = new ("({[0-9]*})");
            if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object objFormat))
                throw new NotSupportedException();
            string[] values = regex.Split(objFormat.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            builder.Append(" CONCAT(");
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];

                if (regex.IsMatch(value))
                {
                    int index = int.Parse(value.Substring(1, value.Length - 2)) + 1;
                    if (index >= expression.Arguments.Count)
                        throw new ArgumentOutOfRangeException();
                    if (lambdaParser.GetValueFromExpression(expression.Arguments[index], out object argObject))
                    {
                        string argumentName = builder.RegisterArgument(argObject.GetType().ToSqlType(), argObject);
                        builder.Append(argumentName);
                    }
                    else
                    {
                        lambdaParser.ParseExpression(expression.Arguments[index]);
                    }
                }
                else
                {
                    string argumentName = builder.RegisterArgument(SqlType.NVarChar, value);
                    builder.Append(argumentName);
                }

                if (i + 1 < values.Length)
                    builder.Append(", ");
            }

            builder.Append(")");
            
        }
    }
}


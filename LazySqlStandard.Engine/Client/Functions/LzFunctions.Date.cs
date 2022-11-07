using LazySql.Engine.Client.Lambda;
using LazySql.Engine.Client.Query;
using System;
using System.Linq.Expressions;

namespace LazySql.Engine.Client.Functions
{
    public partial class LzFunctions : LambdaFunctionParser
    {
        public static int IsDate(string value)
        {
            return DateTime.TryParse(value, out _) ? 1 : 0;
        }

        private void ParseIsDate(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" ISDATE(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static string GetDate()
        {
            return DateTime.Now.ToString();
        }

        private void ParseGetDate(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" GETDATE() ");
        }

        public static int Day(object obj)
        {
            return ((DateTime)obj).Day;
        }

        private void ParseDay(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" DAY(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static int Month(object obj)
        {
            return ((DateTime)obj).Month;
        }
        private void ParseMonth(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" MONTH(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static int Year(object obj)
        {
            return ((DateTime)obj).Year;
        }

        private void ParseYear(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" YEAR(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static string DateAdd(LzDatePart lzDatePart, int increment, object obj)
        {
            return DateTime.Now.ToString();
        }

        private void ParseDateAdd(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" DATEADD(");
            if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object arg1) ||
                arg1 is not LzDatePart datePart)
                throw new NotImplementedException();
            builder.Append($"{Enum.GetName(typeof(LzDatePart), datePart)}, ");
            builder.Append(expression.Arguments[1]);
            builder.Append(", ");
            builder.Append(expression.Arguments[2]);
            //lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static int DateDiff(LzDatePart lzDatePart, DateTime start, DateTime end)
        {
            return (end - start).Days;
        }

        private void ParseDateDiff(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" DATEDIFF(");
            if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object arg1) ||
                arg1 is not LzDatePart datePart)
                throw new NotImplementedException();
            builder.Append($"{Enum.GetName(typeof(LzDatePart), datePart)}, ");
            builder.Append(expression.Arguments[1]);
            builder.Append(", ");
            builder.Append(expression.Arguments[2]);
            builder.Append(") ");
        }
    }
}

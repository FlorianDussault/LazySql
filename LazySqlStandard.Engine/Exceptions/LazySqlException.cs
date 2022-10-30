using System;

namespace LazySql.Engine.Exceptions
{
    public class LazySqlException : Exception
    {
        public LazySqlException(Exception exception) : base(exception.Message, exception)
        {

        }

        public LazySqlException(string message, Exception exception) : base(message, exception)
        {

        }

        public LazySqlException(string message) : base(message)
        {

        }
    }
}
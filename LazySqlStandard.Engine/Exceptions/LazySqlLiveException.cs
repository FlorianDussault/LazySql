namespace LazySql;

public class LazySqlLiveException : LazySqlException
{
    public LazySqlLiveException(string message) : base(message)
    {
    }
}
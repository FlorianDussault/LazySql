namespace LazySql.Engine.Client.Functions;

/// <summary>
/// SQL Date Part
/// </summary>
public enum LzDatePart
{
    /// <summary>
    /// Year
    /// </summary>
    Year = 0,
    /// <summary>
    /// Quarter
    /// </summary>
    Quarter = 1,
    /// <summary>
    /// Month
    /// </summary>
    Month = 2,
    /// <summary>
    /// Day
    /// </summary>
    Day = 3,
    /// <summary>
    /// Hour
    /// </summary>
    Hour = 4,
    /// <summary>
    /// Minute
    /// </summary>
    Minute = 5,
    /// <summary>
    /// Second
    /// </summary>
    Second = 6,
    /// <summary>
    /// Millisecond
    /// </summary>
    Millisecond = 7,
    /// <summary>
    /// Day Of Year
    /// </summary>
    DayOfYear = 8,
    /// <summary>
    /// Week Number
    /// </summary>
    Week = 9,
    /// <summary>
    /// Microsecond
    /// </summary>
    Microsecond = 10,
    /// <summary>
    /// Nanosecond
    /// </summary>
    Nanosecond = 11,
    /// <summary>
    /// TZOffset
    /// </summary>
    TzOffset = 12,
    /// <summary>
    /// ISO Week
    /// </summary>
    Iso_Week = 13
}
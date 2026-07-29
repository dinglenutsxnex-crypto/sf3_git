using System;

public static class DateTimeExtensions
{
	public static long GetUnixTimeStampTicks(this DateTime currentDateTime)
	{
		long ticks = new DateTime(1970, 1, 1).Ticks;
		return currentDateTime.Ticks - ticks;
	}

	public static long GetUnixTimeStampMilliseconds(this DateTime currentDateTime)
	{
		return currentDateTime.GetUnixTimeStampTicks() / 10000;
	}
}

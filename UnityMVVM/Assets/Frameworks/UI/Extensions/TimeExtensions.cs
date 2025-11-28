using System;

namespace MVVMToolkit.UI
{
    public static class TimeExtensions
    {
        public static string FormatTimeSpan(this TimeSpan timeLeft)
        {
            if (timeLeft.TotalHours >= 1)
            {
                int totalHours = (int)timeLeft.TotalHours;
                int minutes = timeLeft.Minutes;
                int seconds = timeLeft.Seconds + 1;
                return $"{totalHours}h {minutes}m {seconds}s";
            }
            else if (timeLeft.TotalMinutes >= 1)
            {
                int minutes = timeLeft.Minutes;
                int seconds = timeLeft.Seconds + 1;
                return $"{minutes}m {seconds}s";
            }
            else if (timeLeft.TotalSeconds > 0)
            {
                int seconds = timeLeft.Seconds + 1;
                return $"{seconds}s";
            }
            else
            {
                return "0s";
            }
        }
    }
}
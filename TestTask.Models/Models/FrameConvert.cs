namespace TestTask.Models.Models
{
    public static class FrameConvert
    {
        public static string ConvertPeriodIntSecToString(int periodInSec)
        {
            var periodDictionary = new Dictionary<int, string>()
            {
                [60] = "1m",
                [300] = "5m",
                [900] = "15m",
                [1800] = "30m",
                [3600] = "1h",
                [10800] = "3h",
                [21600] = "6h",
                [43200] = "12h",
                [86400] = "1D",
                [604800] = "1W",
                [1209600] = "14D",
                [2592000] = "1M"
            };

            var value = periodDictionary.GetValueOrDefault(periodInSec);

            return value is null ? throw new Exception("Not Found period") : value;
        }
    }
}

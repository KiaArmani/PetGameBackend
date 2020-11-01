using System;

namespace PetGameBackend.Calculators
{
    public static class ValueByTickRateCalculator
    {
        public static int GetValueByTickRateAndDateTime(int lastValue, DateTime lastDateTime, int tickRate,
            bool increase = false)
        {
            var currentTime = DateTime.Now.ToUniversalTime();

            var distanceInMilliseconds = (currentTime - lastDateTime).TotalMilliseconds;
            var valueChange = Convert.ToInt32(Math.Floor(distanceInMilliseconds / tickRate));
            int newValue;

            if (increase)
                newValue = lastValue + valueChange;
            else
                newValue = lastValue - valueChange;

            return newValue <= 0 ? 0 : newValue;
        }
    }
}
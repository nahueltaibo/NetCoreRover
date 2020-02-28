namespace Robot.Utils
{
    public static class ValueMapper
    {
        public static double Map(double value, double inMin, double inMax, double outMin, double outMax)
        {
            // Check that the value is at least inMin
            if (value < inMin)
            {
                value = inMin;
            }

            // Check that the value is at most inMax
            if (value > inMax)
            {
                value = inMax;
            }

            //return (value - inMin) * (outMax - outMin);
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}

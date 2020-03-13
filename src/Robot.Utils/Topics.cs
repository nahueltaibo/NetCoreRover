namespace Robot.Utils
{
    public static class Topics
    {
        public static class Command
        {
            public const string RemoteControl = "command/remotecontrol";
        }

        public static class Motion
        {
            public const string Velocity = "motion/velocity";
        }

        public static class Sensor
        {
            public static class Sonar
            {
                public const string Distance = "sensor/sonar/distance";
            }
            public static class Switch
            {
                public const string SinglePoleSwitch = "sensor/switch/singlepole";
            }
        }
    }
}

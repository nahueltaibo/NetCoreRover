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
            /// <summary>
            /// Topic for a Velocity message sent by <see cref="Robot.Controllers.RemoteControl.RemoteControlController"/>
            /// or other input such as an AI course plotter
            /// </summary>
            /// <remarks>This velocity vector is a starting value to estimate a real robot trajectory 
            /// considering all the obstacles discovered</remarks>
            public const string RequestedVelocity = "motion/velocity/requested";

            /// <summary>
            /// Topic for a Velocity message sent by <see cref="Robot.Controllers.Motion.ObstacleAvoidanceController"/>
            /// </summary>
            /// <remarks>This velocity vector is a final robot trajectory considering all the obstacles discovered</remarks>
            public const string AppliedVelocity = "motion/velocity/applied";
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

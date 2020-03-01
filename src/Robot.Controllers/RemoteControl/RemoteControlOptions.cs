using System.Collections.Generic;

namespace Robot.Controllers.RemoteControl
{
    public class RemoteControlOptions
    {
        public int GamepadKeyThrottle { get; set; }

        public int GamepadKeyYaw { get; set; }

        public int KeyboardKeyThrottle { get; set; }

        public int KeyboardKeyYaw { get; set; }
    }
}
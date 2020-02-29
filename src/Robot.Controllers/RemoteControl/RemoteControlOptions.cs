using System.Collections.Generic;

namespace Robot.Controllers.RemoteControl
{
    public class RemoteControlOptions
    {
        public int GamepadTranslationAxisKey { get; set; }

        public int GamepadRotationAxisKey { get; set; }

        public int KeyboardTranslationAxisKey { get; set; }

        public int KeyboardRotationAxisKey { get; set; }
    }
}
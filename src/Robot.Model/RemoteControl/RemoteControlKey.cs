namespace Robot.Model.RemoteControl
{
    public enum RemoteControlKey
    { 
        Invalid = -1,

        /// <summary>
        /// Pushing the left stick to the left or to the right. 
        /// Rotates the robot left or right. Points the front of the robot different directions and helps with changing directions while flying.
        /// </summary>
        Yaw = 0,

        /// <summary>
        /// To increase, push the left stick forwards. To decrease, pull the left stick backwards. This adjusts the altitude, or height, of the robot.
        /// </summary>
        Throttle = 1,

        /// <summary>
        /// Pushing the right stick to the left or right. Rolls the robot, which maneuvers the quadcopter left or right.
        /// </summary>
        Roll = 2,

        /// <summary>
        /// Pushing the right stick forwards or backwards. Tilts the robot, which maneuvers the robot forwards or backwards.
        /// </summary>
        Pitch = 3,

        LeftRight = 4, //6;

        UpDown = 5, //7;

        A = 6, //0;

        B = 7, //1;

        X = 8, //3;

        Y = 9, //4;

        Select = 10,

        Start = 11,

        L1 = 12, //6;

        R1 = 13, //7;

        L2 = 14, //8;

        R2 = 15 //9;
   }
}

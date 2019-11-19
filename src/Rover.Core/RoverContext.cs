using Rover.Core.Hardware.Motors;

namespace Rover.Core
{
    public class RoverContext
    {
        public RoverContext(MotorController motorController)
        {
            MotorController = motorController;
        }

        public MotorController MotorController { get; set; }
    }
}

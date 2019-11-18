namespace Rover.Core.RoverStates
{
    public abstract class RoverState
    {
        public IRoverStateManager RoverStateManager { get; }
        public RoverContext Context { get; }

        public RoverState(IRoverStateManager roverStateManager, RoverContext context)
        {
            RoverStateManager = roverStateManager;
            Context = context;
        }

        public abstract void Update();
    }
}

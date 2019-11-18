using Rover.Core.RoverStates;

namespace Rover.Core
{
    public interface IRoverStateManager
    {
        void SetState<S>() where S : RoverState;

        void Update();
    }
}
using Microsoft.Extensions.Logging;

namespace Rover.Core.RoverStates
{
    public class IddleState : RoverState
    {
        private readonly ILogger<IddleState> _log;

        public IddleState(IRoverStateManager roverStateManager, RoverContext roverContext, ILogger<IddleState> logger) : base(roverStateManager, roverContext)
        {
            this._log = logger;
        }

        public override void Update()
        {
            _log.LogDebug("Updating State...");

            // Set the state to Remote Controlled State to wait for user input
            RoverStateManager.SetState<RemoteControledState>();
        }
    }
}
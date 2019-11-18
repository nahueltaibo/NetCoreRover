using Microsoft.Extensions.Logging;

namespace Rover.Core.RoverStates
{
    public class RemoteControledState : RoverState
    {
        private readonly ILogger<RemoteControledState> _log;

        public RemoteControledState(IRoverStateManager roverStateManager, RoverContext context, ILogger<RemoteControledState> logger) : base(roverStateManager, context)
        {
            this._log = logger;
        }

        public override void Update()
        {
            _log.LogDebug("Updating State...");
        }
    }
}

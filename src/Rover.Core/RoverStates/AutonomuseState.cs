using Microsoft.Extensions.Logging;

namespace Rover.Core.RoverStates
{
    public class AutonomuseState : RoverState
    {
        private readonly ILogger<AutonomuseState> _log;

        public AutonomuseState(IRoverStateManager roverStateManager, RoverContext context, ILogger<AutonomuseState> logger) : base(roverStateManager, context)
        {
            this._log = logger;
        }

        public override void Update()
        {
            _log.LogDebug("Updating State...");
            throw new System.NotImplementedException();
        }
    }
}

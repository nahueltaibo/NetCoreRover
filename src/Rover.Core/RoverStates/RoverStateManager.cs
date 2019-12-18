using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rover.Core.RoverStates;
using System;

namespace Rover.Core
{
    public class RoverStateManager : IRoverStateManager
    {
        private readonly IServiceProvider _services;
        private RoverContext _roverContext;
        private readonly ILogger<RoverStateManager> _logger;
        private RoverState _currentState;

        public RoverStateManager(IServiceProvider services, RoverContext roverContext, ILogger<RoverStateManager> logger)
        {
            _logger = logger;
            _roverContext = roverContext;
            _services = services;
        }

        public void SetState<S>() where S : RoverState
        {
            _logger.LogDebug($"Setting {typeof(S).Name}");
            this._currentState = _services.GetService<S>();
        }

        public void Update()
        {
            if(_currentState== null)
            {
                _logger.LogDebug("Setting IddleState by default");
                this.SetState<IddleState>();
            }

            _logger.LogTrace($"Updating {_currentState.GetType().Name}");
            this._currentState.Update();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Robots
{
    public class RobotsController
    {
        private FakeSDK _sdk;
        private List<Action<IRobot>> _lastActions;

        public virtual FakeSDK SDK => _sdk ?? (_sdk = new FakeSDK());

        public void SendCommands(Guid id, List<Action<IRobot>> actions)
        {
            var robot = GetRobotByGuid(id);

            _lastActions = actions;
            actions.ForEach(p => p?.Invoke(robot));
        }

        public void RepeatCommands(Guid id)
        {
            var robot = GetRobotByGuid(id);
            _lastActions?.ForEach(p => p?.Invoke(robot));
        }

        public FakeSDK.Robot GetRobotByGuid(Guid id)
        {
            var robot = SDK.GetRobots().Where(p => p.Id == id).FirstOrDefault();
            if (robot == null)
                throw new ArgumentException("Unknown robot.");
            return robot;
        }
    }
}

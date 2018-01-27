using System;
using System.Collections.Generic;

namespace Robots
{
    public class FakeSDK
    {
        #region Internal Classes

        public class Robot : IRobot
        {
            private readonly Guid _id = Guid.NewGuid();

            public Guid Id => _id;

            public virtual void Beep()
            {
                throw new NotImplementedException();
            }

            public virtual void Move(double distance)
            {
                throw new NotImplementedException();
            }

            public virtual void Turn(double angle)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        public virtual List<Robot> GetRobots() => new List<Robot>() { new Robot(), new Robot(), new Robot() };
    }
}

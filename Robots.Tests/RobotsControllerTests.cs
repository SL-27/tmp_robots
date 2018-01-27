using System;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace Robots.Tests
{
    [TestFixture]
    public class RobotsControllerTests
    {
        private Mock<FakeSDK> _sdk;
        private Mock<RobotsController> _controller;

        private Mock<FakeSDK.Robot> _robotA;
        private Mock<FakeSDK.Robot> _robotB;
        private Mock<FakeSDK.Robot> _robotC;

        [SetUp]
        public void SetUp()
        {
            _robotA = new Mock<FakeSDK.Robot>() { CallBase = true };
            _robotB = new Mock<FakeSDK.Robot>() { CallBase = true };
            _robotC = new Mock<FakeSDK.Robot>() { CallBase = true };

            _sdk = new Mock<FakeSDK>() { CallBase = true };
            _sdk.Setup(p => p.GetRobots()).Returns(new List<FakeSDK.Robot>() { _robotA.Object, _robotB.Object, _robotC.Object });

            _controller = new Mock<RobotsController>() { CallBase = true };
            _controller.SetupGet(p => p.SDK).Returns(_sdk.Object);

        }

        [Test]
        public void GetRobotByGuid_WhenSdkDoesNotHaveRobot_ShouldRaiseNotFoundException()
        {
            try
            {
                _controller.Object.GetRobotByGuid(Guid.NewGuid());
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Unknown robot.", ex.Message);
            }
        }

        [Test]
        public void GetRobotByGuid_WhenSdkHaveRobot_ShouldReturnIt()
        {
            var robot = _controller.Object.GetRobotByGuid(_robotA.Object.Id);

            Assert.IsNotNull(robot);
            Assert.AreEqual(robot.Id, _robotA.Object.Id);
        }

        [Test]
        public void SendCommands_Always_ShouldRunActions()
        {
            _robotA.Setup(p => p.Move(It.IsAny<double>())).Verifiable();
            _robotA.Setup(p => p.Turn(It.IsAny<double>())).Verifiable();
            _robotA.Setup(p => p.Beep()).Verifiable();

            _controller.Object.SendCommands(_robotA.Object.Id, new List<Action<IRobot>>() {
                r => r.Move(10),
                r => r.Turn(1.5),
                r => r.Beep()
            });

            _robotA.Verify(p => p.Move(It.Is<double>(x => x == 10)), Times.Once);
            _robotA.Verify(p => p.Turn(It.Is<double>(x => x == 1.5)), Times.Once);
            _robotA.Verify(p => p.Beep(), Times.Once);
        }

        [Test]
        public void RepeatCommands_WhenLastCommandsExist_ShouldRunActions()
        {
            _robotA.Setup(p => p.Move(It.IsAny<double>())).Callback(() => { });
            _robotA.Setup(p => p.Turn(It.IsAny<double>())).Callback(() => { });
            _robotA.Setup(p => p.Beep()).Callback(() => { });

            _robotB.Setup(p => p.Move(It.IsAny<double>())).Verifiable();
            _robotB.Setup(p => p.Turn(It.IsAny<double>())).Verifiable();
            _robotB.Setup(p => p.Beep()).Verifiable();

            _controller.Object.SendCommands(_robotA.Object.Id, new List<Action<IRobot>>() {
                r => r.Move(10),
                r => r.Turn(1.5),
                r => r.Beep()
            });

            _controller.Object.RepeatCommands(_robotB.Object.Id);

            _robotB.Verify(p => p.Move(It.Is<double>(x => x == 10)), Times.Once);
            _robotB.Verify(p => p.Turn(It.Is<double>(x => x == 1.5)), Times.Once);
            _robotB.Verify(p => p.Beep(), Times.Once);
        }

        [Test]
        public void RepeatCommands_WhenLastCommandsDoNotExist_ShouldNotRunAnyActions()
        {
            _robotB.Setup(p => p.Move(It.IsAny<double>())).Verifiable();
            _robotB.Setup(p => p.Turn(It.IsAny<double>())).Verifiable();
            _robotB.Setup(p => p.Beep()).Verifiable();

            _controller.Object.RepeatCommands(_robotB.Object.Id);

            _robotB.Verify(p => p.Move(It.IsAny<double>()), Times.Never);
            _robotB.Verify(p => p.Turn(It.IsAny<double>()), Times.Never);
            _robotB.Verify(p => p.Beep(), Times.Never);
        }
    }
}

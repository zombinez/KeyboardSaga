using KeyboardSagaGame.Classes;
using NUnit.Framework;
using System;

namespace KeyboardSagaGame.Tests
{
    [TestFixture]
    class MonsterMethodsTests
    {
        [Test]
        public void GenerateRequiredDistanceTest()
        {
            for(var i = 0; i < 100; i++)
            {
                var distance = MonsterMethods.GenerateRequiredDistance(new Random());
                Assert.IsTrue(distance >= 70 && distance < 131);
            }
        }

        [Test]
        public void GetCurrentDistanceTest()
        {
            var coordinates = new Vector[] { new Vector(0, 10), new Vector(0, -10), new Vector(10, 0), new Vector(-10, 0) };
            var zero = new Vector(0, 0);
            foreach(var coordinate in coordinates)
            {
                Assert.AreEqual(MonsterMethods.GetCurrentDistance(coordinate, zero), 10);
            }
            var complicatedCoordinate = new Vector(5, 5);
            Assert.AreEqual(MonsterMethods.GetCurrentDistance(zero, complicatedCoordinate),
                Math.Sqrt(50));
            complicatedCoordinate = new Vector(-3, 3);
            Assert.AreEqual(MonsterMethods.GetCurrentDistance(zero, complicatedCoordinate),
                Math.Sqrt(18));
            complicatedCoordinate = new Vector(25.5, 18.4);
            Assert.AreEqual(MonsterMethods.GetCurrentDistance(zero, complicatedCoordinate),
                Math.Sqrt(complicatedCoordinate.X * complicatedCoordinate.X + complicatedCoordinate.Y * complicatedCoordinate.Y));
        }

        [Test]
        public void MoveTest()
        {
            var coordinate1 = new Vector(200, 200);
            var target = new Vector(0, 0);
            var coordinate2 = MonsterMethods.Move(target, coordinate1, 10);
            Assert.AreNotEqual(coordinate1, coordinate2);
            Assert.IsTrue(
                MonsterMethods.GetCurrentDistance(target, coordinate2) < MonsterMethods.GetCurrentDistance(target, coordinate1));
        }

        [Test]
        public void ChangeFrameTest()
        {
            var c = 1;
            var b = 2;
            Assert.AreEqual(MonsterMethods.ChangeFrame(c, b), 2);
            b = 3;
            c = 3;
            Assert.AreEqual(MonsterMethods.ChangeFrame(c, b), 0);
        }
    }
}

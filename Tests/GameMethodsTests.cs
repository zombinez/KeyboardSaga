using KeyboardSagaGame.Classes;
using NUnit.Framework;
using System;

namespace KeyboardSagaGame.Tests
{
    [TestFixture]
    class GameMethodsTests
    {
        [Test]
        public void GetImagesTest()
        {
            Assert.DoesNotThrow(() => GameMethods.GetKeys());
            Assert.DoesNotThrow(() => GameMethods.GetSpriteSheets());
        }

        [Test]
        public void CreateRandomMonstersTest()
        {
            var game = new Game();
            var monstersAmount = new Random().Next(1, 8);
            var monsters = game.CreateRandomMonstersAsync(monstersAmount);
            Assert.AreEqual(monsters.Count, monstersAmount * 3);
            foreach(var monster in monsters)
            {
                Assert.IsTrue(monster.Coordinates == game.GateCoordinates[0] 
                    || monster.Coordinates == game.GateCoordinates[1] || monster.Coordinates == game.GateCoordinates[2]);
            }
        }
    }
}

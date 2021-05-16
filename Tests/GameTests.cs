using KeyboardSagaGame.Classes;
using NUnit.Framework;

namespace KeyboardSagaGame.Tests
{
    [TestFixture]
    class GameTests
    {
        [Test]
        public void GameFinishesTest()
        {
            var game = new Game();
            while(!game.IsGameFinished)
            {
                game.DoGameCycle();
            }
        }

        [Test]
        public void WavesChangeTest()
        {
            var game = new Game();
            var wave = game.CurrentWave;
            while(!game.Monsters.TrueForAll(monster => monster.Cycles.DeathCyclesDid >= 60))
            {
                game.DoGameCycle();
                foreach (var monster in game.Monsters)
                {
                    if (!monster.IsDead)
                        monster.BeAttacked(monster.CurrentKeyToPress, new System.Random());
                }
            }
            game.DoGameCycle();
            Assert.AreNotEqual(wave, game.CurrentWave);
        }

        [Test]
        public void GameResetTest()
        {
            var game = new Game();
            for(var i =0; i < 20; i++)
            {
                game.DoGameCycle();
            }
            game.PlayerTower.BeAttacked(5);
            game.Reset();
            game.DoGameCycle();
            Assert.AreEqual(game.Monsters.Count, 3);
            foreach (var monster in game.Monsters)
            {
                Assert.IsTrue(!monster.IsDead);
                Assert.IsTrue(monster.Coordinates == game.GateCoordinates[0]
                    || monster.Coordinates == game.GateCoordinates[1] || monster.Coordinates == game.GateCoordinates[2]);
            }
            Assert.AreEqual(game.PlayerTower.HealthAmount, 50);
        }
    }
}

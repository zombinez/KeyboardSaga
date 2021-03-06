using KeyboardSagaGame.Classes;
using NUnit.Framework;

namespace KeyboardSagaGame.Tests
{
    [TestFixture]
    class MonsterTests
    {
        [Test]
        public void MonsterMovesToTowerTest()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, new Vector(0, 0), game);
            while(MonsterMethods.GetCurrentDistance(monster.Coordinates, game.PlayerTower.Coordinates) > 130)
            {
                monster.Act(game);
            }
        }

        [Test]
        public void MonsterAttacksTowerTest()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, game.PlayerTower.Coordinates, game);
            while(game.PlayerTower.HealthAmount == 50)
            {
                monster.Act(game);
            }
        }

        [Test]
        public void MonsterChangesKeyAfterSuccesfulAttackTest()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, new Vector(0, 0), game);
            var previousKey = monster.CurrentKeyToPress;
            monster.BeAttacked(previousKey, new System.Random());
            Assert.AreNotEqual(monster.CurrentKeyToPress, previousKey);
        }

        [Test]
        public void MonsterDoesNotChangeKeyAfterFoolishAttackTest()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, new Vector(0, 0), game);
            var previousKey = monster.CurrentKeyToPress;
            monster.BeAttacked(previousKey + 1, new System.Random());
            Assert.AreEqual(previousKey, monster.CurrentKeyToPress);
        }

        [Test]
        public void MonsterDiesTest()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, new Vector(0, 0), game);
            while(!monster.IsDead)
            {
                monster.BeAttacked(monster.CurrentKeyToPress, new System.Random());
            }
        }

        [Test]
        public void MonsterGetKilledTest()
        {
            var game = new Game();
            Assert.IsTrue(!game.Monsters[0].IsDead);
            game.Monsters[0].GetKilled();
            Assert.IsTrue(game.Monsters[0].IsDead);
        }

        [Test]
        public void MonsterChangesFramesWhileWalking()
        {
            var game = new Game();
            var monsterFrame = game.Monsters[0].Frame;
            for (var i = 0; i < game.Monsters[0].Cycles.WalkInterval + 1; i++)
                game.Monsters[0].Act(game);
            Assert.AreNotEqual(monsterFrame, game.Monsters[0].Frame);
        }

        [Test]
        public void MonsterChangesFrameWhileAttacking()
        {
            var game = new Game();
            var monster = new Monster(EntityType.SmallKnight, game.PlayerTower.Coordinates, game);
            var monsterFrame = monster.Frame;
            for (var i = 0; i < monster.Cycles.AttackInterval + 1; i++)
                monster.Act(game);
            Assert.AreNotEqual(monsterFrame, monster.Frame);
        }

        [Test]
        public void MonsterChangesFramesWhenDead()
        {
            var game = new Game();
            var monsterFrame = game.Monsters[0].Frame;
            game.Monsters[0].GetKilled();
            for (var i = 0; i < game.Monsters[0].Cycles.DeathInterval + 1; i++)
                game.Monsters[0].Act(game);
            Assert.AreNotEqual(monsterFrame, game.Monsters[0].Frame);
        }
    }
}

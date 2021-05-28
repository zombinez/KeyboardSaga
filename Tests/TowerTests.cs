using KeyboardSagaGame.Classes;
using NUnit.Framework;

namespace KeyboardSagaGame.Tests
{
    [TestFixture]
    class TowerTests
    {
        [Test]
        public void TowerBeingAttackedTest()
        {
            for(var i = 1; i < 51; i++)
            {
                var t = new Tower(0, 0);
                t.BeAttacked(i);
                Assert.AreEqual(t.HealthAmount, 50 - i);
            }
        }

        [Test]
        public void TowerHealTest()
        {
            for(var i = 1; i < 51; i++)
            {
                var t = new Tower(0, 0);
                while(t.HealFrame != 7)
                    t.ChangeHealState();
                t.BeAttacked(i);
                t.Heal();
                if (i <= 15)
                    Assert.AreEqual(t.HealthAmount, 50);
                else
                    Assert.AreEqual(t.HealthAmount, 50 - i + 15);
            }
        }

        [Test]
        public void TowerChangesFrame()
        {
            var tower = new Tower(0, 0);
            var towerFrame = tower.Frame;
            for (var i = 0; i < 10; i++)
                tower.ChangeState();
            Assert.AreNotEqual(towerFrame, tower.Frame);
        }

        [Test]
        public void TowerChangesHealFrame()
        {
            var tower = new Tower(0, 0);
            var towerHealFrame = tower.HealFrame;
            tower.ChangeHealState();
            Assert.AreNotEqual(towerHealFrame, tower.HealFrame);
        }

        [Test]
        public void HealAbilityDoesNotChangeWhenCharged()
        {
            var tower = new Tower(0, 0);
            while (tower.HealFrame != 7)
                tower.ChangeHealState();
            for(var i = 0; i < 10; i++)
            {
                tower.ChangeHealState();
                Assert.AreEqual(tower.HealFrame, 7);
            }
        }
    }
}

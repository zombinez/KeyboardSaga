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
    }
}

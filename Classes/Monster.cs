using System;
using System.Windows.Forms;

namespace KeyboardSagaGame.Classes
{
    public class Monster
    {
        public readonly ImageInfo ImgInfo;
        public readonly EntityType Type;
        public readonly double RequiredDistance;
        public Vector Coordinates { get; private set; }
        public Keys CurrentKeyToPress;
        public MonsterCycles Cycles { get; private set; }
        public MonsterAction CurrentAction { get; private set; }
        public int Frame { get; private set; }
        public bool IsDead => keysCount == 0;
        private readonly double speed;
        private readonly int strength;
        private int keysCount;
        private readonly Action<Game> attackAction;

        public Monster(EntityType monsterType, Vector coordinates, Game game)
        {
            Type = monsterType;
            Coordinates = coordinates;
            CurrentKeyToPress = (Keys)game.Randomizer.Next(65, 91);
            CurrentAction = MonsterAction.Walk;
            Frame = 0;
            RequiredDistance = MonsterMethods.GenerateRequiredDistance(game.Randomizer);
            switch (monsterType)
            {
                case EntityType.SmallKnight:
                    ImgInfo = new ImageInfo(88, 76, 3);
                    keysCount = 3 + game.Randomizer.Next(0, (game.CurrentWave - 3) / 6);
                    Cycles = new MonsterCycles(16, 7, 10);
                    speed = game.Randomizer.Next(4, 6);
                    strength = 2;
                    attackAction = new Action<Game>((g) => g.PlayerTower.BeAttacked(strength));
                    break;
                case EntityType.Slime:
                    ImgInfo = new ImageInfo(60, 64, 9);
                    keysCount = 2 + game.Randomizer.Next(0, (game.CurrentWave - 3) / 4);
                    Cycles = new MonsterCycles(11, 2, 6);
                    speed = game.Randomizer.Next(4, 8);
                    strength = 1;
                    attackAction = new Action<Game>((g) => g.PlayerTower.BeAttacked(strength));
                    break;
                case EntityType.King:
                    RequiredDistance += 100;
                    ImgInfo = new ImageInfo(88, 111, 3);
                    keysCount = 15 + game.Randomizer.Next(0, (game.CurrentWave - 3) / 5);
                    Cycles = new MonsterCycles(43, 6, 8);
                    speed = 2;
                    strength = 0;
                    attackAction = new Action<Game>((g) =>
                    {
                        g.MonsterToAdd.Enqueue(new Monster(EntityType.SmallKnight, 
                            new Vector(Coordinates.X - 130, Coordinates.Y + 60), g));
                        g.MonsterToAdd.Enqueue(new Monster(EntityType.SmallKnight, 
                            new Vector(Coordinates.X + 130, Coordinates.Y + 60), g));
                    });
                    break;
                case EntityType.WitchDoctor:
                    ImgInfo = new ImageInfo(92, 108, 7);
                    keysCount = 1;
                    Cycles = new MonsterCycles(5, 4, 5);
                    speed = 10;
                    strength = 10;
                    attackAction = new Action<Game>((g) =>
                    {
                        g.PlayerTower.BeAttacked(10);
                        CurrentAction = MonsterAction.Death;
                        keysCount = 0;
                        Frame = ImgInfo.FramesAmount;
                    });
                    break;
            }
        }

        public void Act(Game game)
        {
            if (!IsDead)
            {
                if (MonsterMethods.GetCurrentDistance(Coordinates, game.PlayerTower.Coordinates) > RequiredDistance)
                {
                    CurrentAction = MonsterAction.Walk;
                    if (Cycles.WalkCyclesDid == Cycles.WalkInterval)
                    {
                        Cycles.WalkCyclesDid = 0;
                        ChangeState();
                    }
                    else Cycles.WalkCyclesDid++;
                    MoveTowardsTower(game.PlayerTower.Coordinates);
                }
                else
                {
                    CurrentAction = MonsterAction.Attack;
                    if (Cycles.AttackCyclesDid == 0)
                        Frame = 0;
                    else if (Cycles.AttackCyclesDid % Cycles.AttackInterval == 0)
                    {
                        ChangeState();
                        if (Cycles.AttackCyclesDid == Cycles.AttackInterval * (ImgInfo.FramesAmount + 1))
                            Attack(game);
                    }
                    Cycles.AttackCyclesDid++;
                }
            }
            else
            {
                if (Cycles.DeathCyclesDid % Cycles.DeathInterval == 0 && Frame != ImgInfo.FramesAmount)
                    ChangeState();
                Cycles.DeathCyclesDid++;
            }
        }

        private void Attack(Game game)
        {
            attackAction(game);
            Cycles.AttackCyclesDid = 0;
        }

        public void BeAttacked(Keys key, Random gameRandomizer)
        {
            if (!IsDead && key == CurrentKeyToPress)
            {
                keysCount--;
                CurrentKeyToPress = (Keys)gameRandomizer.Next(65, 91);
                if (IsDead)
                {
                    CurrentAction = MonsterAction.Death;
                    Frame = 0;
                }
            }
        }

        private void MoveTowardsTower(Vector towerCoordinates) =>
            Coordinates = MonsterMethods.Move(towerCoordinates, Coordinates, speed);

        private void ChangeState() =>
            Frame = MonsterMethods.ChangeFrame(Frame, ImgInfo.FramesAmount);

        public void GetKilled()
        {
            keysCount = 0;
            CurrentAction = MonsterAction.Death;
            Frame = 0;
        }
    }

    public class ImageInfo
    {
        public readonly int ImgWidth;
        public readonly int ImgHeight;
        public readonly int FramesAmount;

        public ImageInfo(int width, int height, int hCount)
        {
            ImgWidth = width;
            ImgHeight = height;
            FramesAmount = hCount;
        }
    }

    public class MonsterCycles
    {
        public int WalkCyclesDid;
        public int AttackCyclesDid;
        public int DeathCyclesDid;
        public int DefenceCyclesDid;
        public readonly int AttackInterval;
        public readonly int DeathInterval;
        public readonly int WalkInterval;

        public MonsterCycles(int attackInterval, int walkInterval, int deathInterval)
        {
            WalkCyclesDid = 0;
            AttackCyclesDid = 0;
            DeathCyclesDid = 0;
            DefenceCyclesDid = 0;
            AttackInterval = attackInterval;
            DeathInterval = deathInterval;
            WalkInterval = walkInterval; 
        }
    }

    public static class MonsterMethods
    {
        public static double GenerateRequiredDistance(Random randomizer) => randomizer.Next(7, 13) * 10;

        public static double GetCurrentDistance(Vector monsterCoordinates, Vector towerCoordinates) =>
            (monsterCoordinates - towerCoordinates).Length;

        public static Vector Move(Vector tower, Vector monster, double speed) => 
            monster + (tower - monster).Normalize() * speed * 0.3;

        public static int ChangeFrame(int counter, int bound)
        {
            if (counter == bound)
                return 0;
            return counter + 1;
        }
    }
}

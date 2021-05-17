using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Linq;
using System.Threading.Tasks;

namespace KeyboardSagaGame.Classes
{
    public class Game
    {
        public readonly Dictionary<EntityType, Image> SpriteSheets;
        public readonly Dictionary<Keys, Image> KeysImages;
        public readonly Image MapImage;
        public readonly Vector[] GateCoordinates;
        public Tower PlayerTower { get; private set; }
        public List<Monster> Monsters { get; private set; }
        public int CurrentWave { get; private set; }
        public bool IsGameFinished { get; private set; }
        public Random Randomizer { get; private set; }
        public Queue<Monster> MonsterToAdd { get; private set; }

        public Game()
        {
            SpriteSheets = GameMethods.GetSpriteSheets();
            KeysImages = GameMethods.GetKeys();
            MapImage = GameMethods.GetImageByName("map");
            GateCoordinates = new Vector[] 
            { 
                new Vector(275, 85), 
                new Vector(640, 85), 
                new Vector(1005, 85) 
            };
            PlayerTower = new Tower(631.5, 400);
            CurrentWave = 4;
            Randomizer = new Random();
            Monsters = this.CreateRandomMonstersAsync(1);
            IsGameFinished = false;
            MonsterToAdd = new Queue<Monster>();
        }

        public void DoGameCycle()
        {
            if (Monsters.TrueForAll(monster => monster.Cycles.DeathCyclesDid >= 60))
            {
                PlayerTower.ChangeHealState();
                CurrentWave++;
                if ((CurrentWave - 3) % 5 == 0 && CurrentWave > 0)
                    Monsters.Add(new Monster(EntityType.King, GateCoordinates[1], this));
                else
                    Monsters = this.CreateRandomMonstersAsync((CurrentWave - 3) / 5 + 1);
                MonsterToAdd.Clear();
            }
            else if (PlayerTower.HealthAmount > 0)
            {
                PlayerTower.ChangeState();
                while (MonsterToAdd.Count != 0)
                    Monsters.Add(MonsterToAdd.Dequeue());
                foreach (var monster in Monsters)
                    monster.Act(this);
            }
            else IsGameFinished = true;
        }

        public void Reset()
        {
            PlayerTower = new Tower(631.5, 400);
            Monsters = new List<Monster>();
            CurrentWave = 3;
            IsGameFinished = false;
            MonsterToAdd = new Queue<Monster>();
        }

    }

    public static class GameMethods
    {

        public static List<Monster> CreateRandomMonstersAsync(this Game game, int monstersAmount)
        {
            var monstersList = new List<Monster>();
            for(var i = 0; i < 3; i++)
            {
                var tasks = Enumerable.Range(1, monstersAmount)
                    .Select(e => Task.Run(() =>
                    {
                        var randomMonsterEnum = 
                            (EntityType)game.Randomizer.Next(1, Enum.GetNames(typeof(EntityType)).Length - 2);
                        return new Monster(randomMonsterEnum, game.GateCoordinates[i], game);
                    })).ToArray();
                Task.WaitAll(tasks);
                monstersList.AddRange(tasks.Select(task => task.Result));
            }
            if (game.CurrentWave - 3 > 7)
                monstersList.Add(
                    new Monster(EntityType.WitchDoctor, game.GateCoordinates[game.Randomizer.Next(0, 3)], game));
            return monstersList;
        }

        public static Image GetImageByName(string name) => (Image)Properties.Resources.ResourceManager.GetObject(name);

        public static Dictionary<EntityType, Image> GetSpriteSheets() =>
            new Dictionary<EntityType, Image>
            {
                [EntityType.SmallKnight] = GetImageByName("smallknight"),
                [EntityType.Slime] = GetImageByName("slime"),
                [EntityType.Tower] = GetImageByName("tower"),
                [EntityType.WitchDoctor] = GetImageByName("witch_doctor"),
                [EntityType.King] = GetImageByName("king")
            };

        public static Dictionary<Keys, Image> GetKeys()
        {
            var keys = new Dictionary<Keys, Image>();
            for(var i = 65; i < 91; i++)
            {
                keys[(Keys)i] = GetImageByName(((Keys)i).ToString());
            }
            keys[Keys.Space] = Properties.Resources.SPACE;
            return keys;
        }
    }
}

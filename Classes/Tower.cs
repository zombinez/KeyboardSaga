using System.Drawing;
using System.IO;
using System.Reflection;
using WMPLib;

namespace KeyboardSagaGame.Classes
{
    public class Tower
    {
        public int Frame { get; private set; }
        public int HealthAmount { get; private set; }
        public Image HealthImage { get; private set;}
        public Image HealImage { get; private set; }
        public int HealFrame { get; private set; }
        public int Cycles { get; private set; }
        public bool HealRecharged { get; private set; }
        public bool LostHealth { get; private set; }
        public readonly ImageInfo ImgInfo;
        public readonly ImageInfo HealImgInfo;
        public readonly Vector Coordinates;

        public Tower(double x, double y)
        {
            HealRecharged = LostHealth = false;
            Cycles = Frame = 0;
            HealFrame = 0;
            Coordinates = new Vector(x, y);
            HealthAmount = 50;
            HealthImage = GetHealthImage();
            ImgInfo = new ImageInfo(103, 198, 5);
            HealImage = GameMethods.GetImageByName("heal_spriteslist");
            HealImgInfo = new ImageInfo(52, 42, 7);
        }

        public void Heal()
        {
                HealFrame = 0;
                if (HealthAmount < 35)
                    HealthAmount += 15;
                else HealthAmount = 50;
                HealthImage = GetHealthImage();
            if (HealRecharged)
                HealRecharged = false;
        }

        public void ChangeState()
        {
            if (Frame == ImgInfo.FramesAmount)
            {
                Frame = 0;
                Cycles++;
            }
            else
            {
                if (Cycles % 9 == 0)
                Frame++;
                Cycles++;
            }
            HealRecharged = false;
            LostHealth = false;
        }

        public void ChangeHealState()
        {
            if (!HealRecharged && HealFrame == 6)
                HealRecharged = true;
            if (HealFrame < 7)
                HealFrame++;
        }

        public void BeAttacked(int attackStrength)
        {
            var previousHealth = HealthAmount;
            HealthAmount -= attackStrength;
            if (previousHealth / 5 > HealthAmount / 5 && previousHealth != 50 || HealthAmount <= 0)
            {
                LostHealth = true;
                HealthImage = GetHealthImage();
            }
        }

        private Image GetHealthImage()
        {
            int firstDigit = HealthAmount / 10;
            var lastDigit = HealthAmount % 10;
            if (HealthAmount > 0)
                if (lastDigit == 0 || lastDigit > 5)
                    if (firstDigit > 0 && lastDigit == 0)
                        return GameMethods.GetImageByName($"_{(firstDigit - 1) * 10 + 6}___{firstDigit * 10}");
                    else if (firstDigit > 0 && lastDigit > 0)
                        return GameMethods.GetImageByName($"_{firstDigit * 10 + 6}___{(firstDigit + 1) * 10}");
                    else return GameMethods.GetImageByName($"_6___10");
                else if (firstDigit > 0)
                    return GameMethods.GetImageByName($"_{firstDigit * 10 + 1}___{firstDigit * 10 + 5}");
                else return GameMethods.GetImageByName($"_1___5");
            else return GameMethods.GetImageByName("_0");
        }
    }
}

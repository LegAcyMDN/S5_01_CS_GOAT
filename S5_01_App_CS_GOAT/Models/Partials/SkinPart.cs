namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Skin
    {
        public Wear GetClosestWear(float floatValue)
        {
            Wear? closestWear = null;
            float closestDifference = float.MaxValue;

            foreach (Wear wear in Wears)
            {
                float difference = Math.Abs(wear.WearFloat - floatValue);
                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestWear = wear;
                }
            }
            if (closestWear == null)
                throw new Exception("No wears available for this skin.");
            return closestWear;
        }
    }
}
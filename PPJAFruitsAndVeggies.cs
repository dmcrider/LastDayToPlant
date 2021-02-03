using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public class PPJAFruitsAndVeggies : Mod
    {
        // https://github.com/paradigmnomad/PPJA/tree/master/%5BPPJA%5D%20Fruits%20and%20Veggies/%5BJA%5D%20Fruits%20and%20Veggies/Crops
        // TODO: Start with Bell Pepper

        public PPJAFruitsAndVeggies(bool enabled) : base("PPJAFruitsAndVeggies",enabled)
        {
            
        }

        public override void LoadSpringCrops(List<Crop> list, IModHelper helper)
        {
            list.Add(Crop.GetLocalizedCrop("spring", "Basil", 5, helper, Name));
        }

        public override void LoadSummerCrops(List<Crop> list, IModHelper helper)
        {
            list.Add(Crop.GetLocalizedCrop("summer", "Adzuki Bean", 9, helper, Name));
            list.Add(Crop.GetLocalizedCrop("summer", "Aloe", 4, helper, Name));
            list.Add(Crop.GetLocalizedCrop("summer", "Asparagus", 5, helper, Name));
            list.Add(Crop.GetLocalizedCrop("summer", "Bamboo", 7, helper, Name));
        }

        public override void LoadFallCrops(List<Crop> list, IModHelper helper)
        {
            list.Add(Crop.GetLocalizedCrop("fall", "Barley", 4, helper, Name));
        }

        public override void LoadWinterCrops(List<Crop> list, IModHelper helper)
        {

        }
    }
}

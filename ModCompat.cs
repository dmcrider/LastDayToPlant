using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace LastDayToPlant
{
    public class ModCompat
    {
        public string Name { get; set; }
        public string BaseFolderName { get; set; }
        public bool IsEnabled { get; set; }

        public ModCompat(string name, string baseFolderName, bool isEnabled)
        {
            Name = name;
            BaseFolderName = baseFolderName;
            IsEnabled = isEnabled;
        }

        public ModCompat(string name, string baseFodlerName)
        {
            Name = name;
            BaseFolderName = baseFodlerName;
            IsEnabled = false;
        }

        public void LoadCrops(List<Crop> spring, List<Crop> summer, List<Crop> fall, List<Crop> winter, IModHelper helper)
        {
            DirectoryInfo baseCropFolder = new DirectoryInfo(BaseFolderName);

            List<string> addedCrops = new List<string>();

            foreach(var folder in baseCropFolder.GetDirectories())
            {
                foreach(var cropFile in folder.GetFiles())
                {
                    if (addedCrops.Contains(folder.Name)) { continue; }

                    if (cropFile.Name == "crop.json")
                    {
                        var json = JObject.Parse(File.ReadAllText(cropFile.FullName));
                        string name = (string)json["Name"];
                        int daysToMature = GetDaysToMature((string)json["SeedDescription"]);
                        JArray seasons = (JArray)json["Seasons"];

                        Crop currentCrop = new Crop(name,daysToMature)
                        {
                            Message = helper.Translation.Get("notification.crop.no-fertilizer", new { cropName = name }),
                            MessageSpeedGro = helper.Translation.Get("notification.crop.speed-gro", new { cropName = name }),
                            MessageDelxueSpeedGro = helper.Translation.Get("notification.crop.deluxe-speed-gro", new { cropName = name }),
                            MessageHyperSpeedGro = helper.Translation.Get("notification.crop.hyper-speed-gro", new { cropName = name })
                        };

                        foreach(JValue seasonObj in seasons)
                        {
                            string season = seasonObj.Value.ToString();

                            switch (season)
                            {
                                case "spring":
                                    spring.Add(currentCrop);
                                    break;
                                case "summer":
                                    summer.Add(currentCrop);
                                    break;
                                case "fall":
                                    fall.Add(currentCrop);
                                    break;
                                case "winter":
                                    winter.Add(currentCrop);
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }

                    addedCrops.Add(folder.Name); // Keep track of the ones we've added
                    // for some reason it was adding them 3 times...
                }
            }
        }

        private int GetDaysToMature(string input)
        {
            string output = string.Empty;

            for(int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    output += input[i];
                }
            }

            if(output != string.Empty)
            {
                return int.Parse(output);
            }
            else
            {
                return -1;
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderFood.DAL.Data.DataSeed.Entities
{
    public static class EntityStoreSeed
    {
        //public static async Task SeedAsync(HealthCareContext dbContext, UserManager<AppUser> userManager)
        //{
        //    var ServicePath = "../HealthCare.Repository/Data/DataSeed/Locations.json";

        //    await TransferData<Services>(ServicePath, dbContext);

        //    var HWIdPath = "../HealthCare.Repository/Data/DataSeed/HardwareId.json";

        //    await TransferData<Hardware>(HWIdPath, dbContext);

        //    if (userManager.Users.Any())
        //    {
        //        var HistoryPath = "../HealthCare.Repository/Data/DataSeed/History.json";

        //        await TransferData<History>(HistoryPath, dbContext);
        //    }

        //}

        //private static async Task TransferData<T>(string DataPath, HealthCareContext dbContext) where T : AppEntity
        //{
        //    if (!dbContext.Set<T>().Any())
        //    {
        //        var ItemsData = File.ReadAllText(DataPath);
        //        var Items = JsonSerializer.Deserialize<List<T>>(ItemsData);
        //        if (Items?.Count > 0)
        //        {
        //            if (typeof(T) == typeof(History))
        //            {
        //                var history = Items as List<History>;
        //                foreach (var Item in history!)
        //                {
        //                    string DoctorId = dbContext.Set<Patient>().Where(p => p.Id == Item.HistoryPatientId).Select(p => p.PatientDoctorId).FirstOrDefault()!;
        //                    Item.HistoryDoctorId = DoctorId;
        //                    await dbContext.Set<History>().AddAsync(Item);
        //                }
        //            }
        //            else
        //            {
        //                foreach (var Item in Items)
        //                {
        //                    await dbContext.Set<T>().AddAsync(Item);
        //                }
        //            }

        //            await dbContext.SaveChangesAsync();
        //        }
        //    }
        //}

    }
}

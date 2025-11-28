using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class FairRandomManager(CSGOATDbContext context) : CrudRepository<FairRandom>(context)
    {
      
    }
}

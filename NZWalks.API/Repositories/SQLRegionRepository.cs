using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext dbConText;
        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            this.dbConText = dbContext;
        }
        public async Task<List<Region>> GetAllAsync()
        {
            return await dbConText.Regions.ToListAsync();
        }
    }
}

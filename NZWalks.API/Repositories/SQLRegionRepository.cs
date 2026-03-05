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

        public async Task<Region> CreateAsync(Region region)
        {
            await dbConText.Regions.AddAsync(region);
            await dbConText.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existingRegion = await dbConText.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRegion == null)
            {
                return null;
            }
            dbConText.Regions.Remove(existingRegion);
            await dbConText.SaveChangesAsync();
            return existingRegion;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await dbConText.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
           return await dbConText.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existingRegion = await dbConText.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageUrl = region.RegionImageUrl;
            await dbConText.SaveChangesAsync();
            return existingRegion;
        }
    }
}

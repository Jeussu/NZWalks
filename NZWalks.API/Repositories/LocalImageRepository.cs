using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NZWalksDbContext dbContext;
        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            NZWalksDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }
        public async Task<Image> Upload(Image image)
        {
            var imagesPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images");
            Directory.CreateDirectory(imagesPath);

            var localFilePath = Path.Combine(imagesPath,
                $"{image.FileName}{image.FileExtension}");

            // Upload Image to local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // https://localhost:5001/Images/filename.jpg
            var httpContext = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HTTP context is unavailable while generating the image URL.");

            var urlFilePath = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Images/{image.FileName}{image.FileExtension}";
            
            image.FilePath = urlFilePath;

            // Add Image to the Images table in the database
            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }
    }
}

using Cloudflare.NET.R2;
using Cloudflare.NET.R2.Models;

namespace MediaSite_backend.Services
{
    public class StorageService(IR2Client r2, IConfiguration configuration)
    {
        private readonly string bucket = configuration["R2:BucketName"]!;
        public async Task<R2Result> UploadAsync(string key, Stream fileStream)
        {
            return await r2.UploadAsync(bucket, key, fileStream);
        }
    }
}

using Cloudflare.NET.R2;
using Cloudflare.NET.R2.Models;

namespace MediaSite_backend.Services
{
    public class StorageService(IR2Client r2)
    {
        public async Task<R2Result> UploadAsync(string bucket, string key, string filePath)
        {
            return await r2.UploadAsync(bucket, key, filePath);
        }
    }
}

using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;
using CloudNativeApp.Models;

namespace CloudNativeApp.Services
{
    public class S3StorageService
    {
        private readonly string bucketName = "s3-bucket-oneil";
        private readonly IAmazonS3 s3Client;

        public S3StorageService(IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
        }

        public async Task SaveThingAsync(Thing thing)
        {
            var json = JsonSerializer.Serialize(thing);
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{thing.Id}.json",
                ContentBody = json
            };
            await s3Client.PutObjectAsync(request);
        }

        public async Task<Thing?> GetThingAsync(string id)
        {
            try
            {
                var response = await s3Client.GetObjectAsync(bucketName, $"{id}.json");
                using var reader = new StreamReader(response.ResponseStream);
                var json = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<Thing>(json);
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task DeleteThingAsync(string id)
        {
            await s3Client.DeleteObjectAsync(bucketName, $"{id}.json");
        }
    }
}

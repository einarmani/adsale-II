using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace AdSale.Services
{
    public class S3Helper
    {
        private readonly IAmazonS3 _s3Client;

        public S3Helper(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<bool> SaveFile(string bucket, string key, Stream stream, S3CannedACL acl, string contentType = null)
        {
            PutObjectRequest request = new PutObjectRequest();

            request.BucketName = bucket;
            request.InputStream = stream;
            request.Key = key;
            request.CannedACL = acl;
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                request.ContentType = contentType;
            }

            var task = await _s3Client.PutObjectAsync(request);

            return true;
        }

        public string GetUrl(string bucket, string key, int duration = 10)
        {
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
            request.BucketName = bucket;
            request.Expires = DateTime.Now.AddMinutes(duration);
            request.Key = key;

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
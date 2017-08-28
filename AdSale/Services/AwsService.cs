using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Newtonsoft.Json;

namespace AdSale.Services
{
    public class AwsService<T>
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucket;

        public AwsService(IAmazonS3 s3Client, string configKey)
        {
            _s3Client = s3Client;
            //var config = Base.Config.Load();
            //_bucket = config[configKey];
            _bucket = "visir-adsales";
        }

        public async Task<T> GetObject(string objectKey)
        {
            S3Helper s3 = new S3Helper(_s3Client);
            var url = s3.GetUrl(_bucket, objectKey);

            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = null;
            try
            {
                response = await http.GetResponseAsync();
                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();

                        var items = JsonConvert.DeserializeObject<T>(json);

                        return items;
                    }
                }
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
            finally
            {
                response?.Dispose();
            }

            return default(T);
        }

        public async Task<bool> SaveObject(T entity, string objectKey)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.None);

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                S3Helper s3 = new S3Helper(_s3Client);
                await s3.SaveFile(_bucket, objectKey, ms, S3CannedACL.AuthenticatedRead);
            }
            return true;
        }
    }
}

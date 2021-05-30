using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonWebServicesAPI.Controllers
{
    [ApiController]
    [Route("api/v1/s3")]
    public class S3Controller : ControllerBase
    {
        private readonly AmazonS3Client _client;

        public S3Controller()
        {
            _client = new(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"), 
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
                Amazon.RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION")));
        }

        // GET: api/v1/s3
        [HttpGet]
        public async Task<ActionResult<IEnumerable<S3Bucket>>> ListBuckets()
        {
            if (!Auth.IsAuthorized(Request)) return Unauthorized();

            ListBucketsResponse response = await _client.ListBucketsAsync();

            return response.Buckets;
        }

        // GET: api/v1/s3/{bucket-name}[?key={key}]
        [HttpGet("{bucketName}")]
        public async Task<ActionResult<object>> ListBucketObjectsAsync(string bucketName, string key = "")
        {
            if (!Auth.IsAuthorized(Request)) return Unauthorized();

            if (string.IsNullOrEmpty(key))
            {
                ListObjectsResponse response = await _client.ListObjectsAsync(bucketName);
                return response.S3Objects;
            }
            else
            {
                GetObjectMetadataResponse response = await _client.GetObjectMetadataAsync(bucketName, key.TrimStart('/'));
                return response;
            }
        }

        // POST: api/v1/s3/{bucket-name}[?keyPath={keyPath}]
        [HttpPost("{bucketName}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<GetObjectMetadataResponse>> UploadObjectAsync(IFormFile file, string bucketName, string keyPath = "")
        {
            if (!Auth.IsAuthorized(Request)) return Unauthorized();
            if (file == null || file.Length <= 0) return BadRequest();

            string key = string.IsNullOrEmpty(keyPath) ? $"{file.FileName}" : $"{keyPath.TrimStart('/').TrimEnd('/')}/{file.FileName}";

            TransferUtility transfer = new(_client);
            await transfer.UploadAsync(file.OpenReadStream(), bucketName, key);

            GetObjectMetadataResponse response = await _client.GetObjectMetadataAsync(bucketName, key);

            return response;
        }
    }
}

using Amazon.Glacier;
using Amazon.Glacier.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AmazonWebServicesAPI.Controllers
{
    [ApiController]
    [Route("api/v1/s3-glacier")]
    public class S3GlacierController : ControllerBase
    {
        private readonly AmazonGlacierClient _client;

        public S3GlacierController()
        {
            _client = new AmazonGlacierClient(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"), 
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
                Amazon.RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION")));
        }

        // GET: api/v1/s3-glacier/vault/{vault-name}
        [HttpGet("vault/{name}")]
        public async Task<ActionResult<DescribeVaultResponse>> GetGlacierVaultAsync(string name)
        {
            if (!Auth.IsAuthorized(Request)) return Unauthorized();

            DescribeVaultRequest request = new() { VaultName = name };
            DescribeVaultResponse response = await _client.DescribeVaultAsync(request);

            return response;
        }

        // GET: api/v1/s3-glacier/vaults
        [HttpGet("vaults")]
        public async Task<ActionResult<ListVaultsResponse>> GetGlacierVaultsAsync()
        {
            if (!Auth.IsAuthorized(Request)) return Unauthorized();

            ListVaultsResponse response = await _client.ListVaultsAsync();

            return response;
        }
    }
}

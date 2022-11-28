using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWS3.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SgetController : ControllerBase
    {
        //將所有TXT都上傳
        private const string existingBucketName = "jimmy-wang-keenbest-test";
        private const string directoryPath = @"C:\Users\WangNJ\Desktop\AWS3\AWS3";
        // The example uploads only .txt files.
        private const string wildCard1 = "mp3_shareFile";
        private const string wildCard = "*.txt";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APNortheast1;
        public async Task<IActionResult> Get()
        {
            Dictionary<string, string> lstReutn = new Dictionary<string, string>();
            try
            {
              
                var s3Client = new AmazonS3Client();
                var listResponse = await s3Client.ListObjectsAsync("jimmy-wang-keenbest-test");
                var directoryTransferUtility =
                    new TransferUtility(s3Client);
                
                string res = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest { BucketName = "jimmy-wang-keenbest-test", Key = "AWS_test_demo.mp3", Expires = DateTime.Now.AddMinutes(5) });
                // 1. Upload a directory.
                await directoryTransferUtility.UploadDirectoryAsync(directoryPath,
                    existingBucketName);
                Console.WriteLine("Upload statement 1 completed");
                // 2. Upload only the .txt files from a directory 
                //    and search recursively. 
                await directoryTransferUtility.UploadDirectoryAsync(
                                               directoryPath,
                                               existingBucketName,
                                             //  wildCard,
                                               wildCard1,
                                               SearchOption.AllDirectories);
                Console.WriteLine("Upload statement 2 completed");

                // 3. The same as Step 2 and some optional configuration. 
                //    Search recursively for .txt files to upload.
                var request = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = existingBucketName,
                    Directory = directoryPath,
                    SearchOption = SearchOption.AllDirectories,
                    SearchPattern = wildCard
                };

                await directoryTransferUtility.UploadDirectoryAsync(request);
                Console.WriteLine("Upload statement 3 completed");
                foreach (S3Object b in listResponse.S3Objects)
                {
                    lstReutn[b.Key] = b.Size.ToString();
                }
                
            }
            catch (AmazonS3Exception e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


            return Ok(lstReutn);
            
        }

        private void Log(string v, string urlString)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{*key}")]
        public async  Task<IActionResult> GetKey(string key)
        {
            var bytes = new byte[0];
            var s3Client = new AmazonS3Client();
            string title = "";
            string contentType = "";
            string res = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest { BucketName = "jimmy-wang-keenbest-test", Key = "AWS_test_demo.mp3", Expires = DateTime.Now.AddMinutes(5) });

            try
            {
                //var createResponse = await s3Client.PutBucketAsync("3probe");
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = "jimmy-wang-keenbest-test",
                    Key = key
                };
                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    contentType = response.Headers["Content-Type"];
                    var br = new BinaryReader(responseStream);
                    long numBytes = response.ContentLength;
                    bytes = br.ReadBytes((int)numBytes);
                }
            }
            catch (AmazonS3Exception e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return File(bytes, contentType, title);
        }


    }
    class UploadDirMPUHighLevelAPITest
    {
        private const string existingBucketName = "jimmy-wang-keenbest-test";
        private const string directoryPath = @"C:\Users\WangNJ\Desktop\AWS3\AWS3";
        // The example uploads only .txt files.
        private const string wildCard = "*.txt";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APNortheast1;
        private static IAmazonS3 s3Client;
       // static void Main()
        //{
           
       // }

        private static async Task UploadDirAsync()
        {
            try
            {
                s3Client = new AmazonS3Client(bucketRegion);

                UploadDirAsync().Wait();
                var directoryTransferUtility =
                    new TransferUtility(s3Client);

                // 1. Upload a directory.
                await directoryTransferUtility.UploadDirectoryAsync(directoryPath,
                    existingBucketName);
                Console.WriteLine("Upload statement 1 completed");

                // 2. Upload only the .txt files from a directory 
                //    and search recursively. 
                await directoryTransferUtility.UploadDirectoryAsync(
                                               directoryPath,
                                               existingBucketName,
                                               wildCard,
                                               SearchOption.AllDirectories);
                Console.WriteLine("Upload statement 2 completed");

                // 3. The same as Step 2 and some optional configuration. 
                //    Search recursively for .txt files to upload.
                var request = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = existingBucketName,
                    Directory = directoryPath,
                    SearchOption = SearchOption.AllDirectories,
                    SearchPattern = wildCard
                };

                await directoryTransferUtility.UploadDirectoryAsync(request);
                Console.WriteLine("Upload statement 3 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }


}


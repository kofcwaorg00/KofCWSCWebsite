using Azure.Identity;
using Azure.Storage.Blobs;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web.Helpers;

namespace KofCWSCWebsite.Controllers
{
    

    public class BlobController : Controller
    {
        private readonly string blobUri; // = "https://kofcwscdatastorageblob.blob.core.windows.net/statedirectory/State%20Directory.pdf";
        private readonly string tenantId; // = "cba846cf-1683-4d63-8c9c-93e37f653c83";
        private readonly string clientId; // = "77d240dd-8c90-4f50-ad6b-93e08b39e21a";
        private readonly string clientSecret; // = "FuR8Q~1PLpYOpMW4DJWHTwatH11wDIDUE.KrEcLz";

        private readonly IConfiguration _config;
        public BlobController(IConfiguration configurationManager)
        {
            _config = configurationManager;
            KeyVaultHelper kvh = new KeyVaultHelper(_config);
            tenantId = kvh.GetSecret("KOFCWSCWEBSITETENANTID");
            clientId = kvh.GetSecret("KOFCWSCWEBSITECLIENTID");
            clientSecret = kvh.GetSecret("KOFCWSCWEBSITECLIENTSECRET");
            blobUri = kvh.GetSecret("STATEDIRECTORYBLOBURI");
        }

        public async Task<IActionResult> ViewStateDir()
        {
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var blobClient = new BlobClient(new Uri(blobUri), credential);

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;

            // Option 1: Return file inline
            return File(stream.ToArray(), "application/pdf");
        }



        [HttpGet]
        public async Task<IActionResult> GetStateDirectory()
        {
            var FSO = await GetFileStreamObjectFromBlob();
            //return File(fso.Data, fso.ContentType, fso.FileName);
            //return View(Json(new { FSO.FileName, FSO.ContentType, FSO.Data }), "FileStorage/ShowPDF.cshtml"); // Json(new { FileName = FSO.FileName, ContentType = FSO.ContentType, Data = FSO.Data });
            return View("/Views/FileStorage/ShowPDF.cshtml",FSO); // Json(new { FileName = FSO.FileName, ContentType = FSO.ContentType, Data = FSO.Data });
        }
        private async Task<FileStorage> GetFileStreamObjectFromBlob()
        {
            // You can also use KeyVault-integrated credential setup here
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var blobClient = new BlobClient(new Uri(blobUri), credential);

            var download = await blobClient.DownloadAsync();

            byte[] fileBytes = await ConvertStreamToByteArrayAsync(download.Value.Content);

            return new FileStorage
            {
                FileName = "myfile.pdf",
                ContentType = "application/pdf",
                Data = fileBytes
            };
        }
        public static async Task<byte[]> ConvertStreamToByteArrayAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

    }

}

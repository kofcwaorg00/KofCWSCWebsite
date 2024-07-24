using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;

using Serilog;


namespace KofCWSCWebsite.Pages
{
    public class UploadFileModel : PageModel
    {
        private IWebHostEnvironment _environment;
        
        public UploadFileModel(IWebHostEnvironment environment)
        {
            _environment = environment;

            Log.Information(environment.WebRootPath);

        }
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {
            try
            {
                var file = Path.Join(_environment.WebRootPath, "/images/AOI", Upload.FileName);
                Log.Information(file);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                    ViewData["file"] = "~/files/uploads/AOI/" + Upload.FileName;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
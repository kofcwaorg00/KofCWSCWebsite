using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KofCWSCWebsite.Controllers
{
    public class SPController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SPController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GetSOS
        [Route("GetSOSView")]
        public IActionResult GetSOSView()
        {
            // Run Sproc
            var result = _context.Database
                .SqlQuery<SPGetSOSView>($"EXECUTE uspWEB_GetSOSView")
                .ToList();
            ViewData["MyHeading"] = result[0].Heading;
            ViewData["myHost"] = HttpContext.Request.Host.Value;
            return View("Views/StateFamily/GetSOSView.cshtml", result);
        }

        // GET: Bulletins
        [Route("GetBulletins")]
        public IActionResult GetBulletins()
        {
            // Run Sproc
            var result = _context.Database
                .SqlQuery<SPGetBulletins>($"EXECUTE uspSYS_GetBulletins")
                .ToList();


            return View("Views/Home/GetBulletins.cshtml", result);
        }

        // GET: EmailAlias
        [Route("GetEmailAlias")]
        public IActionResult GetEmailAlias()
        {
            // Run Sproc
            var result = _context.Database
                .SqlQuery<SPGetEmailAlias>($"EXECUTE uspWEB_GetEmailAliasForRepeater")
                .ToList();


            return View("Views/StateFamily/GetEmailAlias.cshtml", result);
        }
        // GET: Chairmen
        [Route("GetChairmen")]
        public IActionResult GetChairmen()
        {
            // Run Sproc
            var result = _context.Database
                .SqlQuery<SPGetChairmen>($"EXECUTE uspWEB_GetChairmen 0")
                .ToList();


            return View("Views/StateFamily/GetChairmen.cshtml", result);
        }
        // GET: ChairmanInfoBlock
        [Route("GetChairmanInfoBlock")]
        public IActionResult GetChairmanInfoBlock(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var result = _context.Database
                .SqlQuery<SPGetChairmanInfoBlock>($"uspWEB_GetChairmanInfoBlock {id}")
                .ToList();


            //return View(result);
            //return Task.FromResult<IActionResult>(View("Views/StateFamily/ChairmanDetails.cshtml", spgetchairmanblock));
            return View("Views/StateFamily/ChairmanDetails.cshtml", result);
        }
        
        // GET: Chairmen
        [Route("GetDDs")]
        public IActionResult GetDDs()
        {
            // Run Sproc
            var result = _context.Database
                .SqlQuery<SPGetDDs>($"EXECUTE uspWEB_GetDDs 0")
                .ToList();


            return View("Views/StateFamily/GetDDs.cshtml", result);
        }

        ////////// GET: FourthDegreeOfficers
        ////////[Route("GetFourthDegreeOfficers")]
        ////////public IActionResult GetFourthDegreeOfficers()
        ////////{
        ////////    // trying to create a view model with multiple SPGetChairmanInfoBlock
        ////////    //https://stackoverflow.com/questions/67622116/add-multiple-records-of-the-same-model-in-asp-net-mvc
        ////////    //var foModel = new SPFourthDegreeOfficersViewModel();
        ////////    //foModel.FourthDegreeOfficers = new List<SPGetChairmanInfoBlock>();

        ////////    //var result = _context.Database
        ////////    //    .SqlQuery<SPGetChairmanInfoBlock>($"uspWEB_GetChairmanInfoBlock 1")
        ////////    //    .ToList();
        ////////    //foModel.FourthDegreeOfficers.Add(new SPGetChairmanInfoBlock(result));

        ////////    //var result2 = _context.Database
        ////////    //    .SqlQuery<SPGetChairmanInfoBlock>($"uspWEB_GetChairmanInfoBlock 2")
        ////////    //    .ToList();
        ////////    //foModel.FourthDegreeOfficers.Add(new SPGetChairmanInfoBlock(result2));

        ////////    //return View("Views/StateFamily/GetFourthDegreeOfficers.cshtml", result);
        ////////    return View("Views/StateFamily/FourthDegreeOfficers.cshtml");
        ////////}

        // GET: FourthDegreeOfficers
        [Route("FourthDegreeOfficers")]
        public IActionResult GetFourthDegreeOfficers()
        {

            var result = _context.Database
                .SqlQuery<SPGetChairmanInfoBlock>($"uspWEB_GetFourthDegreeOfficers")
                .ToList();


            //return View(result);
            //return Task.FromResult<IActionResult>(View("Views/StateFamily/ChairmanDetails.cshtml", spgetchairmanblock));
            return View("Views/StateFamily/FourthDegreeOfficers.cshtml", result);
        }
    }
}

using com.sun.tools.@internal.xjc.api;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Threading.Tasks;

namespace KofCWSCWebsite.Pages.Councils
{
    public class MoveCouncilModel : PageModel
    {
        public class Message
        {
            public string messageText { get; set; }
        }

        private readonly ApiHelper _ApiHelper;
        public string ResultMessage { get; set; }
        public List<Message> Messages { get; set; }

        public MoveCouncilModel(ApiHelper apiHelper)
        {
            _ApiHelper = apiHelper;
        }

        [BindProperty]
        public int FromCouncil { get; set; }

        [BindProperty]
        public int ToCouncil { get; set; }

        [BindProperty]
        public bool Post { get; set; }

        public SelectList CouncilOptions { get; set; }

        public async Task OnGetAsync()
        {
            await PopCouncils();
            //var myCouncils = await _ApiHelper.GetAsync<List<TblValCouncil>>("Councils") ?? new List<TblValCouncil>();
            //CouncilOptions = new SelectList(
            //myCouncils.Select(c => new
            //{
            //    Value = c.CNumber,
            //    Text = $"{c.CNumber} - {c.CName}"
            //}),
            //    "Value",
            //    "Text"
            //);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // add validation
            // both councils need to be different
            if (FromCouncil == ToCouncil)
            {
                ResultMessage = "From and To Councils must be differenct";
                await PopCouncils();
                return Page();
            }
            await PopCouncils();
            //var myCouncils = await _ApiHelper.GetAsync<List<TblValCouncil>>("Councils") ?? new List<TblValCouncil>();
            //CouncilOptions = new SelectList(
            //myCouncils.Select(c => new
            //{
            //    Value = c.CNumber,
            //    Text = $"{c.CNumber} - {c.CName}"
            //}),
            //    "Value",
            //    "Text"
            //);
            //new SelectList(myCouncils, "CNumber", "CName");

            // You can call your MoveCouncil method here
            ResultMessage = $"Moving from {FromCouncil} to {ToCouncil}, Post = {Post}";

            string json = await _ApiHelper.GetAsync<string>($"MoveCouncil/{FromCouncil}/{ToCouncil}/{Post}");
            var messages = JsonSerializer.Deserialize<List<Message>>(json);
            ResultMessage = messages?.FirstOrDefault()?.messageText;


            return Page(); // Stay on the same page
        }
        private async Task PopCouncils()
        {
            var myCouncils = await _ApiHelper.GetAsync<List<TblValCouncil>>("Councils") ?? new List<TblValCouncil>();
            CouncilOptions = new SelectList(
            myCouncils.Select(c => new
            {
                Value = c.CNumber,
                Text = $"{c.CNumber} - {c.CName}"
            }),
                "Value",
                "Text"
            );
        }
    }
}

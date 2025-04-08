using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    public class NecImpNecrologyVM
    {
        [DisplayName("Status")]
        public string DecStatus { get; set; }

        [DisplayName("Submission Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime? SubDate { get; set; }

        public string? SubFname { get; set; }

        public string? SubLname { get; set; }

        public string? SubEmail { get; set; }

        public string? SubRole { get; set; }

        [DisplayName("Council")]
        public int? CouncilId { get; set; }

        [DisplayName("Prefix")]
        public string? DecPrefix { get; set; }

        [DisplayName("First Name")]
        public string? DecFname { get; set; }

        [DisplayName("Middle Name")]
        public string? DecMname { get; set; }

        [DisplayName("Last Name")]
        public string? DecLname { get; set; }

        [DisplayName("Suffix")]
        public string? DecSuffix { get; set; }

        public string? DecFmorKn { get; set; }

        public string? Fmprefix { get; set; }

        public string? Fmfname { get; set; }

        public string? Fmmname { get; set; }

        public string? Fmlname { get; set; }

        public string? Fmsuffix { get; set; }

        public string? Relationship { get; set; }

        public DateTime? Dod { get; set; }
        [DisplayName("Deceased Member ID")]
        public int? DecMemberId { get; set; }

        public string? MemberType { get; set; }

        public string? DecOfficesHeld { get; set; }

        public int? AssemblyId { get; set; }

        public string? Nokprefix { get; set; }

        public string? Nokfname { get; set; }

        public string? Nokmname { get; set; }

        public string? Noklname { get; set; }

        public string? Noksuffix { get; set; }

        public string? Nokrelate { get; set; }

        public string? Nokaddress1 { get; set; }

        public string? Nokaddress2 { get; set; }

        public string? Nokcity { get; set; }

        public string? Nokstate { get; set; }

        public string? Nokzip { get; set; }

        public string? Nokcountry { get; set; }

        public string? Comments { get; set; }

        public int Id { get; set; }
        public long SubID { get; set; }
    }
}

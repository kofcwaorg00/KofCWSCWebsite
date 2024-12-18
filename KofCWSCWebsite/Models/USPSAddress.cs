using Microsoft.EntityFrameworkCore;

namespace KofCWSCWebsite.Models
{
    // Root USPS response class
    public class USPSAddress
    {
        public string Firm { get; set; } // "firm": "string"

        public Address Address { get; set; } // Nested Address object

        public AdditionalInfo AdditionalInfo { get; set; } // Nested AdditionalInfo object

        public List<Correction> Corrections { get; set; } // List of corrections

        public List<Match> Matches { get; set; } // List of matches

        public List<string> Warnings { get; set; } // List of warnings
    }

    // Class to represent the "address" section
    [Keyless]
    public class Address
    {
        public string StreetAddress { get; set; } // "streetAddress": "string"
        public string StreetAddressAbbreviation { get; set; } // "streetAddressAbbreviation": "string"
        public string SecondaryAddress { get; set; } // "secondaryAddress": "string"
        public string CityAbbreviation { get; set; } // "cityAbbreviation": "string"
        public string City { get; set; } // "city": "string"
        public string State { get; set; } // "state": "st"
        public string ZIPCode { get; set; } // "ZIPCode": "string"
        public string ZIPPlus4 { get; set; } // "ZIPPlus4": "string"
        public string Urbanization { get; set; } // "urbanization": "string"
    }

    // Class to represent the "additionalInfo" section
    public class AdditionalInfo
    {
        public string DeliveryPoint { get; set; } // "deliveryPoint": "string"
        public string CarrierRoute { get; set; } // "carrierRoute": "strin"
        public string DPVConfirmation { get; set; } // "DPVConfirmation": "Y"
        public string DPVCMRA { get; set; } // "DPVCMRA": "Y"
        public string Business { get; set; } // "business": "Y"
        public string CentralDeliveryPoint { get; set; } // "centralDeliveryPoint": "Y"
        public string Vacant { get; set; } // "vacant": "Y"
    }

    // Class to represent items in the "corrections" list
    public class Correction
    {
        public string Code { get; set; } // "code": "s"
        public string Text { get; set; } // "text": "string"
    }

    // Class to represent items in the "matches" list
    public class Match
    {
        public string Code { get; set; } // "code": "s"
        public string Text { get; set; } // "text": "string"
    }
}

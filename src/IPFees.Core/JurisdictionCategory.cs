namespace IPFees.Core
{
    public enum JurisdictionCategory
    {
        OfficialFees,
        PartnerFees
    }

    public static class ExtensionMethods
    {
        public static string ValueAsString(this JurisdictionCategory e)
        {
            return e switch
            {
                JurisdictionCategory.OfficialFees => "Official Fees",
                JurisdictionCategory.PartnerFees => "Partner Fees",
                _ => "N/A",
            };
        }
    }
}

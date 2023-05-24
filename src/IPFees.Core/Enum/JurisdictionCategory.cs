namespace IPFees.Core.Enum
{
    public enum JurisdictionCategory
    {
        OfficialFees,
        PartnerFees,
        TranslationFees
    }

    public static class ExtensionMethods1
    {
        public static string ValueAsString(this JurisdictionCategory e)
        {
            return e switch
            {
                JurisdictionCategory.OfficialFees => "Official Fees",
                JurisdictionCategory.PartnerFees => "Partner Fees",
                JurisdictionCategory.TranslationFees => "Translation Fees",
                _ => "N/A",
            };
        }
    }
}

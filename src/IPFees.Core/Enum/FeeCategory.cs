namespace IPFees.Core.Enum
{
    public enum FeeCategory
    {
        OfficialFees,
        PartnerFees,
        TranslationFees,
    }

    public static class ExtensionMethods1
    {
        public static string ValueAsString(this FeeCategory e)
        {
            return e switch
            {
                FeeCategory.OfficialFees => "Official Fees",
                FeeCategory.PartnerFees => "Partner Fees",
                FeeCategory.TranslationFees => "Translation Fees",
                _ => "N/A",
            };
        }
    }
}

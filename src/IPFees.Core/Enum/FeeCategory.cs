namespace IPFees.Core.Enum
{
    public enum FeeCategory
    {
        OfficialFees,
        PartnerFees,        
    }

    public static class ExtensionMethods1
    {
        public static string ValueAsString(this FeeCategory e)
        {
            return e switch
            {
                FeeCategory.OfficialFees => "Official Fees",
                FeeCategory.PartnerFees => "Partner Fees",                
                _ => "N/A",
            };
        }
    }
}

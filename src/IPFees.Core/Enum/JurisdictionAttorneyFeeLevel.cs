namespace IPFees.Core.Enum
{
    public enum JurisdictionAttorneyFeeLevel
    {
        Level1,
        Level2,
        Level3,
    }

    public static class ExtensionMethods2
    {
        public static string ValueAsString(this JurisdictionAttorneyFeeLevel e)
        {
            return e switch
            {
                JurisdictionAttorneyFeeLevel.Level1 => "Level 1",
                JurisdictionAttorneyFeeLevel.Level2 => "Level 2",
                JurisdictionAttorneyFeeLevel.Level3 => "Level 3",
                _ => "N/A",
            };
        }
    }
}

namespace IPFees.Core.Enum
{
    public enum AttorneyFeeLevel
    {
        Level1,
        Level2,
        Level3,
    }

    public static class ExtensionMethods2
    {
        public static string ValueAsString(this AttorneyFeeLevel e)
        {
            return e switch
            {
                AttorneyFeeLevel.Level1 => "Level 1",
                AttorneyFeeLevel.Level2 => "Level 2",
                AttorneyFeeLevel.Level3 => "Level 3",
                _ => "N/A",
            };
        }
    }
}

namespace IPFees.Core.Enum
{
    public enum ServiceFeeLevel
    {
        Level1,
        Level2,
        Level3,
    }

    public static class ExtensionMethods2
    {
        public static string ValueAsString(this ServiceFeeLevel e)
        {
            return e switch
            {
                ServiceFeeLevel.Level1 => "Level 1",
                ServiceFeeLevel.Level2 => "Level 2",
                ServiceFeeLevel.Level3 => "Level 3",
                _ => "N/A",
            };
        }
    }
}

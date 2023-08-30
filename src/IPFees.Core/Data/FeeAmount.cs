namespace IPFees.Core.Data
{
    public class FeeAmount
    {
        public double MandatoryAmount { get; set; }
        public double OptionalAmount { get; set; }
        public string Currency { get; set; }

        public FeeAmount(double mandatoryAmount, double optionalAmount, string currency)
        {
            MandatoryAmount = mandatoryAmount;
            OptionalAmount = optionalAmount;
            Currency = currency;
        }

        public static FeeAmount Add(FeeAmount first, FeeAmount second)
        {
            if (first.Currency != second.Currency)
            {
                // TODO: Uncomment this in production
                //throw new ArgumentException("Currencies are not the same.");
            }

            double totalMandatory = first.MandatoryAmount + second.MandatoryAmount;
            double totalOptional = first.OptionalAmount + second.OptionalAmount;

            return new FeeAmount(totalMandatory, totalOptional, first.Currency);
        }

        public override string ToString()
        {
            return $"Mandatory Amount: {MandatoryAmount} {Currency}\n" +
                   $"Optional Amount: {OptionalAmount} {Currency}";
        }
    }
}

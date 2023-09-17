namespace IPFees.Core.Data
{
    public class Fee
    {
        public double MandatoryAmount { get; set; }
        public double OptionalAmount { get; set; }
        public string Currency { get; set; }

        public Fee(double mandatoryAmount, double optionalAmount, string currency)
        {
            MandatoryAmount = mandatoryAmount;
            OptionalAmount = optionalAmount;
            Currency = currency;
        }

        public static Fee Add(Fee first, Fee second)
        {
            if (first.Currency != second.Currency)
            {                
                throw new ArgumentException($"Cannot add different currencies ({first.Currency} and {second.Currency}).");
            }

            double totalMandatory = first.MandatoryAmount + second.MandatoryAmount;
            double totalOptional = first.OptionalAmount + second.OptionalAmount;

            return new Fee(totalMandatory, totalOptional, first.Currency);
        }

        public override string ToString()
        {
            return $"Mandatory Amount: {MandatoryAmount} {Currency}\n" +
                   $"Optional Amount: {OptionalAmount} {Currency}";
        }
    }
}

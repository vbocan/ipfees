namespace IPFees.API.Validator
{
    public interface IApiKeyValidator
    {
        bool IsValid(string apiKey);
    }
}

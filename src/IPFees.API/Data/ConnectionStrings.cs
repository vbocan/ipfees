﻿namespace IPFees.API.Data
{
    public class ConnectionStrings
    {
        public const string SectionName = nameof(ConnectionStrings);
        public string MongoDbConnection { get; set; } = string.Empty;
    }
}

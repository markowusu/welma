using Microsoft.Extensions.Options;
using Npgsql;
using Welma.Core.Configurations;

namespace Welma.Core.Database
{

/// <summary>
// This is my postgresql Factory to make connections to the database and any connection String settings
/// </summary>


    public sealed class PostgresFactory
    {
        private readonly ConnectionString? _connectionString;


        //  add the this() to ensure constructor chaining where each constructor calls the base constructor. because we are using a value parameter from the appsettings
        public PostgresFactory(IOptionsMonitor<AppSettings> appSettings)
        {
            // returning a NullRerefence error when the method we are trying to  access in the appsettings is null. 

            _connectionString = appSettings.CurrentValue.connectionString ?? throw new NullReferenceException("Connection string can't be null");
            
        }

        public NpgsqlConnection New()
        {
            return new (_connectionString?.Postgres);
        }
    }
}
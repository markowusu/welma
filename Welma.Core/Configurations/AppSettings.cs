namespace Welma.Core.Configurations
{

    public  sealed class AppSettings {
        public AppSettings()
        {
        }

        public ConnectionString  connectionString { get; set; }
    }


    public sealed class ConnectionString {
        public  string? Postgres {get; init;}

// ======>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        // I can also define more  connection settings here 

        // public string RedisUssd = {get; init;} = null!; 

        //public GraphQLSettings GraphQLSettings = {get; init;} = null!;

        // public string FileCOntet {get; init;} = null!;
    }
}
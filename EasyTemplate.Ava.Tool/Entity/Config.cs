namespace EasyTemplate.Ava.Tool.Entity;

public class Config
{
    public Application Application { get; set; }
    public Cache Cache { get; set; }
    public Dbconnection DbConnection { get; set; }
}

public class Application
{
    public string Theme { get; set; }
    public string Color { get; set; }
    public string Language { get; set; }
    public bool CloseToTray { get; set; }
    public bool Remember { get; set; }
    public bool Beta { get; set; }
}

public class Cache
{
    public string CacheType { get; set; }
    public string RedisConnectionString { get; set; }
    public string InstanceName { get; set; }
}

public class Dbconnection
{
    public bool EnableConsoleSql { get; set; }
    public Connectionconfig[] ConnectionConfigs { get; set; }
}

public class Connectionconfig
{
    public string ConfigId { get; set; }
    public string DbType { get; set; }
    public string ConnectionString { get; set; }
    public Dbsettings DbSettings { get; set; }
    public Tablesettings TableSettings { get; set; }
    public Seedsettings SeedSettings { get; set; }
}

public class Dbsettings
{
    public bool EnableDiffLog { get; set; }
    public bool EnableInitDb { get; set; }
    public bool EnableUnderLine { get; set; }
}

public class Tablesettings
{
    public bool EnableInitTable { get; set; }
    public bool EnableIncreTable { get; set; }
    public bool EnableUnderLine { get; set; }
}

public class Seedsettings
{
    public bool EnableInitSeed { get; set; }
    public bool EnableIncreSeed { get; set; }
}

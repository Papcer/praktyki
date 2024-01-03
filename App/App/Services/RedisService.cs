using StackExchange.Redis;

namespace App.Services;

public class RedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<byte[]> GetAsync(string key)
    {
        _logger.LogInformation($"GetAsync - Key: {key}");
        return await _database.StringGetAsync(key);
    }
    
    
    public async Task SetAsync(string key, byte[] value, TimeSpan expiry)
    {
        try
        {
            await _database.StringSetAsync(key, value, expiry);
            _logger.LogInformation($"SetPdfAsync - Key: {key}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SetPdfAsync: {ex.Message}");
            throw;
        }
    }
}
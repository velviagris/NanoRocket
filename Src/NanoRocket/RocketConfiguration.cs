namespace NanoRocket;

/// <summary>
/// NanoRocket Configuration structure
/// </summary>
public record RocketConfiguration
{
    /// <summary>
    /// Consumers configuration
    /// </summary>
    public IDictionary<string, ConsumerConfig>? Consumers { get; set; } 
    /// <summary>
    /// Producers configuration
    /// </summary>
    public IDictionary<string, ProducerConfig>? Producers { get; set; } 
}

/// <summary>
/// Base config
/// </summary>
public record ConfigBase
{
    /// <summary>
    /// Endpoints
    /// </summary>
    public string? Endpoints { get; set; }
    /// <summary>
    /// Topic
    /// </summary>
    public string? Topic { get; set; }
    /// <summary>
    /// Access key
    /// </summary>
    public string? AccessKey { get; set; }
    /// <summary>
    /// Secret key
    /// </summary>
    public string? SecretKey { get; set; }
}

/// <summary>
/// Consumer configuration
/// </summary>
public record ConsumerConfig : ConfigBase
{
    /// <summary>
    /// Consumer group
    /// </summary>
    public string? ConsumerGroup { get; set; }
}

/// <summary>
/// Producer configuration
/// </summary>
public record ProducerConfig  : ConfigBase
{
    
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NPlus1Detector;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder AddNPlusOneDetector(
        this DbContextOptionsBuilder optionsBuilder,
        NPlusOneDetectorOptions? options = null,
        ILogger<NPlusOneDetectorInterceptor>? logger = null)
    {
        options ??= new NPlusOneDetectorOptions();

        logger ??= CreateDefaultLogger();

        var interceptor = new NPlusOneDetectorInterceptor(logger, options);
        return optionsBuilder.AddInterceptors(interceptor);
    }

    private static ILogger<NPlusOneDetectorInterceptor> CreateDefaultLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        return loggerFactory.CreateLogger<NPlusOneDetectorInterceptor>();
    }
}
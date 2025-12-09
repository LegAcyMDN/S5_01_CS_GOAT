using S5_01_App_CS_GOAT.Models.Repository;
using Microsoft.Extensions.Logging;

namespace S5_01_App_CS_GOAT.Services
{
    public enum TimedActionFrequency : int
    {
        Minutely = 1,
        QuarterHourly = 15,
        DemiHourly = 30,
        Hourly = 60,
        OctaHourly = 480,
        Daily = 1440,
        Weekly = 10080,
        BiWeekly = 20160,
        Monthly = 43200,
        Semesterly = 259200,
        Yearly = 525600
    }

    public interface ITimedAction
    {
        public Task Tick(IServiceScope scope);
        public static abstract TimedActionFrequency TickFrequency { get; }
    }

    public class TimedActionService<TRepository, TEntity, TIdentifier> : BackgroundService
        where TRepository : IReadableRepository<TEntity, TIdentifier>
        where TEntity : ITimedAction
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimedActionService<TRepository, TEntity, TIdentifier>> _logger;

        public TimedActionService(IServiceProvider serviceProvider, ILogger<TimedActionService<TRepository, TEntity, TIdentifier>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        TRepository repository = scope.ServiceProvider.GetRequiredService<TRepository>();

                        TimedActionFrequency frequency = TEntity.TickFrequency;
                        IEnumerable<TEntity> entities = await repository.GetAllAsync();
                        foreach (TEntity entity in entities)
                        {
                            try
                            {
                                await entity.Tick(scope);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error occurred while processing Tick() for entity {entity}.");
                            }
                        }
                    }
                    int nextTickInMinutes = (int)TEntity.TickFrequency;
                    int nextTickInMilliseconds = nextTickInMinutes * 60 * 1000;
                    await Task.Delay(nextTickInMilliseconds, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in TimedActionService.");
                    break;
                }
            }
        }
    }
}

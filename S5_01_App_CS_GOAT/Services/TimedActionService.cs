using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Services
{
    /// <summary>
    /// Defines the frequency at which timed actions should execute
    /// </summary>
    /// <remarks>
    /// Values represent the number of minutes between executions
    /// </remarks>
    public enum TimedActionFrequency : int
    {
        /// <summary>Every minute</summary>
        Minutely = 1,
        /// <summary>Every 15 minutes</summary>
        QuarterHourly = 15,
        /// <summary>Every 30 minutes</summary>
        DemiHourly = 30,
        /// <summary>Every hour</summary>
        Hourly = 60,
        /// <summary>Every 8 hours</summary>
        OctaHourly = 480,
        /// <summary>Every day (24 hours)</summary>
        Daily = 1440,
        /// <summary>Every week</summary>
        Weekly = 10080,
        /// <summary>Every two weeks</summary>
        BiWeekly = 20160,
        /// <summary>Every month (30 days)</summary>
        Monthly = 43200,
        /// <summary>Every 6 months (180 days)</summary>
        Semesterly = 259200,
        /// <summary>Every year (365 days)</summary>
        Yearly = 525600
    }

    /// <summary>
    /// Interface for entities that require periodic execution of actions
    /// </summary>
    public interface ITimedAction
    {
        /// <summary>
        /// Executes the timed action
        /// </summary>
        /// <param name="scope">The service scope providing access to scoped services</param>
        Task Tick(IServiceScope scope);

        /// <summary>
        /// Gets the frequency at which this action should execute
        /// </summary>
        static abstract TimedActionFrequency TickFrequency { get; }
    }

    /// <summary>
    /// Background service that periodically executes timed actions on entities
    /// </summary>
    /// <typeparam name="TRepository">The repository type for accessing entities</typeparam>
    /// <typeparam name="TEntity">The entity type that implements ITimedAction</typeparam>
    /// <typeparam name="TIdentifier">The identifier type for the entity</typeparam>
    /// <remarks>
    /// This service runs continuously in the background, retrieving all entities from the repository
    /// and calling their Tick method at the frequency specified by the entity's TickFrequency.
    /// Errors in individual entity processing are logged but do not stop the service.
    /// </remarks>
    public class TimedActionService<TRepository, TEntity, TIdentifier> : BackgroundService
        where TRepository : IReadableRepository<TEntity, TIdentifier>
        where TEntity : class, ITimedAction
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimedActionService<TRepository, TEntity, TIdentifier>> _logger;

        /// <summary>
        /// Initializes a new instance of the TimedActionService
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scoped services</param>
        /// <param name="logger">The logger for recording errors and diagnostics</param>
        public TimedActionService(
            IServiceProvider serviceProvider,
            ILogger<TimedActionService<TRepository, TEntity, TIdentifier>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service, continuously processing timed actions until cancellation is requested
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop the service</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for each execution cycle
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        TRepository repository = scope.ServiceProvider.GetRequiredService<TRepository>();

                        // Retrieve all entities and execute their Tick method
                        IEnumerable<TEntity> entities = await repository.GetAllAsync();
                        foreach (TEntity entity in entities)
                        {
                            try
                            {
                                await entity.Tick(scope);
                            }
                            catch (Exception ex)
                            {
                                // Log errors but continue processing other entities
                                _logger.LogError(ex, $"Error occurred while processing Tick() for entity {entity}.");
                            }
                        }
                    }
                    // Wait until the next scheduled execution
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

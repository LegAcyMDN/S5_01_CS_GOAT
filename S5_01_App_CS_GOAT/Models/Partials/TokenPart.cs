using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class for Token that implements user-dependent and timed action functionality
    /// </summary>
    /// <remarks>
    /// This partial provides business logic for authentication/verification tokens including:
    /// - Expiry date tracking and automatic cleanup
    /// - Periodic validity checking via the timed action system
    /// - Automatic deletion of expired tokens
    /// </remarks>
    public partial class Token : IUserDependant, ITimedAction
    {
        /// <summary>
        /// Gets the user ID that owns this token
        /// </summary>
        public int? DependantUserId => UserId;

        /// <summary>
        /// Defines how frequently this token's validity should be checked
        /// </summary>
        /// <remarks>
        /// Tokens are checked every 15 minutes to clean up expired tokens from the database
        /// </remarks>
        public static TimedActionFrequency TickFrequency => TimedActionFrequency.QuarterHourly;

        /// <summary>
        /// Executes periodic validity checks on this token
        /// </summary>
        /// <param name="scope">The service scope providing access to the token repository</param>
        /// <remarks>
        /// This method is called by the TimedActionService at the specified frequency.
        /// It checks if the token is still valid and deletes it if it has expired.
        /// </remarks>
        public async Task Tick(IServiceScope scope)
        {
            IDataRepository<Token, int> tokenRepository = scope.ServiceProvider.GetRequiredService<IDataRepository<Token, int>>();
            _ = await CheckStillValid(tokenRepository);
        }

        /// <summary>
        /// Checks if this token is still valid and deletes it if expired
        /// </summary>
        /// <param name="tokenRepository">The repository used for token deletion</param>
        /// <returns>True if the token is still valid; false if it has been deleted due to expiry</returns>
        /// <remarks>
        /// This method compares the token's expiry date against the current time.
        /// If the token has expired, it is immediately deleted from the database.
        /// This allows for automatic cleanup of expired authentication tokens.
        /// </remarks>
        public async Task<bool> CheckStillValid(IWriteRepository<Token> tokenRepository)
        {
            // Check if token has expired
            if (TokenExpiry <= DateTime.Now)
            {
                // Delete the expired token from the database
                await tokenRepository.DeleteAsync(this);
                return false;
            }
            return true;
        }
    }
}
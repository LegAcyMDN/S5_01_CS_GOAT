using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Token : IUserDependant, ITimedAction
    {
        public int? DependantUserId => UserId;

        public static TimedActionFrequency TickFrequency => TimedActionFrequency.QuarterHourly;

        public async Task Tick(IServiceScope scope)
        {
            IDataRepository<Token, int> tokenRepository = scope.ServiceProvider.GetRequiredService<IDataRepository<Token, int>>();
            _ = await CheckStillValid(tokenRepository);
        }

        public async Task<bool> CheckStillValid(IWriteRepository<Token> tokenRepository)
        {
            if (TokenExpiry <= DateTime.Now)
            {
                await tokenRepository.DeleteAsync(this);
                return false;
            }
            return true;
        }
    }
}
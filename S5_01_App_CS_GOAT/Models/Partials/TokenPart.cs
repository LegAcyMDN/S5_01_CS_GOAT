using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Token : IUserDependant, ITimedAction
    {
        public int? DependantUserId { get => this.UserId; }

        public static TimedActionFrequency TickFrequency => TimedActionFrequency.QuarterHourly;

        public async Task Tick(IServiceScope scope)
        {
            if (this.TokenExpiry <= DateTime.Now)
            {
                IDataRepository<Token, int> tokenRepository = scope.ServiceProvider.GetRequiredService<IDataRepository<Token, int>>();
                await tokenRepository.DeleteAsync(this);
            }
        }
    }
}
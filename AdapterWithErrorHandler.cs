using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
{
    public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
        : base(configuration, logger)
    {
        OnTurnError = async (turnContext, exception) =>
        {
            logger.LogError($"Exception caught : {exception.Message}");
            await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
        };
    }
}

using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace DialogBot1.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog(GreetingDialog greetingDialog, FlightBookingDialog bookingDialog) : base(nameof(MainDialog))
        {
            // Add the sub-dialogs
            AddDialog(greetingDialog);
            AddDialog(bookingDialog);

            // Define the main flow: greet then book
            var waterfallSteps = new WaterfallStep[]
            {
                RunGreetingAsync,
                RunBookingAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // Set the initial dialog
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RunGreetingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Run the greeting dialog
            return await stepContext.BeginDialogAsync(nameof(GreetingDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> RunBookingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Run the booking dialog after greeting
            return await stepContext.BeginDialogAsync(nameof(FlightBookingDialog), null, cancellationToken);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace DialogBot1.Dialogs
{
    // FlightBookingDialog handles a multi-step booking flow
    public class FlightBookingDialog : ComponentDialog
    {
        // Prompt IDs
        private const string DepartureCityPrompt = "departureCityPrompt";
        private const string DestinationCityPrompt = "destinationCityPrompt";
        private const string DepartureDatePrompt = "departureDatePrompt";
        private const string FlightSelectionPrompt = "textPrompt"; // Used later in step 5

        public FlightBookingDialog() : base(nameof(FlightBookingDialog))
        {
            // Define each step in the waterfall dialog
            var waterfallSteps = new WaterfallStep[]
            {
                AskDepartureCityAsync,     // Step 1
                AskDestinationCityAsync,   // Step 2
                AskDepartureDateAsync,     // Step 3
                SearchFlightsAsync,        // Step 4
                ShowAvailableFlightsAsync, // Step 5
                AskFlightSelectionAsync,   // Step 6
                ConfirmBookingAsync        // Step 7
            };

            // Register the waterfall and prompts
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // Add prompts with optional validation
            AddDialog(new TextPrompt(DepartureCityPrompt));
            AddDialog(new TextPrompt(DestinationCityPrompt));
            AddDialog(new TextPrompt(DepartureDatePrompt, DateValidatorAsync));
            AddDialog(new TextPrompt(FlightSelectionPrompt)); // Needed for step 5!

            // Set the initial entry point to the waterfall
            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Ask for the departure city
        private async Task<DialogTurnResult> AskDepartureCityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(DepartureCityPrompt, new PromptOptions
            {
                Prompt = MessageFactory.Text("Which city are you departing from?")
            }, cancellationToken);
        }

        // Step 2: Ask for the destination city
        private async Task<DialogTurnResult> AskDestinationCityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["departureCity"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(DestinationCityPrompt, new PromptOptions
            {
                Prompt = MessageFactory.Text("Which city are you traveling to?")
            }, cancellationToken);
        }

        // Step 3: Ask for the date of departure
        private async Task<DialogTurnResult> AskDepartureDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["destinationCity"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(DepartureDatePrompt, new PromptOptions
            {
                Prompt = MessageFactory.Text("When do you plan to depart?")
            }, cancellationToken);
        }

        // Validator to ensure the date is valid
        private async Task<bool> DateValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return DateTime.TryParse(promptContext.Recognized.Value, out _);
        }

        // Step 4: Simulate searching flights
        private async Task<DialogTurnResult> SearchFlightsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["departureDate"] = (string)stepContext.Result;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("🔍 Searching for available flights..."), cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Step 5: Show adaptive card and ask user to select
        private async Task<DialogTurnResult> ShowAvailableFlightsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                // Load adaptive card from local file
                var cardJson = System.IO.File.ReadAllText("Resources/flightCard.json");

                var attachment = new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = Newtonsoft.Json.JsonConvert.DeserializeObject(cardJson)
                };

                var reply = MessageFactory.Attachment(attachment);
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception ex)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("⚠️ Failed to load flight card: " + ex.Message), cancellationToken);
            }

            return await stepContext.PromptAsync(FlightSelectionPrompt, new PromptOptions
            {
                Prompt = MessageFactory.Text("Please type the number of the flight you want to book.")
            }, cancellationToken);
        }

        // Step 6: Capture the flight selection
        private async Task<DialogTurnResult> AskFlightSelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["selectedFlight"] = (string)stepContext.Result;
            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Step 7: Final confirmation
        private async Task<DialogTurnResult> ConfirmBookingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string from = (string)stepContext.Values["departureCity"];
            string to = (string)stepContext.Values["destinationCity"];
            string date = (string)stepContext.Values["departureDate"];
            string flight = (string)stepContext.Values["selectedFlight"];

            string confirmation = $"✅ *Booking confirmed!*\n\n" +
                                  $"**From**: {from}\n" +
                                  $"**To**: {to}\n" +
                                  $"**Time**: {date}\n" +
                                  $"**Flight**: {flight}";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(confirmation), cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}

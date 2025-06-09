using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace DialogBot1.Dialogs
{
    // GreetingDialog inherits from ComponentDialog
    public class GreetingDialog : ComponentDialog
    {
        // Define IDs for the prompts used in the dialog.
        private const string NamePromptId = "namePrompt";

        // Constructor - no parameters, sets dialog ID properly using nameof operator
        public GreetingDialog() : base(nameof(GreetingDialog))
        {
            // Define the sequence of waterfall steps in the dialog
            var waterfallSteps = new WaterfallStep[]
            {
                AskForNameAsync,   // Step 1: Prompt user for their name
                GreetUserAsync     // Step 2: Greet the user by name
            };

            // Add the WaterfallDialog with a unique ID
            AddDialog(new WaterfallDialog(nameof(GreetingDialog), waterfallSteps));

            // Add a TextPrompt dialog for asking the user's name
            AddDialog(new TextPrompt(NamePromptId));

            // Set the starting dialog to our WaterfallDialog
            InitialDialogId = nameof(GreetingDialog);
        }

        // Step 1: Prompt the user to enter their name
        private async Task<DialogTurnResult> AskForNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Use PromptAsync to ask for the name, with a message
            return await stepContext.PromptAsync(NamePromptId, new PromptOptions
            {
                Prompt = MessageFactory.Text("What is your name?") // Message shown to user
            }, cancellationToken);
        }

        // Step 2: Greet the user using the name they provided
        private async Task<DialogTurnResult> GreetUserAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Retrieve the name entered by the user in the previous step
            var name = (string)stepContext.Result;

            // Send a greeting message back to the user
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello, {name}!"), cancellationToken);

            // End this dialog and return control to the caller
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}

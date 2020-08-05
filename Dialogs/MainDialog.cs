// Created By KASHYAP SAVSANI on 4th Aug,2020

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;


namespace Microsoft.BotBuilderSamples
{
	
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
		public string Category;
		public string Location;

        public MainDialog(ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            // Define the main dialog and its related components.
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceCardStep1Async,
                ShowCardStep1Async,
				ChoiceCardStep2Async,
                ShowCardStep2Async,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        // 1. Prompts the user if the user is not in the middle of a dialog.
        // 2. Re-prompts the user when an invalid input is received.
        private async Task<DialogTurnResult> ChoiceCardStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ChoiceCardStep1Async");

            // Create the PromptOptions which contain the prompt and re-prompt messages.
            // PromptOptions also contains the list of choices available to the user.
            var options1 = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Which Job Category you would like to select?"),
                RetryPrompt = MessageFactory.Text("That was not a valid choice, please select a valid Job category."),
                Choices = GetChoicesCat(),
            };
			return await stepContext.PromptAsync(nameof(ChoicePrompt), options1, cancellationToken);
			
        }
		
		private async Task<DialogTurnResult> ChoiceCardStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ChoiceCardStep2Async");

            // Create the PromptOptions which contain the prompt and re-prompt messages.
            // PromptOptions also contains the list of choices available to the user.
            var options2 = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Which Job location would like to prefer?"),
                RetryPrompt = MessageFactory.Text("That was not a valid choice, please select a valid Location."),
                Choices = GetChoicesLoc(),
            };
			return await stepContext.PromptAsync(nameof(ChoicePrompt), options2, cancellationToken);
			
        }

        // Send a Rich Card response to the user based on their choice.
        // This method is only called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
        private async Task<DialogTurnResult> ShowCardStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ShowCardStep1Async");
            
            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments for the reply activity.
            var attachments = new List<Attachment>();
            
            // Reply to the activity we received with an activity.
            var reply = MessageFactory.Attachment(attachments);

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
			
			Category = ((FoundChoice)stepContext.Result).Value;

            // Give the user instructions about what to do next
			return await stepContext.NextAsync();
        }
		
		private async Task<DialogTurnResult> ShowCardStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ShowCardStep2Async");
            
            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments for the reply activity.
            var attachments = new List<Attachment>();
            
            // Reply to the activity we received with an activity.
            var reply = MessageFactory.Attachment(attachments);
			
			Location = ((FoundChoice)stepContext.Result).Value;
			
            // Decide which type of card(s) we are going to show the user
            switch (((FoundChoice)stepContext.Result).Value)
            {
                default:                   
                    foreach (var item in Cards.GetResult(Category,Location))
                    {
                        reply.Attachments.Add(item.ToAttachment());
                    }
                    break;
            }

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Type anything to see other Jobs available !"), cancellationToken);

            return await stepContext.EndDialogAsync();
        }

        private IList<Choice> GetChoicesCat()
        {
            var cardOptions = new List<Choice>()
            {

                new Choice() { Value = "Financial Services", Synonyms = new List<string>() { "financial" } },
				new Choice() { Value = "Consulting", Synonyms = new List<string>() { "consulting" } },
				new Choice() { Value = "IT Services", Synonyms = new List<string>() { "it services" } },
				new Choice() { Value = "Advertising", Synonyms = new List<string>() { "advertising" } },
				new Choice() { Value = "Media", Synonyms = new List<string>() { "media" } },
				new Choice() { Value = "Technology", Synonyms = new List<string>() { "technology" } },
				new Choice() { Value = "Business Management", Synonyms = new List<string>() { "business management" } },
				new Choice() { Value = "Marketing", Synonyms = new List<string>() { "marketing" } }
            };
            return cardOptions;
        }
        private IList<Choice> GetChoicesLoc()
        {
            var cardOptions = new List<Choice>()
            {

                new Choice() { Value = "Melbourne", Synonyms = new List<string>() { "melbourne" } },
                new Choice() { Value = "Sydney", Synonyms = new List<string>() { "sydney" } },
				new Choice() { Value = "San Francisco", Synonyms = new List<string>() { "san francisco" } },
                new Choice() { Value = "Brisbane", Synonyms = new List<string>() { "brisbane" } }
            };
            return cardOptions;
        }
    }
}

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SiteRequest.Model;
using SiteRequest.Services;

namespace SiteRequest.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        #region Variable
        private readonly UserState _userState;
        private readonly BotServiceState _botServiceState;
        #endregion
        public GreetingDialog(UserState userState) : base(nameof(GreetingDialog))
        {
            _userState = userState;
            //botServiceState = botServiceState ?? throw new System.ArgumentNullException(nameof(botServiceState));
            InitializeWaterfallDialog();

        }
        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));

            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";

        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //UserProfile userProfile = "";//await _botServiceState.UserProfileAccessor(stepContext.Context, () => new UserProfile());

            //if (string.IsNullOrEmpty(userProfile.Name))
            //{
            //    return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
            //    new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Hello...May i know your good name?")
            //    }, cancellationToken);

            //}
            //else
            //{
            return await stepContext.NextAsync(null, cancellationToken);
            // }

        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botServiceState.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                userProfile.Name = (string)stepContext.Result;
                await _botServiceState.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Heyy {0}. How can i help you today?", userProfile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);

        }
    }
}

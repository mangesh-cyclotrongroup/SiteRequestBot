using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SiteRequest.Services;

namespace SiteRequest.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        #region Variable
        private readonly UserState _userState;
        private readonly BotServiceState _botServiceState;

        #endregion


        public MainDialog(UserState userState) : base(nameof(MainDialog))
        {

            _userState = userState;
            //  _botServiceState = botServiceState ?? throw new System.ArgumentNullException(nameof(botServiceState));

            AddDialog(new OAuthPrompt(
            nameof(OAuthPrompt),
            new OAuthPromptSettings
            {
                ConnectionName = "TeamsBotService",
                Text = "Please Sign In",
                Title = "Sign In",
                Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
            }));
            InitializeWaterfallDialog();

        }
        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            //AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _userState));
            //AddDialog(new SiteRequestDialog($"{nameof(MainDialog)}.SiteRequest", _userState));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(MainDialog)}.InValid"));
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";

        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if ((Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success) || (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hey").Success) || (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hello").Success))
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);

            }
            else if ((Regex.Match(stepContext.Context.Activity.Text.ToLower(), "team").Success))
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.SiteRequest", null, cancellationToken);

            }

            else
            {
                return await stepContext.PromptAsync($"{ nameof(MainDialog)}.InValid",
                  new PromptOptions
                  {
                      Prompt = MessageFactory.Text("Wrong Input")
                  }, cancellationToken);

            }
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}

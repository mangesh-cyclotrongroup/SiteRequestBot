// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestChatBot.Model;

namespace TestChatBot.Dialogs
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text($"Welcome to SiteRequest Bot." + "Type anything to get started.");
                    // await turnContext.SendActivityAsync(reply, cancellationToken);
                    await SendIntroCardAsync(turnContext, cancellationToken);
                }
            }
        }
        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard();
            card.Title = "Welcome to SiteRequest Bot!";
            card.Text = @"Welcome to SiteRequest Bot.";
            card.Images = new List<CardImage>() { new CardImage(@"C:\Users\MangeshNikam\source\repos\TestChatBot\TestChatBot\Images\botsImage.jpg") };



            card.Buttons = new List<CardAction>()
            {


                new CardAction(ActionTypes.ImBack, "Create Team", null, "Create Team", "Create Team", "create Team"),
                new CardAction(ActionTypes.OpenUrl, "Ask a question", null, "Ask a question", "Ask a question", "https://stackoverflow.com/questions/tagged/botframework"),
          };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
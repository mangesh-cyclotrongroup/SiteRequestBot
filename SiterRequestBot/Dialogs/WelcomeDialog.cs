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
using SiteRequest.Model;

namespace SiteRequest.Dialogs
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
                    await SendWelcomeCard(turnContext, cancellationToken);
                }
            }
        }
        public async Task SendWelcomeCard(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //var card = File.ReadAllText(@".\Cards\WelcomeCard.json");


            var card = File.ReadAllText(@"C:\Users\MangeshNikam\source\repos\SiteRequest\SiteRequest\wwwroot\SiteCard.json");
            var parsedResult = AdaptiveCard.FromJson(card);
            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = parsedResult.Card
            };

            var activity = turnContext.Activity.CreateReply();
            activity.Attachments.Add(attachment);

            await turnContext.SendActivityAsync(activity, cancellationToken);
        }
        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {


            var card = new HeroCard();
            card.Title = "Welcome to SiteRequest Bot!";
            card.Text = @"Welcome to SiteRequest Bot.";
            card.Images = new List<CardImage>() { new CardImage(@"C:\Users\MangeshNikam\source\repos\SiteRequest\SiteRequest\Images\botsImage.jpg") };
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
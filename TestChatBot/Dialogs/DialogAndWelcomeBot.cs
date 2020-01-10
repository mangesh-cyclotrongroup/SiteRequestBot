// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SiteRequestBot.Helpers;
using SiteRequestBot.Model;

namespace SiteRequestBot.Dialogs
{
    public class DialogAndWelcomeBot : ActivityHandler
    {
        private BotState _userState;
        //public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
        //    : base(conversationState, userState, dialog, logger)
        //{
        //}
        public DialogAndWelcomeBot(UserState userState)
        {
            _userState = userState;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
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


            var card = File.ReadAllText(@".\Cards\WelcomeCard.json");
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

        [System.Obsolete]
        public async Task ShowSiteRequest(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            //var tokenResponse = turnContext.Activity.Value as TokenResponse;

            //List<Microsoft.Graph.User> users = new List<Microsoft.Graph.User>();
            //var list = new List<Tuple<string, string>>();
            //Dictionary<string, string> _teamsa = new Dictionary<string, string>();
            //users = await OAuthHelpers.ListMeAsync(turnContext, tokenResponse);
            //try
            //{
            //    foreach (Microsoft.Graph.User u in users)
            //    {
            //        string dname = u.DisplayName;
            //        string sname = u.Surname;
            //        try
            //        {

            //            if (dname == null)
            //            {
            //                dname = "Test";
            //            }
            //            if (sname == null)
            //            {
            //                sname = "Test";
            //            }
            //        }
            //        catch (Exception e) { }
            //        //_teamsa.Add((JsonConvert.DeserializeObject(u.DisplayName.ToString())), (JsonConvert.DeserializeObject<Microsoft.Graph.User>(u.Surname.ToString())));
            //        list.Add(Tuple.Create(dname, sname));
            //        _teamsa.Add(dname, sname);

            //    }
            //}
            //catch (Exception e)
            //{
            //    e.ToString();
            //}
            List<string> _teamsType = new List<string>
                    {
                        { "Public" },
                        {"Private" }
                    };
            List<string> _teamsClassification = new List<string>
                    {
                        { "Internal" },
                        {"External" },
                        { "Business" },
                        {"Protected" },
                        { "Important" },
                        {"Personal" }
                    };

            //var choicesTeamOwners = _teamsa.Select(s => new AdaptiveChoice { Title = s.Key, Value = s.Key }).ToList();
            var choicesType = _teamsType.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            var choicesClassification = _teamsClassification.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            try
            {
                var card = new AdaptiveCard
                {
                    Version = new AdaptiveSchemaVersion(1, 0),
                    Body =
                        {
                            new AdaptiveTextBlock("Team Name"),
                            new AdaptiveTextInput
                            {
                                Id = "Teamname",
                            },
                            new AdaptiveTextBlock("Description"),
                            new AdaptiveTextInput
                            {
                                Id = "Description",
                            },
                            new AdaptiveTextBlock("Team Owners"),
                            new AdaptiveChoiceSetInput
                            {
                                Choices = choicesType,
                                Id = "TeamOwners",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false
                            },
                            new AdaptiveTextBlock("Type"),
                            new AdaptiveChoiceSetInput
                            {
                                Choices = choicesType,
                                Id = "Type",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false
                            },
                                new AdaptiveTextBlock("Classification"),
                            new AdaptiveChoiceSetInput
                            {
                                Choices = choicesClassification,
                                Id = "Classification",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false
                            }
                        },
                    Actions = new List<AdaptiveAction>
                        {
                            new AdaptiveSubmitAction
                            {
                                Title = "Create Team",
                                Type = "Action.Submit"
                            }
                        }
                };
                //var content = 
                //var parsedResult = AdaptiveCard.FromJson(content);
                var attachment = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(card))
                };

                var activity = turnContext.Activity.CreateReply();
                activity.Attachments.Add(attachment);
                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
            catch (Exception e)

            { 
                e.ToString(); 
            }

            //var card = File.ReadAllText(@".\Cards\SiteCard.json");
            //var parsedResult = AdaptiveCard.FromJson(card);
            //var attachment = new Attachment
            //{
            //    ContentType = AdaptiveCard.ContentType,
            //    Content = parsedResult.Card
            //};

            //var activity = turnContext.Activity.CreateReply();
            //activity.Attachments.Add(attachment);

            //await turnContext.SendActivityAsync(activity, cancellationToken);
        }
        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {


            var card = new HeroCard();
            card.Title = "Welcome to SiteRequest Bot!";
            card.Text = @"Welcome to SiteRequest Bot.";
            card.Images = new List<CardImage>() { new CardImage(@"C:\Users\MangeshNikam\source\repos\SiteRequest\SiteRequest\Images\botsImage.jpg") };
            card.Buttons = new List<CardAction>()
            {
                new CardAction(ActionTypes.ImBack, "Create Team by test", null, "Create Team", "Create Team", "create Team"),
                new CardAction(ActionTypes.OpenUrl, "Ask a question", null, "Ask a question", "Ask a question", "https://stackoverflow.com/questions/tagged/botframework"),
          };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
        private static async Task ValidateInput(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Validated all input,Thanks for site request");

            await turnContext.SendActivityAsync(reply, cancellationToken);


        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if ((turnContext.Activity.Text != null) || (turnContext.Activity.Value != null))
            {
                string Input;
                if (turnContext.Activity.Text != null)
                {
                    Input = turnContext.Activity.Text;
                }
                else
                {
                    Input = turnContext.Activity.Value.ToString();
                }


                if (Input.ToLower().Contains("site") || Input.ToLower().Contains("site"))
                {
                    await ShowSiteRequest(turnContext, cancellationToken);

                }
                else if (Input.ToLower().Contains("team") || Input.ToLower().Contains("team"))
                {
                    await ValidateInput(turnContext, cancellationToken);

                }
                else
                {
                    var reply = MessageFactory.Text($"Welcome to SiteRequest Bot." + "Type anything to get started.");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }

        }


    }
}
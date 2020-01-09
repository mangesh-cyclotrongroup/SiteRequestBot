using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SiteRequest.Model;
using SiteRequest.Services;
using AdaptiveCards;
using System.IO;
using Newtonsoft.Json;
using TeamsHub.Web.Helpers;
using SiteRequest.Helpers;

namespace SiteRequest.Dialogs
{
    public class SiteRequestDialog : ComponentDialog
    {
        #region Variable
        private readonly BotServiceState _botServiceState;
        private readonly UserState _userState;

        #endregion


        public SiteRequestDialog(UserState userState) : base(nameof(SiteRequestDialog))
        {

            _userState = userState;
            // _botServiceState = botServiceState ?? throw new System.ArgumentNullException(nameof(botServiceState));
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
                 PromptStepAsync,
                LoginStepAsync,
                CommandStepAsync,
                //ProcessStepAsync,
                 showAdaptiveCardAsync,
                BugStepAsync

            };

            AddDialog(new WaterfallDialog($"{nameof(SiteRequestDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(SiteRequestDialog)}.CreatewelcomeCard"));
            AddDialog(new TextPrompt($"{nameof(SiteRequestDialog)}.ShowCard", AdaptorCardValidatorAsync));
            AddDialog(new TextPrompt($"{nameof(SiteRequestDialog)}.description"));

            InitialDialogId = $"{nameof(SiteRequestDialog)}.mainFlow";
        }


        private async Task<DialogTurnResult> CreatewelcomeCardasync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageToForward = stepContext.Context.Activity;


            if (stepContext.Result != null)
            {
                //var card = new AdaptiveCard();
                try
                {
                    var json = File.ReadAllText(@"C:\Users\MangeshNikam\source\repos\SiteRequest\SiteRequest\wwwroot\SiteCard.json");
                    AdaptiveCardParseResult result = AdaptiveCard.FromJson(json);
                    AdaptiveCard card = result.Card;

                    IList<AdaptiveWarning> warnings = result.Warnings;
                    Attachment attachment = new Attachment()
                    {

                        ContentType = AdaptiveCard.ContentType,
                        Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(card)),

                    };


                    //return await stepContext.PromptAsync(nameof(TextPrompt), opts);
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Attachments = new List<Attachment> { attachment }
                        },
                    };

                    await stepContext.Context.SendActivityAsync(opts.Prompt);
                    //        //opts.Prompt = new Activity(type: ActivityTypes.Typing);
                    //        //return await stepContext.PromptAsync("AdaptorCardValidatorAsync", opts);


                    return await stepContext.PromptAsync($"{nameof(SiteRequestDialog)}.CreatewelcomeCard",
                    new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Attachments = new List<Attachment> { attachment }
                        },
                    }, cancellationToken);
                }
                catch (AdaptiveSerializationException ex)
                {

                }
            }
            return await stepContext.NextAsync();
        }


        private Task<bool> AdaptorCardValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Context.Activity.Value != null)
            {
                //string JSONString = string.Empty;
                //using (var sqLiteDatabase = new SqLiteDatabase())
                //{
                //    sqLiteDatabase.OpenConnection();
                //    var result = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision Where Name=");
                //    string query = string.Empty;
                //    if (result.Rows.Count > 0)
                //    {
                //        query = $"UPDATE SiteProvision SET ";
                //    }
                //    else
                //    {
                //        //query = $"INSERT INTO SiteProvision ('Name','Description', 'Alias', 'SiteType', 'Language','RequestedBy','Owners','Status') VALUES('{name}','{description}','{alias}','{siteType}','{language}','{requestedBy}','{owners}','Requested')";
                //    }
                //    sqLiteDatabase.ExecuteNonQuery(query);
                //    sqLiteDatabase.CloseConnection();
                //}
            }
            return Task.FromResult(true);
        }

        #region Waterfalldialog
        private async Task<DialogTurnResult> CreateTeamChoiceasync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync($"{nameof(SiteRequestDialog)}.CreateTeamChoice",
             new PromptOptions
             {
                 Prompt = MessageFactory.Text("Do you want to create new team?"),
                 Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
             }, cancellationToken);
        }
        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //When user type create site request
            if (stepContext.Context.Activity.Text != null)
            {
                if (stepContext.Context.Activity.Text.ToString().ToLower().Contains("site"))
                {

                    return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
                }
                else

                {
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
            }
            else
            {
                //When user select create site request from card
                var InputMessage = stepContext.Context.Activity.Value;
                //string InputMessage = "test";
                if (InputMessage.ToString().ToLower().Contains("site"))
                {

                    return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
                }
                else

                {
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
            }
        }

        private async Task<DialogTurnResult> LoginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the token from the previous step. Note that we could also have gotten the
            // token directly from the prompt itself. There is an example of this in the next method.
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You are now logged in."), cancellationToken);
                return await stepContext.NextAsync();

                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to do? (type 'me', 'send <EMAIL>' or 'recent')") }, cancellationToken);
            }

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Login was not successful please try again."), cancellationToken);
            return await stepContext.NextAsync();
            //return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> CommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //stepContext.Values["command"] = "me";

            return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result != null)
            {
                // The token will be available in the Result property of the task.
                var tokenResponse = stepContext.Result as TokenResponse;

                // If we have the token use the user is authenticated so we may use it to make API calls.
                if (tokenResponse?.Token != null)
                {
                    var parts = ((string)stepContext.Values["command"] ?? string.Empty).ToLowerInvariant().Split(' ');

                    var command = parts[0];

                    if (command == "me")
                    {
                        await OAuthHelpers.ListMeAsync(stepContext.Context, tokenResponse);
                        //return await stepContext.NextAsync();
                    }

                    else
                    {
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your token is: {tokenResponse.Token}"), cancellationToken);
                        return await stepContext.NextAsync();
                    }
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("We couldn't log you in. Please try again later."), cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        [Obsolete]
        private async Task<DialogTurnResult> showAdaptiveCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result != null)
            {
                // The token will be available in the Result property of the task.
                var tokenResponse = stepContext.Result as TokenResponse;

                List<Microsoft.Graph.User> users = new List<Microsoft.Graph.User>();
                var list = new List<Tuple<string, string>>();
                Dictionary<string, string> _teamsa = new Dictionary<string, string>();
                users = await OAuthHelpers.ListMeAsync(stepContext.Context, tokenResponse);
                try
                {
                    foreach (Microsoft.Graph.User u in users)
                    {
                        string dname = u.DisplayName;
                        string sname = u.Surname;
                        if (dname == null)
                        {
                            dname = "Test";
                        }
                        if (sname == null)
                        {
                            sname = "Test";
                        }
                        //_teamsa.Add((JsonConvert.DeserializeObject(u.DisplayName.ToString())), (JsonConvert.DeserializeObject<Microsoft.Graph.User>(u.Surname.ToString())));
                        list.Add(Tuple.Create(dname, sname));
                        _teamsa.Add(dname, sname);

                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
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

                var choicesTeamOwners = _teamsa.Select(s => new AdaptiveChoice { Title = s.Key, Value = s.Key }).ToList();
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
                                Choices = choicesTeamOwners,
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

                    return await stepContext.PromptAsync($"{ nameof(SiteRequestDialog)}.ShowCard",
     new PromptOptions
     {
         Choices = ChoiceFactory.ToChoices(_teamsa.Select(pair => pair.Value).ToList()),
         Prompt = (Activity)MessageFactory.Attachment(new Attachment
         {
             ContentType = AdaptiveCard.ContentType,
             Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(card))
         })
     },
     cancellationToken);
                }
                catch (Exception e)
                { e.ToString(); }
            }
            else
            {

            }
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["PhoneNumber"] = (string)stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(SiteRequestDialog)}.bug",
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Please Select the branch that you want to take admission"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Computer", "Mech", "Science", "Automobile", "Arts", "commerce" }),
            }, cancellationToken);
        }
        #endregion

    }
}

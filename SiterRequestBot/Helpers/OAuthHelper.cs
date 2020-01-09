using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Graph;
using Attachment = Microsoft.Bot.Schema.Attachment;
namespace SiteRequest.Helpers
{
    // Copyright (c) Microsoft Corporation. All rights reserved.
    // Licensed under the MIT License.

    // https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
    public static class OAuthHelpers
    {       
       
        // Displays information about the user in the bot.
        public static async Task<List<User>> ListMeAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            List<User> users = new List<User>();
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }

            // Pull in the data from the Microsoft Graph.
            var client = new SimpleGraphClient(tokenResponse.Token);
             users = await client.GetMeAsync();
            return users;
        }

       
      
    }
}



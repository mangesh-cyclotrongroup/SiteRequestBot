
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using SiteRequest.Model;

namespace SiteRequest.Helpers
{
    // This class is a wrapper for the Microsoft Graph API
    // See: https://developer.microsoft.com/en-us/graph
    public class SimpleGraphClient
    {
        private readonly string _token;

        public SimpleGraphClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            _token = token;
        }

       
        // Gets mail for the user using the Microsoft Graph API
        public async Task<Message[]> GetRecentMailAsync()
        {
            var graphClient = GetAuthenticatedClient();
            var messages = await graphClient.Me.MailFolders.Inbox.Messages.Request().GetAsync();
            return messages.Take(5).ToArray();
        }

    
        public async Task<List<User>> GetMeAsync()
        {
            var graphClient = GetAuthenticatedClient();
            var me = await graphClient.Users.Request().GetAsync();
            
            //var me = await graphClient.Me.Request().GetAsync();
            List<User> users = new List<User>();
            foreach (User u in me)
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
                users.Add(new User
                {
                    DisplayName = dname,
                    Surname=sname
                });

            }

            return users;
        }

        // gets information about the user's manager.
        public async Task<User> GetManagerAsync()
        {
            var graphClient = GetAuthenticatedClient();
            var manager = await graphClient.Me.Manager.Request().GetAsync() as User;
            return manager;
        }

        // Gets the user's photo
        public async Task<PhotoResponse> GetPhotoAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            using (var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Graph returned an invalid success code: {response.StatusCode}");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                var photoResponse = new PhotoResponse
                {
                    Bytes = bytes,
                    ContentType = response.Content.Headers.ContentType?.ToString(),
                };

                if (photoResponse != null)
                {
                    photoResponse.Base64String = $"data:{photoResponse.ContentType};base64," +
                                                 Convert.ToBase64String(photoResponse.Bytes);
                }

                return photoResponse;
            }
        }

        // Get an Authenticated Microsoft Graph client using the token issued to the user.
        private GraphServiceClient GetAuthenticatedClient()
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                        // Get event times in the current time zone.
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");

                        return Task.CompletedTask;
                    }));
            return graphClient;
        }


    }
}


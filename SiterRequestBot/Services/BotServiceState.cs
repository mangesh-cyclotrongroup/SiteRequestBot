using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteRequest.Model;

namespace SiteRequest.Services
{
    public class BotServiceState
    {
        public ConversationState ConversationState { get; }
        #region Variables

        public UserState UserState { get; }


        public static string UserProfileId { get; } = $"{nameof(BotServiceState)}.UserProfile";
        public static string ConversationDataId { get; } = $"{nameof(BotServiceState)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotServiceState)}.DialogState";


        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        #endregion

        public BotServiceState(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            initializeAccessors();

        }
        public void initializeAccessors()
        {
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
        }
    }
}

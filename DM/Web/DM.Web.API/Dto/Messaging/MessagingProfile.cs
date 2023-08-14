using AutoMapper;
using DM.Services.Community.BusinessProcesses.Chat.Creating;
using DM.Services.Community.BusinessProcesses.Messaging.Creating;
using DtoConversation = DM.Services.Community.BusinessProcesses.Messaging.Reading.Conversation;
using DtoMessage = DM.Services.Community.BusinessProcesses.Messaging.Reading.Message;
using DtoChatMessage = DM.Services.Community.BusinessProcesses.Chat.Reading.ChatMessage;

namespace DM.Web.API.Dto.Messaging;

/// <inheritdoc />
internal class MessagingProfile : Profile
{
    /// <inheritdoc />
    public MessagingProfile()
    {
        CreateMap<DtoConversation, Conversation>()
            .ForMember(d => d.Name, s => s.Ignore());

        CreateMap<DtoMessage, Message>()
            .ForMember(d => d.Created, s => s.Ignore());

        CreateMap<Message, CreateMessage>()
            .ForMember(d => d.ConversationId, s => s.Ignore());

        CreateMap<DtoChatMessage, ChatMessage>()
            .ForMember(d => d.Created, s => s.Ignore());

        CreateMap<ChatMessage, CreateChatMessage>();
    }
}

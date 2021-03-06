﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Api.Aggregator;
using Telegram.Api.Services;
using Telegram.Api.Services.Cache;
using Telegram.Api.TL;
using Unigram.Collections;
using Unigram.Common;
using Windows.UI.Xaml.Navigation;
using Template10.Common;
using Telegram.Api.Helpers;
using Unigram.Core.Services;
using Telegram.Api.Services.Updates;
using Telegram.Api.Transport;
using Telegram.Api.Services.Connection;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using Windows.UI.Xaml;
using Unigram.Converters;
using Windows.UI.Xaml.Media;

namespace Unigram.ViewModels
{
    public partial class DialogViewModel : UnigramViewModelBase
    {
        int ChatType=-1;
        //0 if private, 1 if group, 2 if supergroup/channel
        int loadCount =15;
        int loaded = 0;
        public TLPeerBase peer;
        public TLInputPeerBase inputPeer;
        public MessageCollection Messages { get; private set; } = new MessageCollection();
        public Brush PlaceHolderColor { get; internal set; }
        public string DialogTitle;
        public string LastSeen;
        public string pinnedMessage;
        public string pinnedMessageSender;
        public Visibility pinnedMessageVisible = Visibility.Collapsed;
        public Visibility LastSeenVisible;
        public string debug;













        private readonly IJumpListService _jumpListService;

        public DialogViewModel(IMTProtoService protoService, ICacheService cacheService, ITelegramEventAggregator aggregator, IJumpListService jumpListService)
            : base(protoService, cacheService, aggregator)
        {
            _jumpListService = jumpListService;
        }












        public object photo;
        public string SendTextHolder;
        public TLUser user;
        private TLUserBase _item;
        public TLUserBase Item
        {
            get
            {                
                return _item;
            }
            set
            {
                Set(ref _item, value);
            }
        }
        public TLUserBase Self
        {
            get
            {
                return _item;
            }
            set
            {
                Set(ref _item, value);
            }
        }
        public TLInputPeerChannel channel;

        public TLInputPeerChat chat;
        private TLPeerBase _chatItem;
        public TLPeerBase peerItem
        {
            get
            {
                return _chatItem;
            }
            set
            {
                Set(ref _chatItem, value);
            }
        }

        private TLInputPeerBase _peerBase;
        public TLInputPeerBase selfPeerBase
        {
            get
            {
                return _peerBase;
            }
            set
            {
                Set(ref _peerBase, value);
            }
        }
        private TLInputPeerBase _peer;
        public TLInputPeerBase Peer
        {
            get
            {
                return _peer;
            }
            set
            {
                Set(ref _peer, value);
            }
        }
        public async Task FetchMessages(TLPeerBase peer, TLInputPeerBase inputPeer)
        {
            var result = await ProtoService.GetHistoryAsync(inputPeer, peer, false, loaded, int.MaxValue, loadCount);
            if (result.IsSucceeded)
            {
                ProcessReplies(result.Value.Messages);
                foreach (var item in result.Value.Messages)
                {
                    
                    Messages.Insert(0, item);
                }
            }

            loaded += loadCount;
        }

        public async void ProcessReplies(IList<TLMessageBase> messages)
        {
            var replyIds = new TLVector<int>();
            var replyToMsgs = new List<TLMessage>();

            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i] as TLMessage;
                if (message != null)
                {
                    var replyId = message.ReplyToMsgId;
                    if (replyId != null && replyId.Value != 0)
                    {
                        var channelId = new int?();
                        var channel = message.ToId as TLPeerChat;
                        if (channel != null)
                        {
                            channelId = channel.Id;
                        }

                        var reply = CacheService.GetMessage(replyId.Value, channelId);
                        if (reply != null)
                        {
                            messages[i].Reply = reply;
                        }
                        else
                        {
                            replyIds.Add(replyId.Value);
                            replyToMsgs.Add(message);
                        }
                    }

                    //if (message.NotListened && message.Media != null)
                    //{
                    //    message.Media.NotListened = true;
                    //}
                }
            }

            if (replyIds.Count > 0)
            {
                Task<MTProtoResponse<TLMessagesMessagesBase>> task = null;

                if (Peer is TLInputPeerChannel)
                {
                    var first = replyToMsgs.FirstOrDefault();
                    if (first.ToId is TLPeerChat)
                    {
                        task = ProtoService.GetMessagesAsync(replyIds);
                    }
                    else
                    {
                        var peer = Peer as TLInputPeerChannel;
                        task = ProtoService.GetMessagesAsync(new TLInputChannel { ChannelId = peer.ChannelId, AccessHash = peer.AccessHash }, replyIds);
                    }
                }
                else
                {
                    task = ProtoService.GetMessagesAsync(replyIds);
                }

                var result = await task;
                if (result.IsSucceeded)
                {
                    CacheService.AddChats(result.Value.Chats, (results) => { });
                    CacheService.AddUsers(result.Value.Users, (results) => { });

                    for (int j = 0; j < result.Value.Messages.Count; j++)
                    {
                        for (int k = 0; k < replyToMsgs.Count; k++)
                        {
                            var message = replyToMsgs[k];
                            if (message != null && message.ReplyToMsgId.Value == result.Value.Messages[j].Id)
                            {
                                replyToMsgs[k].Reply = result.Value.Messages[j];
                                replyToMsgs[k].RaisePropertyChanged(() => replyToMsgs[k].ReplyInfo);
                            }
                        }
                    }
                }
                else
                {
                    Execute.ShowDebugMessage("messages.getMessages error " + result.Error);
                }
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            loaded = 0;
            channel = parameter as TLInputPeerChannel;
            chat = parameter as TLInputPeerChat;
            user = parameter as TLUser;
            if (user != null)
            {
         
                //Happy Birthday Alexmitter xD
                Messages.Clear();
                Item = user;
                photo = user.Photo;
                DialogTitle = Item.FullName;
                PlaceHolderColor = BindConvert.Current.Bubble(Item.Id);
                LastSeen = LastSeenHelper.GetLastSeen(user).Item1;
                LastSeenVisible = Visibility.Visible;
                peer = new TLPeerUser { Id = SettingsHelper.UserId };
                inputPeer = new TLInputPeerUser { UserId = user.Id, AccessHash = user.AccessHash ?? 0 };
                Peer = new TLInputPeerUser { UserId = user.Id, AccessHash = user.AccessHash ?? 0 };
                peerItem = new TLPeerUser { Id = user.Id };
                await FetchMessages(peer,inputPeer);
                ChatType = 0;

                await _jumpListService.UpdateAsync(user);
            }
            else if (channel != null)
            {

                TLInputChannel x=new TLInputChannel();                
                x.ChannelId = channel.ChannelId;
                x.AccessHash = channel.AccessHash;
                var channelDetails = await ProtoService.GetFullChannelAsync(x);                
                DialogTitle = channelDetails.Value.Chats[0].FullName;
                var channelFull=(TLChannelFull)channelDetails.Value.FullChat;
                if (channelFull.HasPinnedMsgId)
                {
                    pinnedMessageVisible = Visibility.Visible;
                    var msgId = channelFull.PinnedMsgId;
                    TLVector<int> temp = new TLVector<int>(1) { msgId.GetValueOrDefault() };
                    var y = await ProtoService.GetMessagesAsync(x, temp);
                    pinnedMessage = ((TLMessage)y.Value.Messages[0]).Message;
                    pinnedMessageSender = y.Value.Users[0].FullName;
                }
                else
                {
                    pinnedMessageVisible = Visibility.Collapsed;
                }
                PlaceHolderColor = BindConvert.Current.Bubble(channelDetails.Value.Chats[0].Id);
                photo = channelDetails.Value.Chats[0].Photo;
                LastSeenVisible = Visibility.Collapsed;
                peer = new TLPeerUser { Id = SettingsHelper.UserId };
                inputPeer = new TLInputPeerChannel { ChannelId = x.ChannelId, AccessHash = x.AccessHash };
                Peer = new TLInputPeerChannel { ChannelId = x.ChannelId, AccessHash = x.AccessHash };
                await FetchMessages(peer, inputPeer);
                peerItem = new TLPeerChannel { Id = channel.ChannelId };
                ChatType = 2;
            }
            else if (chat != null)
            {
                var chatDetails = await ProtoService.GetFullChatAsync(chat.ChatId);
                DialogTitle = chatDetails.Value.Chats[0].FullName;
                photo =chatDetails.Value.Chats[0].Photo;
                PlaceHolderColor = BindConvert.Current.Bubble(chatDetails.Value.Chats[0].Id);
                LastSeenVisible = Visibility.Collapsed;
                peer = new TLPeerUser { Id = SettingsHelper.UserId };
                inputPeer = new TLInputPeerChat { ChatId = chat.ChatId, AccessHash = chat.AccessHash };
                Peer = new TLInputPeerChat { ChatId = chat.ChatId, AccessHash = chat.AccessHash };
                await FetchMessages(peer, inputPeer);
                peerItem = new TLPeerChat { Id = chat.ChatId };
                ChatType = 1;
            }

            var dialog = CacheService.GetDialog(peerItem);
            if (dialog != null)
            {
                if (dialog.HasDraft) {
                    var draft = dialog.Draft as TLDraftMessage;
                    if (draft != null) {
                        ProcessDraftReply(draft);
                    }
                }
                int unread = dialog.UnreadCount;
                if (inputPeer is TLInputPeerChannel) {
                    var asChannel = new TLChannel { Id = (inputPeer as TLInputPeerChannel).ChannelId, AccessHash = inputPeer.AccessHash };
                    await ProtoService.ReadHistoryAsync(asChannel, Messages.Last().Id);
                } else {
                    await ProtoService.ReadHistoryAsync(inputPeer, Messages.Last().Id, 0);
                }
                dialog.UnreadCount = dialog.UnreadCount - unread; // in case any new messages come in during that split second
                dialog.RaisePropertyChanged("UnreadCount"); // otherwise it won't update on the main page
                
            }
        }

        public async void ProcessDraftReply(TLDraftMessage draft)
        {
            var shouldFetch = false;

            var replyId = draft.ReplyToMsgId;
            if (replyId != null && replyId.Value != 0)
            {
                var channelId = new int?();
                //var channel = message.ToId as TLPeerChat;
                //if (channel != null)
                //{
                //    channelId = channel.Id;
                //}
                // TODO: verify
                if (Peer is TLInputPeerChannel)
                {
                    channelId = peer.Id;
                }

                var reply = CacheService.GetMessage(replyId.Value, channelId);
                if (reply != null)
                {
                    Reply = reply;
                }
                else
                {
                    shouldFetch = true;
                }
            }

            if (shouldFetch)
            {
                Task<MTProtoResponse<TLMessagesMessagesBase>> task = null;

                if (Peer is TLInputPeerChannel)
                {
                    // TODO: verify
                    //var first = replyToMsgs.FirstOrDefault();
                    //if (first.ToId is TLPeerChat)
                    //{
                    //    task = ProtoService.GetMessagesAsync(new TLVector<int> { draft.ReplyToMsgId.Value });
                    //}
                    //else
                    {
                        var peer = Peer as TLInputPeerChannel;
                        task = ProtoService.GetMessagesAsync(new TLInputChannel { ChannelId = peer.ChannelId, AccessHash = peer.AccessHash }, new TLVector<int> { draft.ReplyToMsgId.Value });
                    }
                }
                else
                {
                    task = ProtoService.GetMessagesAsync(new TLVector<int> { draft.ReplyToMsgId.Value });
                }

                var result = await task;
                if (result.IsSucceeded)
                {
                    CacheService.AddChats(result.Value.Chats, (results) => { });
                    CacheService.AddUsers(result.Value.Users, (results) => { });

                    for (int j = 0; j < result.Value.Messages.Count; j++)
                    {
                        if (draft.ReplyToMsgId.Value == result.Value.Messages[j].Id)
                        {
                            Reply = result.Value.Messages[j];
                        }
                    }
                }
                else
                {
                    Execute.ShowDebugMessage("messages.getMessages error " + result.Error);
                }
            }
        }


        #region Reply 

        private TLMessageBase _reply;
        public TLMessageBase Reply
        {
            get
            {
                return _reply;
            }
            set
            {
                if (_reply != value)
                {
                    _reply = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(() => ReplyInfo);
                }
            }
        }

        public ReplyInfo ReplyInfo
        {
            get
            {
                if (_reply != null)
                {
                    return new ReplyInfo
                    {
                        Reply = _reply,
                        ReplyToMsgId = _reply.Id
                    };
                }

                return null;
            }
        }

        public RelayCommand ClearReplyCommand => new RelayCommand(() => { Reply = null; });

        #endregion

        public RelayCommand<string> SendCommand => new RelayCommand<string>(SendMessage);
        private async void SendMessage(string args)
        {
            await SendMessageAsync(null, args != null);
        }

        public async Task SendMessageAsync(List<TLMessageEntityBase> entities, bool sticker)
        {
            var messageText = SendTextHolder;

            TLPeerBase toId = null;
            TLInputPeerBase toPeer = null;
            int replyToId = 0;

            switch (ChatType)
            {
                case 0:
                    toId = new TLPeerUser { Id = int.Parse(Item.Id.ToString()) };
                    toPeer = new TLInputPeerUser { UserId = Item.Id, AccessHash = ((TLUser)Item).AccessHash ?? 0 };
                    break;
                case 1:
                    toId = new TLPeerChat { Id = int.Parse(peerItem.Id.ToString()) };
                    toPeer = new TLInputPeerChat { ChatId = peerItem.Id };
                    break;
                case 2:
                    toId = new TLPeerChannel { Id = int.Parse(peerItem.Id.ToString()) };
                    toPeer = new TLInputPeerChannel { ChannelId = peerItem.Id };
                    break;
            }

            TLDocument document = null;
            TLMessageMediaBase media = null;
            if (sticker)
            {
                messageText = string.Empty;

                var set = await ProtoService.GetStickerSetAsync(new TLInputStickerSetShortName { ShortName = "devsfrog" });
                if (set.IsSucceeded)
                {
                    document = set.Value.Documents.FirstOrDefault(x => x.Id == 325680287654608971) as TLDocument;
                }
            }

            if (document != null)
            {
                media = new TLMessageMediaDocument { Document = document };
            }
            else
            {
                media = new TLMessageMediaEmpty();
            }

            var date = TLUtils.DateToUniversalTimeTLInt(ProtoService.ClientTicksDelta, DateTime.Now);
            var message = TLUtils.GetMessage(SettingsHelper.UserId, toId, TLMessageState.Sending, true, true, date, messageText, media, TLLong.Random(), null);

            message.Entities = entities != null ? new TLVector<TLMessageEntityBase>(entities) : null;
            message.HasEntities = entities != null;

            if (Reply != null)
            {
                message.HasReplyToMsgId = true;
                message.ReplyToMsgId = Reply.Id;
                message.Reply = Reply;
                Reply = null;
            }

            var previousMessage = InsertSendingMessage(message);

            CacheService.SyncSendingMessage(message, previousMessage, async (m) =>
            {
                if (document != null)
                {
                    var input = new TLInputMediaDocument
                    {
                        Id = new TLInputDocument
                        {
                            Id = document.Id,
                            AccessHash = document.AccessHash
                        }
                    };
                    await ProtoService.SendMediaAsync(Peer, input, message);
                }
                else
                {
                    await ProtoService.SendMessageAsync(message, () =>
                    {
                        message.State = TLMessageState.Confirmed;
                    });
                }
            });
        }

        private TLMessageBase InsertSendingMessage(TLMessage message, bool useReplyMarkup = false)
        {
            TLMessageBase result;
            if (Messages.Count > 0)
            {
                //if (useReplyMarkup && _replyMarkupMessage != null)
                //{
                //    var chat = With as TLChatBase;
                //    if (chat != null)
                //    {
                //        message.ReplyToMsgId = _replyMarkupMessage.Id;
                //        message.Reply = _replyMarkupMessage;
                //    }

                //    Execute.BeginOnUIThread(() =>
                //    {
                //        if (Reply != null)
                //        {
                //            Reply = null;
                //            SetReplyMarkup(null);
                //        }
                //    });
                //}

                var messagesContainer = Reply as TLMessagesContainter;
                if (Reply != null)
                {
                    if (Reply.Id != 0)
                    {
                        message.ReplyToMsgId = Reply.Id;
                        message.Reply = Reply;
                    }
                    else if (messagesContainer != null && !string.IsNullOrEmpty(message.Message.ToString()))
                    {
                        message.Reply = Reply;
                    }

                    var replyMessage = Reply as TLMessage;
                    if (replyMessage != null)
                    {
                        //var replyMarkup = replyMessage.ReplyMarkup;
                        //if (replyMarkup != null)
                        //{
                        //    replyMarkup.HasResponse = true;
                        //}
                    }

                    Execute.BeginOnUIThread(delegate
                    {
                        Reply = null;
                    });
                }

                result = Messages.LastOrDefault();
                Messages.Add(message);

                if (messagesContainer != null && !string.IsNullOrEmpty(message.Message.ToString()))
                {
                    foreach (var fwdMessage in messagesContainer.FwdMessages)
                    {
                        Messages.Insert(0, fwdMessage);
                    }
                }

                //for (int i = 1; i < Messages.Count; i++)
                //{
                //    var serviceMessage = Messages[i] as TLMessageService;
                //    if (serviceMessage != null)
                //    {
                //        var unreadAction = serviceMessage.Action as TLMessageActionUnreadMessages;
                //        if (unreadAction != null)
                //        {
                //            Messages.RemoveAt(i);
                //            break;
                //        }
                //    }
                //}
            }
            else
            {
                var messagesContainer = Reply as TLMessagesContainter;
                if (Reply != null)
                {
                    if (Reply.Id != 0)
                    {
                        message.HasReplyToMsgId = true;
                        message.ReplyToMsgId = Reply.Id;
                        message.Reply = Reply;
                    }
                    else if (messagesContainer != null && !string.IsNullOrEmpty(message.Message.ToString()))
                    {
                        message.Reply = Reply;
                    }
                    Reply = null;
                }

                Messages.Clear();
                Messages.Add(message);

                var history = CacheService.GetHistory(TLUtils.InputPeerToPeer(Peer, ProtoService.CurrentUserId), 15);
                result = history.FirstOrDefault();

                for (int j = 0; j < history.Count; j++)
                {
                    Messages.Add(history[j]);
                }

                //if (messagesContainer != null && !string.IsNullOrEmpty(message.Message.ToString()))
                //{
                //    foreach (var fwdMessage in messagesContainer.FwdMessages)
                //    {
                //        Messages.Insert(0, fwdMessage);
                //    }
                //}

                //for (int k = 1; k < Messages.Count; k++)
                //{
                //    var serviceMessage = Messages[k] as TLMessageService;
                //    if (serviceMessage != null)
                //    {
                //        var unreadAction = serviceMessage.Action as TLMessageActionUnreadMessages;
                //        if (unreadAction != null)
                //        {
                //            Messages.RemoveAt(k);
                //            break;
                //        }
                //    }
                //}
            }
            return result;
        }
    }

    public class MessageCollection : ObservableCollection<TLMessageBase>
    {
        public ObservableCollection<MessageGroup> Groups { get; private set; } = new ObservableCollection<MessageGroup>();

        protected override void InsertItem(int index, TLMessageBase item)
        {
            base.InsertItem(index, item);

            var group = GroupForIndex(index);
            if (group == null || group?.FromId != item.FromId)
            {
                group = new MessageGroup(this, item.From, item.FromId, item.ToId, item is TLMessage ? ((TLMessage)item).IsOut : false);
                Groups.Insert(index == 0 ? 0 : Groups.Count, group); // TODO: should not be 0 all the time
            }

            group.Insert(index - group.FirstIndex, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            var group = GroupForIndex(index);
            if (group != null)
            {
                group.RemoveAt(index - group.FirstIndex);
            }
        }

        private MessageGroup GroupForIndex(int index)
        {
            if (index == 0)
            {
                return Groups.FirstOrDefault();
            }
            else if (index == Count - 1)
            {
                return Groups.LastOrDefault();
            }
            else
            {
                return Groups.FirstOrDefault(x => x.FirstIndex >= index && x.LastIndex <= index);
            }
        }
    }

    public class MessageGroup : ObservableCollection<TLMessageBase>
    {
        private ObservableCollection<MessageGroup> _parent;

        public MessageGroup(MessageCollection parent, TLUser from, int? fromId, TLPeerBase toId, bool isOut)
        {
            _parent = parent.Groups;

            From = from;
            if (fromId == null)
                FromId = 33303409;
            FromId = fromId;
            ToId = toId;
            IsOut = isOut;
        }

        public TLUser From { get; private set; }

        public int? FromId { get; private set; }

        public TLPeerBase ToId { get; private set; }

        public bool IsOut { get; private set; }

        public int FirstIndex
        {
            get
            {
                var count = 0;
                var index = _parent.IndexOf(this);
                if (index > 0)
                {
                    count = _parent[index - 1].LastIndex + 1;
                }

                return count;
            }
        }

        public int LastIndex
        {
            get
            {
                return FirstIndex + Math.Max(0, Count - 1);
            }
        }

        protected override void InsertItem(int index, TLMessageBase item)
        {
            // TODO: experimental
            if (index == 0)
            {
                if (Count > 0)
                    this[0].IsFirst = false;

                item.IsFirst = true;
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}

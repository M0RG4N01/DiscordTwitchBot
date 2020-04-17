using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses;

namespace TwitchBot
{
    public class TwitchEvents 
    {
        public TwitchClient client { get; set; }
        public TwitchPubSub pubsub = new TwitchPubSub();
        public ChannelMetadata channelMetadata { get; set; }
        public LiveStreamMonitorService livemonitor;

        public TwitchAPI TwitchApi = new TwitchAPI(settings: new ApiSettings
        {
            ClientId = "**Removed**",
            AccessToken = "**Removed**"
        });

        public TwitchEvents(ConnectionCredentials credentials, string username)
        {
            try
            {
                Dictionary<string, string> channels = new Dictionary<string, string>()
                {
                    {"phenomalytv", "PaymoneyWubby"}
                };

                livemonitor = new LiveStreamMonitorService(TwitchApi, 20);
                livemonitor.SetChannelsByName(channels.Keys.ToList());
                livemonitor.Start();

                livemonitor.OnStreamOnline += OnStreamOnline;
                livemonitor.OnStreamOffline += OnStreamOffline;
            } catch(Exception ex) { Console.WriteLine(ex);}

            try
            {
                client = new TwitchClient { AutoReListenOnException = true };
                client.Initialize(credentials, username);

                client.Connect();

                client.OnConnected += OnConnected;
                client.OnReconnected += OnReconnected;
                client.OnDisconnected += OnDisconnected;
                

                client.OnNewSubscriber += OnNewSubscriber;
                client.OnMessageReceived += OnMessageReceived;


                pubsub.ListenToBitsEvents("109598326");
                pubsub.ListenToFollows("109598326");
                pubsub.ListenToSubscriptions("109598326");
                pubsub.OnFollow += OnNewFollower;
                pubsub.OnBitsReceived += OnBitsReceived;
                

                pubsub.Connect();

                if (!client.IsConnected)
                {
                    throw new NotImplementedException("Error out no connection");
                }

            } catch(Exception ex) { Console.WriteLine(ex);}
        }

         public async void OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            Console.WriteLine("Reconnected to Twitch!");
        }

         public async void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected from Twitch!");
        }

         public async void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("Connected to Twitch!");
        }

         public async void OnStreamOffline(object sender, OnStreamOfflineArgs e)
         {

            var subs = WebCalls.GetRootObject();

            List<string> gameIds = new List<string> { e.Stream.GameId };
            var games = await TwitchApi.Helix.Games.GetGamesAsync(gameIds);

            var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");
            Console.WriteLine(e.Stream.ThumbnailUrl);

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = channel.DisplayName,
                    IconUrl = channel.Logo
                },
                Color = DiscordColor.Purple,
                Title = e.Channel + " is now offline! :(",
                Url = "https://www.twitch.tv/phenomalytv",
                ImageUrl = "https://static-cdn.jtvnw.net/previews-ttv/live_user_phenomalytv-320x180.jpg",
                ThumbnailUrl = channel.Logo,

            };
            embed.AddField("Stream Title: ", e.Stream.Title);
            embed.AddField("Viewers: ", e.Stream.ViewerCount.ToString(), true);
            embed.AddField("Subscriptions: ", (subs.data.Count - 1).ToString(), true);
            embed.AddField("Game: ", games.Games[0].Name, true);
            embed.AddField("Time started [GMT]: ", e.Stream.StartedAt.ToShortTimeString(), true);
            embed.AddField("Followers: ", channel.Followers.ToString(), true);
            embed.AddField("Rated Mature?: ", channel.Mature.ToString(), true);
            embed.AddField("Language: ", e.Stream.Language, true);
            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync(" " + e.Channel + " is now offline :(!");
            await chan.SendMessageAsync("\n", embed: embed);
        }

        public async void OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            var subs = WebCalls.GetRootObject();
            List<string> gameIds = new List<string> { e.Stream.GameId };
            var games = await TwitchApi.Helix.Games.GetGamesAsync(gameIds);

            var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");
            Console.WriteLine(e.Stream.ThumbnailUrl);
              
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = channel.DisplayName,
                    IconUrl = channel.Logo
                },
                Color = DiscordColor.Purple,
                Title = e.Channel + " is now live! :D",
                Url = "https://www.twitch.tv/phenomalytv",
                ImageUrl = "https://static-cdn.jtvnw.net/previews-ttv/live_user_phenomalytv-320x180.jpg",
                ThumbnailUrl = channel.Logo,           

            };
            embed.AddField("Stream Title: ", e.Stream.Title);
            embed.AddField("Viewers: ", e.Stream.ViewerCount.ToString(), true);
            embed.AddField("Subscriptions: ", (subs.data.Count - 1).ToString(), true);
            embed.AddField("Game: ", games.Games[0].Name, true);
            embed.AddField("Time started [GMT]: ", e.Stream.StartedAt.ToShortTimeString() , true);      
            embed.AddField("Followers: ", channel.Followers.ToString(), true);
            embed.AddField("Rated Mature?: ", channel.Mature.ToString(), true);
            embed.AddField("Language: ", e.Stream.Language, true);
            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync("Supppp " + "@everyone" + ", " + channel.DisplayName + " is now live! Come and watch here: <https://www.twitch.tv/phenomalytv> :)!!");
            await chan.SendMessageAsync("\n", embed: embed);
        }

        public static async void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine((string.Format("{0}: {1}", e.ChatMessage.Username, e.ChatMessage.Message)));
            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Purple,
                Title = "-LIVE CHAT-",
                Url = "https://www.twitch.tv/phenomalytv",
            };
            embed.AddField(e.ChatMessage.DisplayName,
                e.ChatMessage.Message + "\n | Subscribed: " + e.ChatMessage.IsSubscriber + ", Moderator: " + e.ChatMessage.IsModerator);
            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync("\n", embed: embed);
        }

        private static async void OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Purple,
                ImageUrl = "https://cdn.twitchalerts.com/twitch-bits/images/hd/100.gif",
                Title = "Bits! Bits! Bits!",
                Url = "https://www.twitch.tv/phenomalytv",
            };

            embed.AddField("PhenomalyTV HAS RECEIVED BITSSS!",$"{e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");

            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync("\n", embed: embed);

        }

        private static async void OnNewFollower(object sender, OnFollowArgs e)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Blue,
                Title = e.DisplayName + " has followed " + e.FollowedChannelId,
                Url = "https://www.twitch.tv/phenomalytv",
              
            };
            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync("\n", embed: embed);
        }

        private static async void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {

            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = e.Subscriber.DisplayName + " has subscribed to " + e.Channel + (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime ? " using Twitch Prime!" : ""),
                Url = "https://www.twitch.tv/phenomalytv",
            };

            var chan = await TwitchBotMain.discord.GetChannelAsync(552255972060692504);
            await chan.SendMessageAsync("\n", embed: embed);

        }
    }
}

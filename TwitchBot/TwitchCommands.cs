using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using SubscriptionPlan = TwitchLib.Api.Core.Enums.SubscriptionPlan;

namespace TwitchBot
{
    internal class TwitchCommands : BaseCommandModule
    {
        public TwitchAPI TwitchApi = new TwitchAPI(settings: new ApiSettings
        {
            ClientId = "**Removed**",
            AccessToken = "**Removed**"
        });

        [Command("invoke")]
        internal async Task InvoiceOnNowhosting(CommandContext ctx)
        {
            try
            {
                var subs = WebCalls.GetRootObject();
                Console.WriteLine(subs.data.Count);
                await ctx.RespondAsync("Subscribers: " + (subs.data.Count - 1));
                await ctx.RespondAsync("Invoked");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("bitsleaderboard"), Aliases("bitsleader", "bleader", "bitsl", "top10bits")]
        internal async Task BitsLeaderboard(CommandContext ctx)
        {
            var bits = await TwitchApi.Helix.Bits.GetBitsLeaderboardAsync();
            var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");
            var channel2 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[0].UserId);
            var channel3 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[1].UserId);
            var channel4 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[2].UserId);
            var channel5 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[3].UserId);
            var channel6 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[4].UserId);
            var channel7 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[5].UserId);
            var channel8 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[6].UserId);
            var channel9 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[7].UserId);
            var channel10 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[8].UserId);
            var channel11 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[9].UserId);

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = channel.DisplayName + " Top 10 bits leaderboard",
                    IconUrl = channel.Logo
                },
                Color = DiscordColor.Purple,
                Title = channel.DisplayName + "'s Top 10 Bits Leaderboard",
                Url = channel.Url,
                ThumbnailUrl = "https://cdn.twitchalerts.com/twitch-bits/images/hd/100.gif",

            };
            embed.AddField("1st Place: " + channel2.DisplayName, " Score: " + bits.Listings[0].Score, true);
            embed.AddField("2nd Place: " + channel3.DisplayName, " Score: " + bits.Listings[1].Score, true);
            embed.AddField("3rd Place: " + channel4.DisplayName, " Score: " + bits.Listings[2].Score, true);
            embed.AddField("4th Place: " + channel5.DisplayName, " Score: " + bits.Listings[3].Score, true);
            embed.AddField("5th Place: " + channel6.DisplayName, " Score: " + bits.Listings[4].Score, true);
            embed.AddField("6th Place: " + channel7.DisplayName, " Score: " + bits.Listings[5].Score, true);
            embed.AddField("7th Place: " + channel8.DisplayName, " Score: " + bits.Listings[6].Score, true);
            embed.AddField("8th Place: " + channel9.DisplayName, " Score: " + bits.Listings[7].Score, true);
            embed.AddField("9th Place: " + channel10.DisplayName, "Score: " + bits.Listings[8].Score, true);
            embed.AddField("10th Place: " + channel11.DisplayName, " Score: " + bits.Listings[9].Score, true);

            await ctx.RespondAsync("\n", embed: embed);
        }

        [Command("bitsleaderboard2"), Aliases("bitsleader2", "bleader2", "bitsl2", "top3bits")]
        internal async Task BitsLeaderboard2(CommandContext ctx)
        {
            var bits = await TwitchApi.Helix.Bits.GetBitsLeaderboardAsync();
            var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");
            var channel2 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[0].UserId);
            var channel3 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[1].UserId);
            var channel4 = await TwitchApi.V5.Channels.GetChannelByIDAsync(bits.Listings[2].UserId);

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = channel.DisplayName + " Top 3 bits leaderboard",
                    IconUrl = channel.Logo
                },
                Color = DiscordColor.Purple,
                Title = channel.DisplayName + "'s Top 3 Bits Leaderboard",
                Url = channel.Url,
                ThumbnailUrl = "https://cdn.twitchalerts.com/twitch-bits/images/hd/1000.gif",

            };
            embed.AddField("1st Place: " + channel2.DisplayName, " Score: " + bits.Listings[0].Score);
            embed.AddField("2nd Place: " + channel3.DisplayName, " Score: " + bits.Listings[1].Score);
            embed.AddField("3rd Place: " + channel4.DisplayName, " Score: " + bits.Listings[2].Score);

            await ctx.RespondAsync("\n", embed: embed);
        }

        [Command("subscriberlist"), Aliases("subs", "sublist")]
        internal async Task SubsList(CommandContext ctx)
        {
            try
            {
                var subs = WebCalls.GetRootObject();

                var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");

                var embed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = channel.DisplayName + " Subscriber List",
                        IconUrl = channel.Logo
                    },
                    Color = DiscordColor.Purple,
                    Title = channel.DisplayName + "'s Subscriber List",
                    Url = channel.Url,
                    ThumbnailUrl = "https://cdn.twitchalerts.com/twitch-bits/images/hd/100000.gif",

                };

                for (int i = 0; i < subs.data.Count; i++)
                {
                    if(i == 0) continue;
                    embed.AddField(subs.data[i].user_name,"Plan Name: " + subs.data[i].plan_name, true);
                }

                await ctx.RespondAsync("\n", embed: embed);
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("twitchinfo"), Aliases("tinfo", "chaninfo", "tprofile")]
        internal async Task TwitchInfo(CommandContext ctx)
        {

            bool isStreaming = await TwitchApi.V5.Streams.BroadcasterOnlineAsync("109598326");

            var subs = WebCalls.GetRootObject();

            var channel = await TwitchApi.V5.Channels.GetChannelByIDAsync("109598326");

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = channel.DisplayName,
                    IconUrl = channel.Logo
                },
                Color = DiscordColor.Purple,
                Title = channel.DisplayName + "'s Channel",
                Url = channel.Url,
                ImageUrl = channel.ProfileBanner,
                ThumbnailUrl = channel.Logo, 

            };
            embed.AddField("Streaming Live?: ", isStreaming.ToString(), true);
            embed.AddField("Followers: ", channel.Followers.ToString(), true);
            embed.AddField("Subscribers: ", (subs.data.Count - 1).ToString(), true);
            embed.AddField("Views: ", channel.Views.ToString(), true);                         
            embed.AddField("Rated Mature?: ", channel.Mature.ToString(), true);
            embed.AddField("Streamer Type: ", channel.BroadcasterType, true);
            embed.AddField("Is Partnered?: ", channel.Partner.ToString(), true);
            embed.AddField("Channel Created: ", channel.CreatedAt.ToShortDateString(), true);
            
            await ctx.RespondAsync("\n", embed: embed);


        }

    }
}
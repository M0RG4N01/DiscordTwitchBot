using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;

namespace TwitchBot
{
    public class TwitchBotMain
    {
        public static DiscordClient discord;
        public static CommandsNextExtension commands;
        public static bool DebugLogging = false;
        public static List<TwitchEvents> TwitchEvents = new List<TwitchEvents>();

        public static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = "**Removed**",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug

            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { "!" },
                EnableDms = false,
                EnableMentionPrefix = true,
                CaseSensitive = false

            });

            discord.GetCommandsNext().CommandErrored += CommandErrors;

            commands.RegisterCommands<TwitchCommands>();

            CreateEvents();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        public static void CreateEvents()
        {
            var twitchEvents = new TwitchEvents(new TwitchLib.Client.Models.ConnectionCredentials("M0RG4N01", "**Removed**"), "phenomalytv");

            TwitchEvents.Add(twitchEvents);
        }

        private static async Task CommandErrors(CommandErrorEventArgs e)
        {

            if (e.Exception is ChecksFailedException cfe)
            {
                if (cfe.FailedChecks[0] is CooldownAttribute cooldown)
                {
                    var msg = await e.Context.RespondAsync($"Cooldown: **{cooldown.GetRemainingCooldown(e.Context).Seconds}s remaining.**");
                    await Task.Delay(2500);
                    await msg.DeleteAsync();
                    return;
                }
                else
                {
                    var msg = await e.Context.RespondAsync("Oh no! A random error has occured, the details have been sent to the Developer!");
                    await Task.Delay(2500);
                    await msg.DeleteAsync();
                    discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"{e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

                    var ms = e.Exception.Message;
                    var st = e.Exception.StackTrace;

                    ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
                    st = st.Length > 1000 ? st.Substring(0, 1000) : st;

                    var embed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Botnomaly TwitchBot - An exception occured when executing a command",
                        Description = $"`{e.Exception.GetType()}` occured when executing `{e.Command.Name}`.",

                    }.AddField(e.Command.Name, st, true);
                    embed.AddField("Server ID", e.Context.Guild.Id.ToString(), false);
                    embed.AddField("Channel ID/Name", e.Context.Channel.Id.ToString() + "/" + e.Context.Channel.Name);
                    await discord.GetGuildAsync(492464943220785183).Result.GetChannel(521810791448444950).SendMessageAsync(embed: embed);
                }
            }

            if (DebugLogging)
            {
                discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"{e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

                var ms = e.Exception.Message;
                var st = e.Exception.StackTrace;

                ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
                st = st.Length > 1000 ? st.Substring(0, 1000) : st;

                var embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = "Botnomaly - An exception occured when executing a command",
                    Description = $"`{e.Exception.GetType()}` occured when executing `{e.Command.Name}`.",

                }.AddField(e.Command.Name, st, true);
                await e.Context.Channel.SendMessageAsync(embed: embed);
            }
        }
    }
}

using System;
using DSharpPlus.Entities;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

public struct ChannelMetadata
{
    // Set true if they are hosting
    public bool IsHosting { get; set; }
    // Set the args of the hosted channel.
    public OnNowHostingArgs HostingArgs { get; set; }

    public Channel Channel { get; set; }
}

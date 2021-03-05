/**********************************************************************
    Project           : TwitchBot
    File name         : Bot.cs
    Author            : Rilegis
    Date created      : 25/06/2019
    Purpose           : This file contains the code needed to start the
                        chatbot.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Renamed file to 'Bot.cs' and moved to TwitchLib
**********************************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchBot
{
    public class Bot
    {
        public static TwitchClient client;
        public static Config cfg;
        public static List<Command> bindings = new List<Command>();

        public void Run()
        {
            try
            {
                Loggers.Log("[+]", "Starting chat bot...");

                // Deserialization of the configuration file
                cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("./etc/config.json"));

                // ChatBot's initialization
                ConnectionCredentials credentials = new ConnectionCredentials(cfg.Bot.Username, cfg.Bot.Oauth);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                client = new TwitchClient(customClient);
                client.Initialize(credentials);

                // Register Chat Event handlers
                client.OnError += ChatEventHandlers.OnError;
                client.OnConnectionError += ChatEventHandlers.OnConnectionError;
                client.OnIncorrectLogin += ChatEventHandlers.OnIncorrectLogin;
                client.OnConnected += ChatEventHandlers.OnConnected;
                client.OnDisconnected += ChatEventHandlers.OnDisconnected;
                client.OnMessageReceived += ChatEventHandlers.OnMessageReceived;
                client.OnReconnected += ChatEventHandlers.OnReconnected;

                // Binding commands
                bindings.Add(new Command { commandName = $"{cfg.Bot.Trigger}force", method = CoreCommands.ForceCommands, methodDescription = "Collection of owner-only chat commands." });

                // Connect to Twitch
                client.Connect();
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }
    }
}

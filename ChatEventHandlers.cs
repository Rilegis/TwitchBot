/**********************************************************************
    Project           : TwitchBot
    File name         : ChatEvents.cs
    Author            : Rilegis
    Date created      : 25/06/2019
    Purpose           : This file contains the code for the events that
                        will happen after the chatbot has been started.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Renamed file to 'ChatEventHandlers.cs' and
                                    added new events.
    19/04/2020  Rilegis     3       Added Database communication to 'ForceJoin' and 'ForcePart'.
**********************************************************************/

using MySql.Data.MySqlClient;
using System;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;

namespace TwitchBot
{
    public static class ChatEventHandlers
    {
        private static MySqlConnectionStringBuilder _stringBuilder;
        private static MySqlConnection _dbConn;
        private static MySqlCommand _cmd;
        private static MySqlDataReader _rdr;

        public static void OnLog(object sender, OnLogArgs e)
        {
            Loggers.Log($"{e.Data}");
        }
        public static void OnError(object sender, OnErrorEventArgs e)
        {
            Loggers.ExceptionLogger(e.Exception);
        }

        public static void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Loggers.Log("[!]", $"Failed to connect to Twitch's websocket due to error: {e.Error.Message}");
        }

        public static void OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            Loggers.Log("[!]", $"{e.Exception.Message}");
        }

        public static void OnConnected(object sender, OnConnectedArgs e)
        {
            try
            {
                Loggers.Log("[+]", $"Successfully connected as: {e.BotUsername}");
                Loggers.Log("[+]", "Joining channels...");
                if (Bot.cfg.Bot.Channels.Length > 0)
                    // Joins channels defined in config file
                    foreach (string channel in Bot.cfg.Bot.Channels) Bot.client.JoinChannel(channel);
                if (!Bot.cfg.Database.Username.Equals(""))
                {
                    // If a valid username is given in the database configuration object, then join the channels that are stored inside the 'channels' table
                    _stringBuilder = new MySqlConnectionStringBuilder
                    {
                        Server = Bot.cfg.Database.Host,
                        Port = Bot.cfg.Database.Port,
                        UserID = Bot.cfg.Database.Username,
                        Password = Bot.cfg.Database.Password,
                        Database = Bot.cfg.Database.DBName
                    };
                    _dbConn = new MySqlConnection(_stringBuilder.ToString());
                    _dbConn.Open();
                    _cmd = new MySqlCommand
                    {
                        Connection = _dbConn,
                    };
                    // Get joinable channels list
                    _cmd.CommandText = $"SELECT channel FROM `channels` WHERE `isJoined`=1";
                    _rdr = _cmd.ExecuteReader();

                    while (_rdr.Read()) Bot.client.JoinChannel(_rdr.GetString(0));

                    // Then close DB connection and dispose command instance
                    _dbConn.Dispose();
                    _dbConn.Close();
                    _cmd.Dispose();
                }
                Loggers.Log("Done.");

                Bot.client.SendRaw($"PRIVMSG #rilebot :{Bot.cfg.Bot.Username} is now online.");
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }

        public static void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Loggers.Log("[-]", $"Disconnected from Twitch's websocket.");
        }

        public static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            try
            {
                string[] messageArray = e.ChatMessage.Message.Split(" ");
                foreach (Command command in Bot.bindings)
                {
                    if (command.commandName == messageArray[0]) command.method(Bot.client, e);
                }
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }

        public static void OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            Loggers.Log("[+]", $"Successfully reconnected to Twitch's websocket");
        }
    }
}

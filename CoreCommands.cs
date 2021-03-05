/**********************************************************************
    Project           : TwitchBot
    File name         : CoreCommands.cs
    Author            : Rilegis
    Date created      : 25/06/2019
    Purpose           : This file contains all the core commands of the
                        chatbot.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Modified commands to accomodate TwitchLib.
**********************************************************************/

using MySql.Data.MySqlClient;
using System;
using System.IO;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TwitchBot
{
    public static class CoreCommands
    {
        private static MySqlConnectionStringBuilder _stringBuilder;
        private static MySqlConnection _dbConn;
        private static MySqlCommand _cmd;
        private static MySqlDataReader _rdr;

        private static string _tabName;

        //----------------------------------------------------------------------------------------------
        // Bot-specific commands (OWNER ONLY)
        //----------------------------------------------------------------------------------------------
        public static void ForceCommands(TwitchClient client, OnMessageReceivedArgs args)
        {
            // user!username@host PRIVMSG #channel :!force command args
            try
            {
                string[] messageArray = args.ChatMessage.Message.Split(" ");
                if ((messageArray.Length < 2) && (args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)))
                    client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} Not enough parameters.");
                else if ((messageArray.Length > 1) && (args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)))
                {
                    switch (messageArray[1])
                    {
                        case "say":
                            ForceSay(client, args);
                            break;
                        case "join":
                            ForceJoin(client, args);
                            break;
                        case "part":
                            ForcePart(client, args);
                            break;
                    }
                }
                else
                {
                    client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} you don't have the right access to perform that command.");
                    Loggers.Log("[!]", $"{args.ChatMessage.DisplayName} tried to perform force on #{args.ChatMessage.Channel}.");
                }
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }

        public static void ForceSay(TwitchClient client, OnMessageReceivedArgs args)
        {
            // user!username@host PRIVMSG #channel :!force say #channel message
            try
            {
                string[] messageArray = args.ChatMessage.Message.Split(" ");
                if ((messageArray.Length > 2) && (messageArray[2][0] != '#')) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} Channel name must start with '#'.");
                else if ((messageArray.Length < 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName}: No channel and message indicated.");
                else if ((messageArray.Length == 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName}: No message to relay.");
                else if ((messageArray.Length > 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :{args.ChatMessage.Message.Substring(20)}");
                else
                {
                    client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} you don't have the right access to perform that command. This incident will be reported.");
                    Loggers.Log("[!]", $"{args.ChatMessage.DisplayName} tried to perform force-say on  #{args.ChatMessage.Channel}.");

                    // Check if logfile exists
                    if (!File.Exists("./logs/noaccess"))
                    {
                        Loggers.Log("[!]", "LogFile 'noaccess' does not exists...generating one");
                        File.Create("./logs/noaccess");
                    }

                    // Then write inside it
                    using StreamWriter sw = File.AppendText("./logs/noaccess");
                    sw.WriteLine($"{DateTime.Now:[dd/MM/yyyy - HH:mm:ss]} {args.ChatMessage.DisplayName} tried to perform force-say on #{args.ChatMessage.Channel}.");
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }

        public static void ForceJoin(TwitchClient client, OnMessageReceivedArgs args)
        {
            // user!username@host PRIVMSG #channel :!force join #channel
            try
            {
                string[] messageArray = args.ChatMessage.Message.Split(" ");
                string chanName;
                if ((messageArray.Length > 2) && (messageArray[2][0] != '#')) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} Channel name must start with '#'.");
                else if ((messageArray.Length < 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName}: No channel indicated.");
                else if ((messageArray.Length == 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID))
                {
                    chanName = messageArray[2].Substring(1);
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

                    // Check if channel is alredy in DB.channels
                    _cmd.CommandText = $"SELECT * FROM `channels` WHERE `channel`=@channel";
                    _cmd.Prepare();
                    _cmd.Parameters.AddWithValue("@channel", chanName);
                    _rdr = _cmd.ExecuteReader();
                    _rdr.Read(); // Only one entry is allowed to exist
                    if (_rdr.HasRows) // If channel is alredy in DB.channels
                    {
                        _rdr.Close(); // Close previous DataReader
                        // Set DB.channels.isJoined to 1
                        _cmd.CommandText = $"UPDATE `channels` SET `isJoined`=1 WHERE `channel`=@channelUpdate";
                        _cmd.Prepare();
                        _cmd.Parameters.AddWithValue("@channelUpdate", chanName);
                        _cmd.ExecuteNonQuery();
                        // And join the channel
                        client.JoinChannel(messageArray[2].Substring(1));
                        Loggers.Log("[+]", $"Joined channel #{chanName}");
                    }
                    else // If channel is not in DB.channels
                    {
                        chanName = messageArray[2].Substring(1);
                        _rdr.Close(); // Close previous DataReader
                        // Add a new channel entry
                        _cmd.CommandText = $"INSERT INTO `channels` (`channel`, `isJoined`) VALUES (@newChannel, @isJoined)";
                        _cmd.Prepare();
                        _cmd.Parameters.AddWithValue("@newChannel", chanName);
                        _cmd.Parameters.AddWithValue("@isJoined", 1);
                        _cmd.ExecuteNonQuery();
                        // Add a command table for the newly joined channel
                        _tabName = $"{chanName}_commands";
                        _cmd.CommandText = String.Format(File.ReadAllText("./etc/sql/NewCommandsTable.sql"), _tabName); // Not ideal, but it wouldn't work otherwise
                        _cmd.ExecuteNonQuery();
                        client.JoinChannel(chanName);
                        Loggers.Log("[+]", $"Joined channel #{chanName}");
                    }
                    // Then close DB connection and dispose command instance
                    _dbConn.Dispose();
                    _dbConn.Close();
                    _cmd.Dispose();
                }
                else
                {
                    client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} you don't have the right access to perform that command. This incident will be reported.");
                    Loggers.Log("[!]", $"{args.ChatMessage.DisplayName} tried to perform force-join on #{args.ChatMessage.Channel}.");

                    // Check if logfile exists
                    if (!File.Exists("./logs/noaccess"))
                    {
                        Loggers.Log("[!]", "LogFile 'noaccess' does not exists...generating one");
                        File.Create("./logs/noaccess");
                    }

                    // Then write inside it
                    using StreamWriter sw = File.AppendText("./logs/noaccess");
                    sw.WriteLine($"{DateTime.Now:[dd/MM/yyyy - HH:mm:ss]} {args.ChatMessage.DisplayName} tried to perform force-join on #{args.ChatMessage.Channel}.");
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }

        public static void ForcePart(TwitchClient client, OnMessageReceivedArgs args)
        {
            // user!username@host PRIVMSG #channel :!force part #channel
            try
            {
                string[] messageArray = args.ChatMessage.Message.Split(" ");
                string chanName;
                if ((messageArray.Length > 2) && (messageArray[2][0] != '#')) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} Channel name must start with '#'.");
                else if ((messageArray.Length < 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID)) client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName}: No channel indicated.");
                else if ((messageArray.Length == 3) && args.ChatMessage.UserId.Equals(Bot.cfg.Bot.OwnerID))
                {
                    chanName = messageArray[2].Substring(1);
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

                    // Check if channel is in DB.channels and currently joined
                    _cmd.CommandText = $"SELECT * FROM `channels` WHERE `channel`=@channel AND `isJoined`=1";
                    _cmd.Prepare();
                    _cmd.Parameters.AddWithValue("@channel", chanName);
                    _rdr = _cmd.ExecuteReader();
                    _rdr.Read(); // Only one entry is allowed to exist
                    if (_rdr.HasRows) // If channel is in DB.channels
                    {
                        _rdr.Close(); // Close previous DataReader
                        // Set DB.channels.isJoined to 0
                        _cmd.CommandText = $"UPDATE `channels` SET `isJoined`=0 WHERE `channel`=@channelUpdate";
                        _cmd.Prepare();
                        _cmd.Parameters.AddWithValue("@channelUpdate", chanName);
                        _cmd.ExecuteNonQuery();
                        // And part the channel
                        client.LeaveChannel(chanName);
                        Loggers.Log($"Parted from channel #{chanName}");
                    }
                    else // If channel is not in DB.channels or is bot is currently not on channel, notify the user.
                    {
                        client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} I'm not in that channel.");
                    }
                    // Then close DB connection and dispose command instance
                    _dbConn.Dispose();
                    _dbConn.Close();
                    _cmd.Dispose();
                }
                else
                {
                    client.SendRaw($"PRIVMSG #{args.ChatMessage.Channel} :@{args.ChatMessage.DisplayName} you don't have the right access to perform that command. This incident will be reported.");
                    Loggers.Log("[!]", $"{args.ChatMessage.DisplayName} tried to perform force-join on #{args.ChatMessage.Channel}.");

                    // Check if logfile exists
                    if (!File.Exists("./logs/noaccess"))
                    {
                        Loggers.Log("[!]", "LogFile 'noaccess' does not exists...generating one");
                        File.Create("./logs/noaccess");
                    }

                    // Then write inside it
                    using StreamWriter sw = File.AppendText("./logs/noaccess");
                    sw.WriteLine($"{DateTime.Now:[dd/MM/yyyy - HH:mm:ss]} {args.ChatMessage.DisplayName} tried to perform force-join on #{args.ChatMessage.Channel}.");
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }
    }
}

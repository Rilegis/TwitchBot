/**********************************************************************
    Project           : TwitchBot
    File name         : Config.cs
    Author            : Rilegis
    Date created      : 5/10/2019
    Purpose           : This file contains the definition of the config
                        file stored in './etc/config.json'.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Renamed file to 'Config.cs',
                                    removed 'PubSubWS', 'ChatWS', properties,
                                    added 'Trigger' property,
                                    changed 'MainChan' to 'Channels',
                                    changed 'Owner' to 'OwnerHost',
                                    added 'OwnerID'.
**********************************************************************/

namespace TwitchBot
{
    public class Config
    {
        public BotConfig Bot { get; set; }
        public DatabaseConfig Database { get; set; }
    }

    public class BotConfig
    {
        public string Username { get; set; }
        public string Oauth { get; set; }
        public string OwnerHost { get; set; }
        public string OwnerID { get; set; }
        public string Trigger { get; set; }
        public string[] Channels { get; set; }
    }

    public class DatabaseConfig
    {
        public string Host { get; set; }
        public uint Port { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

/**********************************************************************
    Project           : TwitchBot
    File name         : CommandStruct.cs
    Author            : Rilegis
    Date created      : 18/04/2020
    Purpose           : This file contains the structure for the bot's command
                        bindings.

    Revision History  :
    Date        Author      Ref     Revision 
    18/04/2020  Rilegis     1       Added file.
**********************************************************************/

using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TwitchBot
{
    public struct Command
    {
        public string commandName;
        public Action<TwitchClient, OnMessageReceivedArgs> method;
        public string methodDescription;
    }
}

/**********************************************************************
    Project           : TwitchBot
    File name         : Program.cs
    Author            : Rilegis
    Date created      : 25/6/2019
    Purpose           : This is the project's entry point file.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Moved from WebSocketSharp to TwitchLib
    11/04/2021  Rilegis     3       Changed target framework from 'NET Core 3.1' to '.NET 5'
                                    and updated packages.
**********************************************************************/

using System;
using System.IO;
using System.Threading;

namespace TwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Display MOTD
            Console.WriteLine(File.ReadAllText("./etc/motd"));

            try
            {
                // Check main folder filesystem
                Loggers.Log("Checking folder filesystem...");
                if (!Directory.Exists("./logs"))
                {
                    Loggers.Log("[!]", "Logs folder not found...created");
                    Directory.CreateDirectory("./logs");
                }
                Loggers.Log("Done.");

                // Start ChatBot's thread
                new Thread(() => new Bot().Run()).Start();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Loggers.ExceptionLogger(ex);
            }
        }
    }
}

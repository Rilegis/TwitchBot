/**********************************************************************
    Project           : TwitchBot
    File name         : Loggers.cs
    Author            : Rilegis
    Date created      : 30/6/2019
    Purpose           : This file contains the logger methods.

    Revision History  :
    Date        Author      Ref     Revision 
    22/12/2019  Rilegis     1       Added file header.
    18/04/2020  Rilegis     2       Renamed file to 'Loggers.cs'
**********************************************************************/

using System;
using System.IO;

namespace TwitchBot
{
    public static class Loggers
    {
        private static string _dateTime;

        public static void ExceptionLogger(Exception ex)
        {
            if (!Directory.Exists("./logs/exceptions")) Directory.CreateDirectory("./logs/exceptions");

            _dateTime = DateTime.Now.ToString("[dd/MM/yyyy - HH:mm:ss]");
            Console.Error.WriteLine($"{_dateTime}\tERROR! Showing stack trace:\n{ex}\n{_dateTime}\tSaving stack trace...");
            // Dump stack trace on a file
            var fileName = $"EXCEPTION_{DateTime.Now:ddMMyy_HHmmss}.log";
            using StreamWriter file = new StreamWriter($"./logs/exceptions/{fileName}", true);
            file.WriteLine(ex.ToString());
            file.Dispose();
            file.Close();
        }

        public static void Log(string log)
        {
            _dateTime = DateTime.Now.ToString("[dd/MM/yyyy - HH:mm:ss]");
            Console.WriteLine($"{_dateTime}     {log}");
        }

        public static void Log(string mode, string log)
        {
            _dateTime = DateTime.Now.ToString("[dd/MM/yyyy - HH:mm:ss]");
            Console.WriteLine($"{_dateTime} {mode} {log}");
        }
    }
}

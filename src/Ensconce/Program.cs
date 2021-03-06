﻿using System;
using System.Threading;

namespace Ensconce
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                MainLogic(args);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Something went wrong. :(");
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return -1;
            }
        }

        private static void MainLogic(string[] args)
        {
            Arguments.SetUpAndParseOptions(args);

            Logging.Log("Arguments parsed");

            if (!string.IsNullOrWhiteSpace(Arguments.DictionarySavePath) || !string.IsNullOrWhiteSpace(Arguments.DictionaryPostUrl))
            {
                DictionaryExport.ExportTagDictionary();
                return;
            }

            if (Arguments.ReadFromStdIn)
            {
                using (var input = Console.In)
                {
                    Console.Out.Write(input.ReadToEnd().Render());
                }
                // No other operations can be performed when reading from stdin
                return;
            }

            if (Arguments.DeployReports || Arguments.DeployReportingRole)
            {
                Reporting.RunReportingServices();
                // No other operations can be performed when deploying reports
                return;
            }

            if (Arguments.UpdateConfig)
            {
                TagSubstitution.DefaultUpdate();
            }

            if (!string.IsNullOrEmpty(Arguments.TemplateFilters))
            {
                TagSubstitution.UpdateFiles();
            }

            if (!string.IsNullOrEmpty(Arguments.ConnectionString) || !string.IsNullOrEmpty(Arguments.DatabaseName))
            {
                DatabaseInteraction.DoDeployment();
            }

            if (Arguments.Replace)
            {
                Arguments.DeployTo.ForEach(FileInteraction.DeleteDirectory);
                Thread.Sleep(500); // Allow for the delete to complete
            }

            if (Arguments.CopyTo || Arguments.Replace)
            {
                Arguments.DeployTo.ForEach(dt => FileInteraction.CopyDirectory(Arguments.DeployFrom, dt));
            }

            Logging.Log("Ensconce operation complete");
        }
    }
}
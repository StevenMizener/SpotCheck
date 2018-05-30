using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotCheck
{
    /// <summary>**********************************************************************************************************
    ///  Application Name: SoftSpot Command Console
    ///  Description: A simple console application that provides commandline input/output to highly performance optimized, 
    ///  efficient and accurate file content duplication checking algorithms.   
    /// *******************************************************************************************************************
    ///  All source code Copyright 2018, Steven L. Taylor (Blacksmith Software Works)
    /// **********************************************************************************************************</summary>
    class Program
    {
        #region Command Line Constants
        private const string SOFT_SPOT = "SoftSpot";
        private const string BATCH_SPOT = "BatchSpot";
        private const string HELP_ME = "HelpMe";
        #endregion

        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0].Contains("/c"))
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("*           Blacksmith Software Works - SPOT CHECK COMMAND CONSOLE            *");
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("");
                Console.WriteLine(@"Input command line <enter> or input HelpMe <enter> for help... ");
                Console.WriteLine("");
                Console.WriteLine("");
                ReadInput();
            }
        }
        private static void ReadInput()
        {
            string input;
            do
            {
                input = Console.ReadLine();
                ProcessInput(input);
                if (SpotCheckToolBox.SpotCheck.ErrorMessages.Count > 0)
                {
                    SpotCheckToolBox.SpotCheck.ErrorMessages.ForEach(t=> Console.WriteLine("Message: " + t));
                }
            } while (input != "");
        }
        private static void ProcessInput(string input)
        {
            try
            {
                input = input.TrimEnd(' ');
                string[] commandlinetokens = input.Split(' ');
                List<string> results = new List<string>();
                switch (commandlinetokens[0].ToString())
                {                        
                    case SOFT_SPOT:
                        if (commandlinetokens.Length > 3)
                        {
                            Console.WriteLine(SpotCheckToolBox.SpotCheck.SoftSpotCheck(commandlinetokens[1], commandlinetokens[2], int.Parse(commandlinetokens[3])).ToString());
                        }
                        else
                        {
                            Console.WriteLine(SpotCheckToolBox.SpotCheck.SoftSpotCheck(commandlinetokens[1], commandlinetokens[2]).ToString());
                        }
                        break;
                    case BATCH_SPOT:
                        String BatchPath = commandlinetokens[1];
                        if (commandlinetokens.Length == 2)
                        {
                            results = SpotCheckToolBox.SpotCheck.BatchSpotCheck(BatchPath);
                        }
                        else if (commandlinetokens.Length == 3)
                        {
                            results = SpotCheckToolBox.SpotCheck.BatchSpotCheck(BatchPath, int.Parse(commandlinetokens[2]));
                        }
                        else
                        {
                            throw new Exception("Invalid number of command input options. Please check your input.");
                        }
                        if (results.Count > 0)
                        {
                            foreach (string line in results)
                            {
                                Console.WriteLine(line);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Batch processing of files in path: " + BatchPath + " has completed with no likely duplicate files found.");
                        }
                        break;
                    case HELP_ME:
                        Console.Clear();
                        Console.WriteLine("******************************************************************************");
                        Console.WriteLine("*                             HELP AND OPTIONS                               *");
                        Console.WriteLine("******************************************************************************");
                        Console.WriteLine("Supported commands include the following, with options shown...");
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("SoftSpot [SrcFilePath] [TrgFilePath] [optional precision factor]");
                        Console.WriteLine("   Tests source file against target file and returns true if the files seem");
                        Console.WriteLine("   to contain duplicate binary content, false if they do not.");
                        Console.WriteLine("");
                        Console.WriteLine("BatchSpot [SearchFilePath] [optional precision factor]");
                        Console.WriteLine("   If the specified path exists and contains a subfolder named SourceFiles,");
                        Console.WriteLine("   this batch processing method tests each folder in the SourceFiles folder");
                        Console.WriteLine("   for duplicate content in each of the files in the top level input folder.");
                        Console.WriteLine("");
                        Console.WriteLine("HelpMe - What you see is all this does right now. Hope it helps. Good luck! :)");
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("******************************************************************************");
                        Console.WriteLine("******************************************************************************");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SpotCheckToolBox.SpotCheck.ErrorMessages.Add(ex.Message);
                Console.WriteLine("There has been an error processing your command. Message: " + ex.Message);
            }
        }
    }
}
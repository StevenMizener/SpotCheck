////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SpotCheck.cs
//
// summary:	Implements the spot check algorithm, a highly efficient large file dupe check function
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SpotCheckToolBox
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// *  ********************************************************************************************
    /// 
    /// *  
    /// *  *********** Application Name: SoftSpotSDK Description: A lightweight, highly performance
    /// optimized, efficient and accurate file content duplication checking algorithm and small
    /// helper software component library.
    /// *  ********************************************************************************************
    /// 
    /// *  
    /// *  ******************** All source code Copyright 2018, Steven L. Taylor
    /// *  ********************************************************************************************
    /// 
    /// *  
    /// *  ***********.
    /// </summary>
    ///
    /// <remarks>   Steven L. Taylor, 5/31/2018. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SpotCheck
    {
        #region Declarations
        /// <summary>   The error messages. </summary>
        private static List<string> errorMessages = new List<string>();
        /// <summary>   True to last result. </summary>
        private static bool lastResult;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the error messages. Reserved for future use. </summary>
        ///
        /// <value> The error messages. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<string> ErrorMessages { get => errorMessages; set => errorMessages = value; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating the last result of processing. May not be reliable to
        /// external callers in this build.
        /// </summary>
        ///
        /// <value> True if last result true, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool LastResult
        {
            get { return lastResult; }
            set { lastResult = value; if (value == true) ErrorMessages.Clear(); }
        }
        #endregion

        #region SpotCheck File Duplication Check Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests two input files efficiently for size and metadata equality. </summary>
        ///
        /// <remarks>   Steven Taylor, 5/31/2018. </remarks>
        ///
        /// <param name="pathA">    The path a. </param>
        /// <param name="pathB">    The path b. </param>
        ///
        /// <returns>   True if file size and attributes are identical, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool MetaSpotCheck(string pathA, string pathB) // Can be used to perform a quick check of file length and file system metadata
        {
            FileInfo fileA = new FileInfo(pathA);
            FileInfo fileB = new FileInfo(pathB);
            if (fileA.Length != fileB.Length || !fileA.Attributes.Equals(fileB.Attributes))
            {
                LastResult = false;
                return LastResult;
            }
            LastResult = true;
            return LastResult;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A streamlined, minimal implemtation of SpotCheck with no input validation. </summary>
        ///
        /// <remarks>   Steven L. Taylor, 5/31/2018. </remarks>
        ///
        /// <param name="pathA">    The path a. </param>
        /// <param name="pathB">    The path b. </param>
        ///
        /// <returns>
        /// Returns True if files appear to contain identical binary content, false if not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool MicroSpotCheck(string pathA, string pathB) // Performs the simplest, most efficient check with no input validation or other logic
        {
            if (CompareSampleSets(GetBinarySamples(new FileInfo(pathA)), GetBinarySamples(new FileInfo(pathB))))
            {
                LastResult = true;
                return LastResult;
            }
            LastResult = false;
            return LastResult;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// A highly efficient, file size insensitive file content duplication checking algorithm.
        /// </summary>
        ///
        /// <remarks>   Steven L. Taylor, 5/31/2018. </remarks>
        ///
        /// <param name="pathA">            The path of the source file to compare. </param>
        /// <param name="pathB">            The path of the target file to compare. </param>
        /// <param name="numberofSamples">  (Optional) The number of samples to process. </param>
        ///
        /// <returns>
        /// Returns True if files appear to contain identical binary content, false if not.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool SoftSpotCheck(string pathA, string pathB, int numberofSamples = 5) // Simple spot check with minimal input validation and optional error messaging
        {                                                                                     
            LastResult = true; // Reset processing result flag
            FileInfo fileA = new FileInfo(pathA);
            FileInfo fileB = new FileInfo(pathB);
            if (fileA.Length != fileB.Length)
            {
                //ErrorMessages.Add("File length mismatch.");
                LastResult = false;
                return LastResult;
            }
            if (fileA.Length <= 5000)
            {
                byte[] firstBytes = File.ReadAllBytes(pathA);
                byte[] secondBytes = File.ReadAllBytes(pathB);
                int it = 0;
                foreach(byte bite in firstBytes)
                {
                    if(firstBytes[it] != secondBytes[it])
                    {
                        LastResult = false;
                        return LastResult;
                    }
                    it++;
                }
                return LastResult;
            }
            if (CompareSampleSets(GetBinarySamples(fileA, numberofSamples), GetBinarySamples(fileB, numberofSamples)))
            {
                LastResult = true;
                return LastResult;
            }
            else
            {
                LastResult = false;
                return LastResult;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Batch spot check. </summary>
        ///
        /// <remarks>   Steven L. Taylor, 5/31/2018. </remarks>
        ///
        /// <param name="path">             Full pathname of the folder root containing files to process. </param>
        /// <param name="numberofSamples">  (Optional) The numberof samples to process for each file. </param>
        ///
        /// <returns>
        /// A List&lt;&amp;String&gt;&amp; containing result report output for any files matched.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<String> BatchSpotCheck(string path, int numberofSamples = 5) // A simple batch process for efficient bulk file duplication checking
        {
            // Expects existing subfolder "SourceFiles" with source files to compare to files in top directory
            String[] srcFilePaths = Directory.GetFiles(path + @"\SourceFiles\", "*", SearchOption.TopDirectoryOnly);
            String[] targetFilePaths = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            List<String> results = new List<String>();
            foreach(string filePathA in srcFilePaths)
            {
                foreach(string filePathB in targetFilePaths)
                {
                    if (SoftSpotCheck(filePathA, filePathB, numberofSamples))
                    {
                        if(results.Count == 0) { results.Add("Matches found. See detail results below."); }
                        results.Add("SourceFile: " + filePathA + Environment.NewLine + "TargetFile: " + filePathB);
                    }
                }
            }
            return results;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compares binary data sample results for equality. </summary>
        ///
        /// <remarks>   Saulgoodman, 5/31/2018. </remarks>
        ///
        /// <param name="samplesA"> The first array of samples to compare. </param>
        /// <param name="samplesB"> The second array of sample to compare. </param>
        ///
        /// <returns>   True if samples are identical, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool CompareSampleSets(byte[] samplesA, byte[] samplesB) // Compares two byte arrays for equality efficiently
        {
            for (int i = 0; i <= samplesA.Length - 1; i++)
            {
                if (!samplesA[i].Equals(samplesB[i]))
                {
                    //ErrorMessages.Add("Sample#" + i + " failed match.");
                    LastResult = false;
                    return LastResult;
                }
            }
            LastResult = true;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs the most stringent file equality checking, including inspecting file attributes for
        /// equality.
        /// </summary>
        ///
        /// <remarks>   Saulgoodman, 5/31/2018. </remarks>
        ///
        /// <param name="pathA">    The path to the source file to test. </param>
        /// <param name="pathB">    The path to the target file to test. </param>
        ///
        /// <returns>   True if file binary contents and metadata appear to match, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool HardSpotCheck(string pathA, string pathB) // Performs the most stringent duplication checking, checking file attributes for equality and sampling binary content
        {
            FileInfo fileA = new FileInfo(pathA);
            FileInfo fileB = new FileInfo(pathB);
            if (MetaSpotCheck(fileA.DirectoryName, fileB.DirectoryName) && GetBinarySamples(fileA) == GetBinarySamples(fileB))
            {
                LastResult = true;
                return LastResult;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets binary samples. </summary>
        ///
        /// <remarks>   Saulgoodman, 5/31/2018. </remarks>
        ///
        /// <param name="file">             The file to sample. </param>
        /// <param name="numberofSamples">  (Optional) The number of samples to return. </param>
        ///
        /// <returns>
        /// An array of byte containing samples from points distributed at fixed relative positions
        /// throughout the given file. Position of sampling is absolute for both relative to file size.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static byte[] GetBinarySamples(FileInfo file, int numberofSamples = 5) // Returns the number of binary samples specified
        {
            // Declare and initialize working array and other types
            string[] samples = new string[numberofSamples];
            long[] samplePositions = new long[numberofSamples];
            decimal baseUnit = 0;
            byte[] bites = new byte[numberofSamples];

            // If default number of samples, apply default sampling logic
            if (numberofSamples == 5)
            {
                samplePositions = new long[] { 0, Convert.ToInt64(file.Length * 0.25), Convert.ToInt64(file.Length * 0.50), Convert.ToInt64(file.Length * 0.75), Convert.ToInt64(file.Length - 1) };
            }
            else
            {                
                baseUnit = (decimal)(long)file.Length / (long)numberofSamples;
                for (int i = 0; i <= numberofSamples - 1; i++)
                {
                    if (i == 0)
                    {
                        samplePositions[i] = 0;
                    }
                    else if (i == numberofSamples - 1)
                    {
                        samplePositions[i] = (long)Math.Round((double)file.Length - 1, MidpointRounding.AwayFromZero); // Grab the last possible position for the sample size
                    }
                    else
                    {
                        samplePositions[i] = (long)(baseUnit * i);
                    }
                }
            }
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int i = 0; //keep state of loop iteration to calculate index pointer                   
                    foreach (long pos in samplePositions)
                    {
                        fs.Position = pos;
                        byte bite = reader.ReadByte();
                        bites[i] = bite;
                        i++;
                    }
                }
            }
            return bites;
        }
        #endregion

        #region Miscellaneous Utility and Test Scaffolding Code

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Calculates the md 5. </summary>
        ///
        /// <remarks>   Saulgoodman, 5/31/2018. </remarks>
        ///
        /// <param name="filename1">    The first filename. </param>
        /// <param name="filename2">    The second filename. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool CalculateMD5(string filename1, string filename2) // A reference unit testing implementation of the MD5 hash method for comparing large files
        {
            using (var md5 = MD5.Create())
            {
                using (var stream1 = File.OpenRead(filename1))
                {
                    using (var stream2 = File.OpenRead(filename2))
                    {
                        var hash1 = md5.ComputeHash(stream1);
                        var hash2 = md5.ComputeHash(stream2);
                        for (int i = 0; i < hash1.Length; i++)
                        {
                            if (hash1[i] != hash2[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
        }
    }

    #endregion
}
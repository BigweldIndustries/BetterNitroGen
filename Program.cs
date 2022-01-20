using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Threading.Thread;
using FireUtils;

namespace NitroGenSharp
{
    class Program
    {  
        static object sync = new object(); // Sync
        static  int codenum; // Codes requestsd
        static int generated = 0; // Codes generated
        static bool link; // If user wants link
        static int threadnum; // Number of threads to use
        static string n = Environment.NewLine; // New line
        static int perthread; // Codes to generate per thread
        static int perthreadremainder; // Codes to generate for remainder
        static dynamic sb; // Head StringBuilder
                
        public static string RandomCode(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            var result = new char[length];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(bytes);
            }
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        static void Main(string[] args)
        {

            // Print title

            Console.WriteLine(@"    )                                   (   (     ");
            Console.WriteLine(@" ( /(       )        (                  )\ ))\ )  ");
            Console.WriteLine(@" )\())(  ( /((       )\ )     (        (()/(()/(  ");
            Console.WriteLine(@"((_)\ )\ )\())(   ( (()/(    ))\ (      /(_))(_)) ");
            Console.WriteLine(@" _((_|(_|_))(()\  )\ /(_))_ /((_))\ )  (_))(_))   ");
            Console.WriteLine(@"| \| |(_) |_ ((_)((_|_)) __(_)) _(_/(  |_ _|_ _|  ");
            Console.WriteLine(@"| .` || |  _| '_/ _ \ | (_ / -_) ' \))  | | | |   ");
            Console.WriteLine(@"|_|\_||_|\__|_| \___/  \___\___|_||_|  |___|___|  ");
                                                  
                                                
           
            bool clear; // Does user want to clear codes file
            

            // Get number of codes wanted
            codenum = FireUtils.Input.checkInput($"{n}How many codes should be generated: ", "num", "Please enter a valid number", null, 1);

            // Get number of threads user wants
            threadnum = FireUtils.Input.checkInput("How many threads should run? (Maximum 100): ", "num", "Please enter a number under 100", 100, 1);
            // Get if user wants link end or not
            link = FireUtils.Input.checkInput("Should we add \"https://discord.gift/\" to the beggining? (Y or N): ", "bool", "Please enter either Y or N", null, null);

            // Get if user wants to clear codes file or not
            clear = FireUtils.Input.checkInput("Should clear the codes file first? (Y or N): ", "bool", "Please enter either Y or N", null, null);

            // Creates / clears file
            if (clear == true)
            {
                System.IO.File.WriteAllText(@"codes.txt",string.Empty);
            }

            sb = new StringBuilder("", codenum*38); // Where the codes are stored
            
            perthread = codenum / threadnum;
            perthreadremainder = codenum % threadnum;

            Thread ETAthread = new Thread(new ThreadStart(ETA));
            ETAthread.Start();

            
            for (int i=0; i<threadnum; i++){
                // loops
                Thread genthread = new Thread(new ThreadStart(Generate));
                genthread.Start();
 
            }
            if (perthreadremainder != 0){
                GenRemainder();
            }
              
            
        }

        public static void ETA()
        {

            string perc; // Percentage completed
            double percdub; // Percentage before rounding
            string codestring = ""; // List of codes generated, used for writing to file

            while (generated < codenum){
                percdub = ((double) generated / codenum)* 100;
                perc = Math.Round(percdub, 2).ToString("0.00");
                Console.Write($"Generating... (%{perc})                                     {generated.ToString()}/{codenum.ToString()}\r");       
                Thread.Sleep(100);
                
            }

            codestring = sb.ToString();
            codestring = codestring.Substring(0, codestring.Length-1); // Remove trailing newline
            using (StreamWriter sww = File.AppendText("codes.txt"))
            {
                
            sww.Write(codestring);
            }	
            Console.WriteLine("______________________________________________________________________________________________________________");
            Console.WriteLine("All codes have been appended to the codes file. Press enter to close the program");
            Console.ReadLine();
            System.Environment.Exit(0); 
        }
        
        public static void Generate()
        {
            string code; // Random code
            StringBuilder tempsb = new StringBuilder("", codenum*17); // Local string builder
            int genlocal = 0; // Local generation count

            while (genlocal < perthread)
            {
                code = RandomCode(16); // Make code
                switch (link)
                {
                    case false:
                        // No link
                        tempsb.Append($"{code}{Environment.NewLine}");
                        break;

                    case true:
                        // Yes link
                        tempsb.Append($"https://discord.gift/{code}{Environment.NewLine}");
                        break;
                }

                lock( sync ) generated += 1; // Another code down globally
                genlocal += 1; // Another code down locally
            }


            lock( sync ) sb.Append(tempsb); // Write local stringbuilder to main stringbuilder
            
        }

        public static void GenRemainder()
        {
            string code; // Random code
            int genlocal = 0; // Local generation count
            StringBuilder tempsb = new StringBuilder("", codenum*17); // Local string builder

            while (genlocal < perthreadremainder)
            {
                code = RandomCode(16); // Make code
                switch (link)
                {
                    case false:
                        // No link
                        tempsb.Append($"{code}{Environment.NewLine}");
                        break;

                    case true:
                        // Yes link
                        tempsb.Append($"https://discord.gift/{code}{Environment.NewLine}");
                        break;
                }

                lock( sync ) generated += 1; // Another code down globally
                genlocal += 1; // Another code down locally
            }
            lock( sync ) sb.Append(tempsb); // Write local stringbuilder to main stringbuilder
            
        }
    }
}
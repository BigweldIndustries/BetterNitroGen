using System;
using System.Text;

namespace FireUtils
{
    class Input 
    {

        public static dynamic checkInput(string query, string check, string errorMsg, dynamic max, dynamic min)
        {
            bool goodInput = false;

            while(goodInput == false)
            {
                // Question
                Console.Write(query);
                string inp = Console.ReadLine();
                
                // Check type of answer wanted
                switch (check)
                {

                case "num":
                    // Check if number
                    int result;
                    bool numberTrue = int.TryParse(inp, out result); // Try to parse as int
                    dynamic mingood = null;
                    dynamic maxgood = null;

                    if (numberTrue == true)
                    {

                        if (max != null)
                        {
                            if (result < max)
                            {
                                maxgood = true;
                            }
                            else
                            {
                                maxgood = false;
                            }
                            
                        }
                        if (min != null)
                        {
                            if (result > min)
                            {
                                mingood = true;
                            }
                            else
                            {
                                mingood = false;
                            }
                            
                        }
                        if (mingood != false && maxgood != false)
                        {
                            goodInput=true;
                            return result;
                        }
                        
                    }

                    break;
        
                case "bool":
                    // Check if y or n
                    inp = inp.ToLower();

                    switch (inp)
                    {
                        case "y":
                            goodInput = true;
                            return true;

                        case "n":
                            goodInput = true;
                            return false;

                        default:
                            goodInput = false;
                            break;
                    }

                    break;

                default:
                    // No correct answer type given
                    Console.WriteLine("ERROR - Bad function call");
                    break;
                }
                
                if (goodInput == false)
                {
                    // Bad answer
                    Console.WriteLine(errorMsg);
                }
            
            }
            return true;
        }
    }
}

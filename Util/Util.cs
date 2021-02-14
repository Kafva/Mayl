using System;
using System.IO;
using System.Collections.Generic;

namespace Utils
{
   public class Util
   {
      public const string ACCOUNTS_FILE = "./secret/accounts.txt";
      public static List<string> getAccounts()
      {
         var accounts = new List<string>();  
         string line;

         if(File.Exists(ACCOUNTS_FILE))
         {
            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(ACCOUNTS_FILE);  
            while((line = file.ReadLine()) != null) 
            {  
               accounts.Add(line);
            }  
             
            file.Close();  
         }
         else { Console.Error.WriteLine("Missing path: " + ACCOUNTS_FILE); }

         return accounts; 
      }    
       
   }
}

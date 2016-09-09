using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sys = Cosmos.System;
using System.Runtime.InteropServices;
using System.IO;

namespace MyOS {
    public class Kernel : Sys.Kernel {
        protected override void BeforeRun() {
            Console.WriteLine("Cosmos booted successfully. Welcome to our OS.");
        }

        protected override void Run() {
            
                Console.Write("> ");
                 var input = Console.ReadLine();
                interpreter(input);
        }

        public void interpreter(string s) {
            string[] input = s.Split();
            string fileName = "";

            if (input[0].ToLower().Equals("shutdown", StringComparison.OrdinalIgnoreCase))
                Stop();
            else if (input[0] == "create" && input.Length == 2)
            {

                if (input[1].Substring(input[1].Length - 4, input[1].Length - 3) != ".bat")
                {
                    Console.WriteLine("Error: File extension not recognized. Try creating another file..");
                }
                else {
                    fileName = input[1];
                    createFile(fileName);
                }
            }
            else if (input[0] == "dir" && input.Length == 1)
                displayDirectory();
            else if (input[0] == "echo" && input.Length > 1)
            {
                if (input[1].Substring(0, 1) == "$") {          // echo value of variable
                    int varIndex = int.Parse(input[1].Substring(2,3));
                    Console.WriteLine(Globals.VARIABLES[varIndex]);
                }
                else {
                    string echoedText = "";
                    for (int i = 1; i < input.Length; i++)
                    {
                        echoedText += input[i] + " ";
                    }
                    Console.WriteLine(echoedText);
                }
            }
            else if (input[0] == "reboot" && input.Length == 1)
            {
                Cosmos.System.Power.Reboot();
            }
            else if (input[0] == "set" && input.Length == 3)
            {
                set(input[1], input[2]);
            }
            else if (input[0] == "add" && input.Length == 4)
            {
                add(input[1], input[2], input[3]);
            }
            else if (input[0] == "sub" && input.Length == 4)
            {
                subtract(input[1], input[2], input[3]);
            }
            else if (input[0] == "mul" && input.Length == 4)
            {
                multiply(input[1], input[2], input[3]);
            }
            else if (input[0] == "div" && input.Length == 4)
            {
                divide(input[1], input[2], input[3]);
            }
            else if (input[0] == "run" && input.Length == 2 && input[1].Substring(input[1].Length-4, input[1].Length) == ".bat")
            {
                run(input[1]);
            }
            else if (input[0] == "runall" && input.Length >= 2)
            {
                runall(input);
            }
            else if (input[0] == "help")
            {
                help();
            }
            else
            {
                Console.WriteLine("Error:");
                Console.WriteLine("Could not interpret user input: '" + s + "'");
                
            }
        }

        public class Globals {
            
            public static ArrayList FILELIST = new ArrayList();
            public static int fileCount;
            public static Boolean creatingAFile;
            public static int[] VARIABLES = initializeVars();
        }

        public static int[] initializeVars()
        {
            int[] result = new int[10];
            for (int i = 0; i < result.Length; i++) 
            {
                result[i] = 0;
            }
            return result;
        }

        public void createFile(string fileName) {
            var fileContents = "";
            var textLine = "";

            Globals.creatingAFile = true;
            while (Globals.creatingAFile) {
                Console.Write("~ ");
                textLine = Console.ReadLine();
                if (textLine.ToLower().Equals("save", StringComparison.OrdinalIgnoreCase))
                    Globals.creatingAFile = false;
                else fileContents += "\n" + textLine;          
            }

            // Store fileName and file contents of incoming file in file list
            Globals.FILELIST.Add(fileName);
            Globals.FILELIST.Add(fileContents);
            
            Globals.fileCount++;
            Console.WriteLine("File count: " + Globals.fileCount);
            Console.WriteLine("File create: " + Globals.FILELIST[Globals.fileCount - 1]);
        }

        public static void displayDirectory() {
            // File list empty
            if (Globals.fileCount == 0)
                Console.WriteLine("Directory is empty");
            else { // Display file list 
                string fileName = "";
                string fileContents = "";
                string fileDate = "";
                string fileSize = "";
                Console.WriteLine("FILENAME     EXTENSION     CREATED       FILESIZE");
                for (int i = 0; i < Globals.FILELIST.Count; i+=2)
                {
                    fileName = Globals.FILELIST[i].ToString();
                    fileContents = Globals.FILELIST[i + 1].ToString();
                    fileSize = fileContents.Length * sizeof(Char) + " Bytes";
                    Console.WriteLine(fileName + "       " + fileName.Substring(fileName.Length-4) + "      3/07/2016     " + fileSize);
                }
            }
        }

        public void set(string variable, string value)
        {
            int varIndex = int.Parse(variable.Substring(1));
            int valIndex, amount;

            // Set variable equal to value of another variable
            if (value.Substring(0, 1) == "v" || value.Substring(0, 1) == "V")
            {
                valIndex = int.Parse(value.Substring(1, 2));
                amount = Globals.VARIABLES[valIndex];
                Globals.VARIABLES[varIndex] = amount;
            }
            else // Set variable to integer value
            {
                Globals.VARIABLES[varIndex] = int.Parse(value);
            }
        }

        public void add(string varInt_1, string varInt_2, string variable)
        {
            int varIndex = int.Parse(variable.Substring(1, 2));
            int varIntIndex_1, varIntIndex_2, amount_1, amount_2, result = 0;

            if (varInt_1.Substring(0,1) == "v" || varInt_2.Substring(0,1) == "V")
            {
                varIntIndex_1 = int.Parse(varInt_1.Substring(1,2));
                amount_1 = Globals.VARIABLES[varIntIndex_1];
            }
            else
            {
                amount_1 = int.Parse(varInt_1);
            }
            if (varInt_2.Substring(0,1) == "v" || varInt_2.Substring(0,1) == "V" )
            {
                varIntIndex_2 = int.Parse(varInt_2.Substring(1, 2));
                amount_2 = Globals.VARIABLES[varIntIndex_2];
            }
            else
            {
                amount_2 = int.Parse(varInt_2);
            }

            result = amount_1 + amount_2;
            set(variable,result + "");
        }

        public void subtract(string varInt_1, string varInt_2, string variable)
        {
            int varIndex = int.Parse(variable.Substring(1, 2));
            int varIntIndex_1, varIntIndex_2, amount_1, amount_2, result = 0;

            if (varInt_1.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_1 = int.Parse(varInt_1.Substring(1, 2));
                amount_1 = Globals.VARIABLES[varIntIndex_1];
            }
            else
            {
                amount_1 = int.Parse(varInt_1);
            }
            if (varInt_2.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_2 = int.Parse(varInt_2.Substring(1, 2));
                amount_2 = Globals.VARIABLES[varIntIndex_2];
            }
            else
            {
                amount_2 = int.Parse(varInt_2);
            }

            result = amount_1 - amount_2;
            set(variable, result + "");
        }

        public void multiply(string varInt_1, string varInt_2, string variable)
        {
            int varIndex = int.Parse(variable.Substring(1, 2));
            int varIntIndex_1, varIntIndex_2, amount_1, amount_2, result = 0;

            if (varInt_1.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_1 = int.Parse(varInt_1.Substring(1, 2));
                amount_1 = Globals.VARIABLES[varIntIndex_1];
            }
            else
            {
                amount_1 = int.Parse(varInt_1);
            }
            if (varInt_2.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_2 = int.Parse(varInt_2.Substring(1, 2));
                amount_2 = Globals.VARIABLES[varIntIndex_2];
            }
            else
            {
                amount_2 = int.Parse(varInt_2);
            }

            result = amount_1 * amount_2;
            set(variable, result + "");
        }

        public void divide(string varInt_1, string varInt_2, string variable)
        {
            int varIndex = int.Parse(variable.Substring(1, 2));
            int varIntIndex_1, varIntIndex_2, amount_1, amount_2, result = 0;
            
            if (varInt_1.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_1 = int.Parse(varInt_1.Substring(1, 2));
                amount_1 = Globals.VARIABLES[varIntIndex_1];
            }
            else
            {
                amount_1 = int.Parse(varInt_1);
            }
            if (varInt_2.Substring(0, 1) == "v" || varInt_2.Substring(0, 1) == "V")
            {
                varIntIndex_2 = int.Parse(varInt_2.Substring(1, 2));
                amount_2 = Globals.VARIABLES[varIntIndex_2];
            }
            else
            {
                amount_2 = int.Parse(varInt_2);
            }

            result = amount_1 / amount_2;
            set(variable, result + "");
        }

        public void run(string fileName)
        {
            int indexOfFile = -1;
            for (int i = 0; i < Globals.FILELIST.Count; i++)
                if (String.Equals(Globals.FILELIST[i].ToString(), fileName))
                    indexOfFile = i;
            string[] lines = Globals.FILELIST[indexOfFile + 1].ToString().Split('\n');
            for ( int i = 1; i < lines.Length; i++ )
                interpreter(lines[i]);
            
        }

        public void runall(string[] s)
        {
            for ( int i = 1; i < s.Length; i++ )
                run(s[i]);
        }

        public void help()
        {
            Console.WriteLine("*** Help ***");
        }
    }
}
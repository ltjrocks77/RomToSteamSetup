using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomToSteamSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFolder = @"E:\SourceCode\RomToSteamSetup\RomToSteamSetup\bin\Debug\Test Folders\Source\";
            var targetFolder = @"E:\SourceCode\RomToSteamSetup\RomToSteamSetup\bin\Debug\Test Folders\Final\";
            var rocketLauncherLocation = @"F:\Hyperlaunch\RocketLauncherUI";
            var systemName = "Gamecube";

            var gamesList = Directory.GetFiles(sourceFolder);

            foreach (var game in gamesList)
            {
                var gameName = game.Remove(0, sourceFolder.Length);
                gameName = gameName.Remove(gameName.Length - 4);
                var arguments = string.Format("\"{0}\" -s {1} -r {2} -f RocketLauncherUI -p RocketLauncherUI", rocketLauncherLocation, systemName, gameName);

                var finalFile = Path.Combine(targetFolder, gameName) + ".bat";
                StreamWriter streamWriter = new StreamWriter(finalFile);
                streamWriter.WriteLine(arguments);
                streamWriter.Close();
            }
            Console.WriteLine("Finished Writing Bat Files hit a key to make EXEs");
            //Console.ReadKey();

            var batList = Directory.GetFiles(targetFolder,"*.bat");

            foreach (var bat in batList)
            {
                var gameName = bat.Remove(0, targetFolder.Length);
                gameName = gameName.Remove(gameName.Length - 4) + ".exe";
                var finalLocation = Path.Combine(targetFolder,"exes", gameName);
                CompileBatToExe(bat, finalLocation);
            }

            Console.WriteLine("Finished Writing EXE Files hit a key to exit");
            Console.ReadKey();
        }

        public static void DownloadImage(string gameName)
        {
            //intital post

            //urlparams
            //page = post
            //s = list
            // tags = gameName

            // search for id based on a regex s=view&amp;id=127895 where 127895 is the image id

            /*
            * page = post
            * s = view
             *   id = 12878
            */

            // grab image based on png

            //save as gameName.png
        }

        public static void CompileBatToExe(string batchFile, string outputExe)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters comParms = new CompilerParameters();

            comParms.GenerateExecutable = true;
            comParms.GenerateInMemory = false;
            comParms.IncludeDebugInformation = false;

            comParms.MainClass = "GenericConsole.Program";
            comParms.CompilerOptions = "/optimize";
            comParms.OutputAssembly = outputExe;
            comParms.TreatWarningsAsErrors = false;

            comParms.ReferencedAssemblies.AddRange(new string[] { "mscorlib.dll", "System.dll", "System.Data.dll", "System.Xml.dll" });

            //Update template source code to reflect batch file contents
            string source = CreateSourceCode(batchFile);
            //Perform actual compilation
            CompilerResults comRes = compiler.CompileAssemblyFromSource(comParms, source);
        }

        private static string CreateSourceCode(string batchFile)
        {
            StringBuilder sourceCode = new StringBuilder(GetGenericSource());
            string batchContent = File.ReadAllText(batchFile);
            //Command Line instructions often contain double quotes -> " <- which prematurely terminates string assignment
            //resulting in compilation errors. As a work around, replace all double quotes with $&$ before compiling. 
            //Before executing command line script apply the same logic in reverse, replace all instances of $&$ with "
            batchContent = batchContent.Replace("\"", "$&$");

            sourceCode.Replace("batchFileContents = \"\";", "batchFileContents = " + "@" + "\"" + batchContent + "\";");
            sourceCode.Replace("redirectStandardOutput = true", "redirectStandardOutput = false");


            return sourceCode.ToString();
        }

        /// <summary>
        /// Retrieving source code template, pay special attention!
        /// </summary>
        /// <returns></returns>
        private static string GetGenericSource()
        {
            //The source code template used in run-time compilation forms part of this Visual Studio Solution: GenericConsole.cs
            //GenericConsole.cs has been added as an embedded resource to this project, with Persistence configured as:
            // "Linked at compile time" -- The result being any changes made to the template file aka GenericConsole.cs will 
            //always update the embedded resource

            return RomToSteamSetup.Properties.Resources.GenericConsole;
        }
    }
}

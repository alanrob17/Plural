// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateTags.cs" company="Software Inc.">
//   A.Robson
// </copyright>
// <summary>
//   Create batch files to rename Pluralsight content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Plural
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Create tags.
    /// </summary>
    public class CreateTags
    {
        public static int Main(string[] args)
        {
            // Bring in the Plural web page
            var argList = GetArguments(args);

            var directory = Environment.CurrentDirectory + @"\";

            // save the page from the web
            var homePage = GetPage(argList.FileName);
            CreatePage(homePage);

            var page = new List<string>();

            page = GetPageContent(directory + "\\webpage.txt");

            // Note - these need to be numbered properly
            var items = new List<Item>();

            var modules = CreateModuleList(page, items);

            modules = ReNumberModules(modules);

            CreateModuleCmd(modules);

            ReNumberItems(items);

            PrintItemList(items);

            return 0;
        }

        /// <summary>
        /// Print the item list.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void PrintItemList(IEnumerable<Item> items)
        {
            var outFile = Environment.CurrentDirectory + "\\alan.txt";
            var outStream = File.Create(outFile);
            var sw = new StreamWriter(outStream);

            foreach (var item in items)
            {
                sw.WriteLine("{0} - {1}: {2} - {3}", item.ModuleId, item.Module, item.ItemId, item.Name);
            }

            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Number the item list.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void ReNumberItems(IEnumerable<Item> items)
        {
            var currentModule = string.Empty;
            var moduleCount = 0;
            var itemCount = 0;

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(currentModule) || item.Module != currentModule)
                {
                    ++moduleCount;
                    itemCount = 1;
                    currentModule = item.Module;

                    RenameModule(item, moduleCount);
                    RenameItem(item, itemCount);
                }
                else
                {
                    ++itemCount;
                    RenameModule(item, moduleCount);
                    RenameItem(item, itemCount);
                }
            }
        }

        /// <summary>
        /// Rename and number item.
        /// </summary>
        /// <param name="item">The item.
        /// </param>
        /// <param name="itemCount">The item count.</param>
        private static void RenameItem(Item item, int itemCount)
        {
            var stringNumber = string.Empty;

            item.Name = CleanModuleName(item.Name);

            if ((itemCount) < 10)
            {
                stringNumber = "0" + itemCount;
            }
            else
            {
                stringNumber = itemCount.ToString(CultureInfo.InvariantCulture);
            }

            item.ItemId = itemCount;
            item.Name = stringNumber + "-" + item.Name;
        }

        /// <summary>
        /// Rename module with the correct module number.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="moduleNumber">The module Number.</param>
        private static void RenameModule(Item item, int moduleNumber)
        {
            var moduleNumberString = string.Empty;

            if (moduleNumber < 10)
            {
                moduleNumberString = "0" + moduleNumber.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                moduleNumberString = moduleNumber.ToString(CultureInfo.InvariantCulture);
            }
            var newModuleName = string.Empty;
            var newModuleId = 0;

            item.ModuleId = moduleNumber;
            newModuleName = item.Module;
            newModuleName = CleanModuleName(newModuleName);
            item.Module = moduleNumberString + "-" + newModuleName;
        }

        /// <summary>
        /// Renumber all modules.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <returns>The <see cref="IEnumerable"/>renumbered module list.</returns>
        private static IEnumerable<string> ReNumberModules(IEnumerable<string> modules)
        {
            var newModules = new List<string>();
            var count = 0;
            var stringNumber = string.Empty;

            foreach (var module in modules)
            {
                ++count;

                if (count < 10)
                {
                    stringNumber = "0" + count;
                }
                else
                {
                    stringNumber = count.ToString(CultureInfo.InvariantCulture);
                }

                var newModule = string.Empty;

                newModule = stringNumber + "-" + module;
                newModules.Add(newModule);
            }

            return newModules;
        }

        /// <summary>
        /// Create a list of modules.
        /// </summary>
        /// <param name="page">The page content.</param>
        /// <param name="items">The items.</param>
        /// <returns>The <see cref="IEnumerable"/>module list.</returns>
        private static IEnumerable<string> CreateModuleList(IEnumerable<string> page, List<Item> items)
        {
            var modules = new List<string>();
            var inModuleHeader = false; // note that we are are within a module header tag.
            var moduleName = string.Empty;
            var currentModuleName = string.Empty;

            foreach (var line in page)
            {
                var newLine = line.Trim();
                if (newLine.Contains("class=\"accordion-title__title\""))
                {
                    // this is where the directory name is stored
                    inModuleHeader = true;
                    moduleName = newLine;
                }

                if (inModuleHeader)
                {
                    moduleName = StripHtml(moduleName);
                    currentModuleName = moduleName;
                    modules.Add(moduleName);
                    inModuleHeader = false;
                }

                if (inModuleHeader)
                {
                    moduleName += newLine;
                }

                if (newLine.Contains("class=\"accordion-content__row__title\""))
                {
                    // this is an item
                    var item = new Item { Module = currentModuleName, Name = StripHtml(newLine) };
                    items.Add(item);
                }
            }

            return modules;
        }

        /// <summary>
        /// Get content from the page to build batch files.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="List"/>page content for LinkedIn batch files.</returns>
        private static List<string> GetPageContent(string fileName)
        {
            var page = new List<string>();

            if (!File.Exists(fileName))
            {
                Console.WriteLine("\n\n***ERROR Missing file \n{0}", fileName);
            }

            // make sure you write back any UTF8 characters
            var sr = new StreamReader(fileName);

            while (sr.Peek() > -1)
            {
                var s = sr.ReadLine();

                if (s.Trim().Length > 0)
                {
                    page.Add(s);
                }
            }

            return page;
        }

        /// <summary>
        /// Strip html from text.
        /// </summary>
        /// <param name="input">The input line.</param>
        /// <returns>The <see cref="string"/>clean text.</returns>
        private static string StripHtml(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, "<.*?>", string.Empty);
            }

            return input.Trim();
        }

        /// <summary>
        /// Create a module batch file.
        /// </summary>
        /// <param name="modules">The modules.</param>
        private static void CreateModuleCmd(IEnumerable<string> modules)
        {
            var outFile = Environment.CurrentDirectory + "\\alan.cmd";
            var outStream = File.Create(outFile);
            var sw = new StreamWriter(outStream);

            foreach (var module in modules)
            {
                sw.WriteLine("md \"{0}\"", module);
            }

            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Get command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="bool"/>subfolder status.</returns>
        private static ArgList GetArguments(IList<string> args)
        {
            var fileName = string.Empty;
            if (args.Count == 1)
            {
                fileName = args[0];
                var argList = new ArgList(fileName);
                return argList;
            }
            else
            {
                var arglist = new ArgList(string.Empty);

                return arglist;
            }
        }

        /// <summary>
        /// Clean the module name of dodgy characters.
        /// </summary>
        /// <param name="moduleName">The new module name.</param>
        /// <returns>The <see cref="string"/>cleaned module name.</returns>
        public static string CleanModuleName(string moduleName)
        {
            var module = Regex.Replace(moduleName, @"&quot;|['"",&?%\.*:#/\\-]", " ").Trim();

            return module;
        }

        /// <summary>
        /// Find the first alpha position.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="int"/>position of the first alpha character.</returns>
        private static int FindAlpha(string fileName)
        {
            var iposn = 0;

            // loop through each character until you find the first Alpha
            for (var i = 0; i < fileName.Length; i++)
            {
                if (char.IsLetter(fileName[i]))
                {
                    iposn = i;
                    break;
                }
            }

            return iposn;
        }

        /// <summary>
        /// Get a page.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="string"/>page content.</returns>
        private static string GetPage(string url)
        {
            var client = new WebClient();
            
            client.Headers.Add("user-agent", "ASP.NET WebClient");
            
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            var data = client.OpenRead(url);
            
            var reader = new StreamReader(data);
            
            var page = reader.ReadToEnd();

            if (data != null)
            {
                data.Close();
            }
        
            reader.Close();
            
            return page;
        }

        /// <summary>
        /// Create a page from the web content.
        /// </summary>
        /// <param name="page">The page.</param>
        private static void CreatePage(string page)
        {
            var outFile = string.Format("webpage.txt");

            // create new files to work with
            var outStream = File.Create(outFile);
            
            // use StreamWriters to write data to files
            var sw = new StreamWriter(outStream);
            sw.WriteLine(page);
            
            // flush and close
            sw.Flush();
            sw.Close();
        }
    }
}

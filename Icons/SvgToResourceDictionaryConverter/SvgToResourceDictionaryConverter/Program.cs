using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Diagnostics = System.Diagnostics;

internal class Program
{
    private const int THD_COUNT = 18;

    private static Thread? _mainThread;
    private static int _totalFiles;
    private static int _processedFiles;
    private static object _lock = new object();

    private static void Main(string[] args)
    {
        if (RunConversion(args))
        {
            if (BuildDictionary(args.First()))
            {
                Console.WriteLine("Dictionary created.");
                Console.Read();
                return;
            }
        }
        Console.WriteLine("Process failed.");
        Console.Read();

    }

    private static bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write(question);
            var input = Console.ReadLine()?.ToUpper();
            if (input == "YES")
            {
                return true;
            }
            else if (input == "NO")
            {
                return false;
            }
        }
    }

    private static bool AskInput(string prompt, out string? userInput, Func<string?, bool> repeatCondition)
    {
        Console.Write(prompt);
        userInput = Console.ReadLine();
        if (repeatCondition(userInput))
        {
            return AskInput(prompt, out userInput, repeatCondition);
        }
        else
        {
            return true;
        }
    }


    private static bool BuildDictionary(string root)
    {
        string? dictionaryPrefix;
        do
        {
            if (AskInput("Please enter a resource key for the items in this resource dictionary: ", out dictionaryPrefix, (input) => string.IsNullOrWhiteSpace(input)) &&
                AskYesNo($"Are you sure you want the prefix to be {dictionaryPrefix}? (Yes/No): "))
            {
                break;
            }
            else
            {
                continue;
            }
        } while (true);

        var fileName = GetFileNameFromPath(root);
        if (fileName == null)
        {
            AskInput("Please enter a filename: ", out fileName, (input) =>
            {
                if (input == null) return false;
                var target = GetXamlFilePath(root, input);
                if (File.Exists(target))
                {
                    if (AskYesNo("Do you want to overwrite the file? (Yes/No)") == true)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                return false;
            });
        }

        var target = GetXamlFilePath(root, fileName);
        var sw = new StringBuilder();

        sw.Append("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" mc:Ignorable=\"d\">\r\n");


        foreach (var file in Directory.EnumerateFiles(root, "*.xaml", SearchOption.AllDirectories))
        {
            Console.WriteLine($"Appending -> {Path.GetFileName(file)}");
            sw.AppendLine(LoadXamlImage(file, dictionaryPrefix));
        }
        sw.Append("\r\n</ResourceDictionary>\r\n");
        if (File.Exists(fileName))
        {
            using (var fileHandle = File.OpenWrite(target))
            {
                fileHandle.SetLength(0);
            }
        }
        Console.WriteLine("Writing file...");
        File.WriteAllText(target, sw.ToString());
        Console.WriteLine("Writing to file completed.");
        return true;
    }

    private static string LoadXamlImage(string fileName, string keyRoot)
    {
        var xml = File.ReadAllText(fileName);



        xml = xml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
       // xml = xml.Replace($"xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "");
        var regexName = new Regex("Name=\"(?<NameTag>.+?)\"");
        xml = regexName.Replace(xml, "");
        var regexCanvasTag = new Regex("(?<CanvasTag><Canvas\\s+)");
        xml = regexCanvasTag.Replace(xml, $"<Canvas x:Key=\"{keyRoot}.{Path.GetFileNameWithoutExtension(fileName)}\" x:Shared=\"False\" ");
        var regexUnkownTag = new Regex("\\<!--\\s?Unknown tag:.+?-->");
        xml = regexUnkownTag.Replace(xml, "");


        var xDoc = XDocument.Parse(xml);
        XNode vb = xDoc.Element(XName.Get("Viewbox", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
        if (vb == null)
        {
            vb = xDoc.Element(XName.Get("Canvas", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
        }
        else
        {
            vb = ((XElement)vb).Element(XName.Get("Canvas", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
        }
        xml =  vb.ToString();
        xml = xml.Replace("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "");
        xml = xml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "");
        return xml;

    }

    private static string GetXamlFilePath(string root, string fileName)
    {
        if (Path.GetExtension(fileName)?.TrimStart('.').ToLower() != "xaml")
        {
            fileName += ".xaml";
        }

        return Path.Combine(root, fileName);
    }

    private static string? GetFileNameFromPath(string path)
    {
        path = Path.GetFullPath(path);
        string? fileName;
        if (path == Path.GetPathRoot(path))
        {
            Console.WriteLine("path is already root.");
            return null;
        }
        else
        {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            var parent = Path.GetFullPath($"{path}\\..");
            parent = parent.TrimEnd(Path.DirectorySeparatorChar);

            fileName = path.Substring(parent.Length + 1);

            fileName = Path.ChangeExtension(fileName, "xaml");
            Console.WriteLine(fileName);
            return fileName;

        }
    }


    private static bool RunConversion(string[] args)
    {
        var root = args.FirstOrDefault();
        if (args.Length == 0 || !Directory.Exists(root))
        {
            Console.WriteLine("Please start this program with a valid directory.");
            Console.WriteLine("Press Enter to exit.");
            Console.Read();
            return false;
        }

        _mainThread = SpawnThread(() => EnumerateFilesAndStartThreads(root));


        while (_mainThread.ThreadState ==  ThreadState.Running || _mainThread.ThreadState == ThreadState.WaitSleepJoin)
        {
            Thread.Sleep(1000);
        }

        _mainThread = null;

        var success = false;
        _mainThread = SpawnThread(() => success = Checkup(root));
        while (_mainThread.ThreadState == ThreadState.Running)
        {
            Thread.Sleep(1000);
        }

        return success;

    }

    private static bool ExecuteCmdCommand(string command)
    {
        Diagnostics.Process cmd = new Diagnostics.Process();
        cmd.StartInfo.FileName = "inkscape.com";// "cmd.exe";
        cmd.StartInfo.Arguments = command;
        //cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.RedirectStandardError = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();

        //cmd.StandardInput.WriteLine(command);
        //cmd.StandardInput.Flush();
        //cmd.StandardInput.Close();
        Diagnostics.Stopwatch stw = new Diagnostics.Stopwatch();
        int timeout = 10000;
        stw.Start();
        while (!cmd.HasExited && stw.ElapsedMilliseconds < timeout)
        {
            Thread.Sleep(100);
        }

        stw.Stop();
        if (stw.ElapsedMilliseconds >= timeout)
        {
            Console.WriteLine("TIMEOUT", "Timeout for process exceeded.");
        }


        if (cmd.HasExited)
        {
            if (cmd.ExitCode != 0)
            {
                Console.WriteLine($"FAIL: {command}");
                return false;
            }
            return true;

        }
        else
        {
            cmd.Kill(true);
            return false;
        }
    }

    private static bool Checkup(string root)
    {
        var files = GetSvgFiles(root).Where(f => !File.Exists(Path.ChangeExtension(f, "xaml")));
        if (files.Any())
        {
            foreach (var file in files)
            {
                Console.WriteLine("ERR: " + file);
            }
            return false;
        }
        return true;

    }

    private static IEnumerable<string> GetSvgFiles(string root)
    {
        return Directory.EnumerateFiles(root, "*.svg", SearchOption.AllDirectories)
                            .Where(f => !File.Exists(Path.ChangeExtension(f, "xaml")));
    }
    private static void EnumerateFilesAndStartThreads(string root)
    {
        var files = GetSvgFiles(root);
        _totalFiles = files.Count();
        Console.WriteLine($"Files to convert {_totalFiles}.");

        var blockSize = files.Count() / THD_COUNT;
        var rest = files.Count() % THD_COUNT;
        var workers = new List<Thread>();
        // Spawn all threads if the file count / threads gives no rest, otherwise handle
        // the last block after this for loop
        if (blockSize > 0)
        {
            for (int i = 0; i < THD_COUNT - (rest > 0 ? 1 : 0); i++)
            {
                workers.Add(SpawnThread(() =>
                                ConvertSvgBlock(files
                                     .Skip(blockSize * i)
                                     .Take(blockSize).ToList())));
            }
        }
        if (rest > 0)
        {
            workers.Add(SpawnThread(() =>
                            ConvertSvgBlock(files
                                .Skip(blockSize * THD_COUNT)
                                .Take(blockSize + rest).ToList())));
        }
        foreach (var thd in workers)
        {
            thd.Join();

        }
    }

    private static void ConvertSvgBlock(List<string> files)
    {
        Console.WriteLine("Thrad " + Thread.CurrentThread.ManagedThreadId + ": " + files.Count);
        foreach (var src in files)
        {
            var dst = Path.ChangeExtension(src, "xaml");
            var result = ExecuteCmdCommand($"\"{src}\" -o \"{dst}\"");
            lock (_lock)
            {
                _processedFiles++;
            }

            UpdateScreen(src);
        }
    }

    private static void UpdateScreen(string lastFile)
    {
        int current = 0;
        lock (_lock)
        {
            current = _processedFiles;
        }

        Console.WriteLine($"{current} of {_totalFiles} {(double)current / _totalFiles:p2} -> {Path.GetFileName(lastFile)}");
    }

    private static Thread SpawnThread(Action action)
    {
        var thread = new Thread(new ThreadStart(action));
        thread.Start();
        return thread;

    }
}
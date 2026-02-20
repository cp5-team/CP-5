using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public class CP5
{

    /*
     Yeah! CP-5 Working at one switch ;)
        Memory: 4096 bytes
        pointer start: 0 (you can edit this, but you need to rewrite your program :/ )
        syntax guide at Program.cs (in help block)
    Now you can... just read this or edit ^^
     */
    public static byte[] memory = new byte[4096];
    public static int p = 0;
    private Dictionary<string, int> labels = new Dictionary<string, int>();

    public void SetMemSize(int mem)
    {
        Array.Resize(ref memory, mem);
        if (p >= memory.Length) p = memory.Length - 1;
    }

    public void SetPointer(int startP)
    {
        if (startP >= 0 && startP < memory.Length)
        {
            p = startP;
        }
        else
        {
            throw new Exception($"CP5 Init Error: Invalid starting pointer {startP} (Memory size is {memory.Length})");
        }
    }

    public void Prog(string programPath)
    {
        if (!File.Exists(programPath)) return;
        var lines = File.ReadAllLines(programPath);
        var cleanLines = lines.Select(line =>
        {
            int idx = line.IndexOf("//");
            if (idx >= 0) line = line.Substring(0, idx);
            idx = line.IndexOf("!!");
            if (idx >= 0) line = line.Substring(0, idx);
            return line;
        });
        string content = string.Join(" ", cleanLines).Replace(":", " : ");
        string[] commands = content.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        labels.Clear();
        for (int j = 0; j < commands.Length; j++)
        {
            if (commands[j] == ":" && j + 1 < commands.Length)
            {
                labels[commands[j + 1]] = j;
                j++;
            }
            else if (commands[j].StartsWith(":") && commands[j].Length > 1)
            {
                labels[commands[j].Substring(1)] = j;
            }
        }
        for (int i = 0; i < commands.Length; i++)
        {
            string cmd = commands[i];
            if (cmd == ":") { i++; continue; }
            if (cmd.StartsWith(":") && labels.ContainsKey(cmd.Substring(1))) continue;

            switch (cmd)
            {
                case "0": Console.ReadKey(true); break;
                case "1":
                    Console.Write((char)memory[p]);
                    p = (p < memory.Length - 1) ? p + 1 : 0;
                    break;
                case "2": if (p < memory.Length - 1) memory[p] = memory[p + 1]; break;
                case "3": memory[p] = 0; break;
                case "4": case "7": memory[p]++; break;
                case "5": memory[p] = 255; break;
                case "6": case "8": memory[p]--; break;
                case "9": if (memory[p] == 0) i++; break;
                case "A": if (memory[p] != 0 && p > 0) p--; break;
                case "B": if (p < memory.Length - 1) p++; break;
                case "C": if (p > 0) p--; break;
                case "D": p = memory[p]; break;
                case "E": p = memory.Length - 1; break;
                case "F": Console.Write(memory[p]); break;
                case "P": Console.Beep(1000, 50); break;
                case "?": Console.WriteLine($"\n[DEBUG] P:{p} Val:{memory[p]}"); break;
                case "+": memory[p] = (byte)(memory[p] + memory[p + 1]); break;
                case "-": memory[p] = (byte)(memory[p] - memory[p + 1]); break;
                case "*": memory[p] = (byte)(memory[p] * memory[p + 1]); break;

                case "IF":
                    int addr1 = int.Parse(commands[++i]);
                    string op = commands[++i];
                    int addr2 = int.Parse(commands[++i]);
                    if (i + 1 < commands.Length && commands[i + 1] == ":") i++;

                    bool condition = op switch
                    {
                        "EQL" or "EQLL" => memory[addr1] == memory[addr2],
                        "BG" => memory[addr1] > memory[addr2],
                        "BL" => memory[addr1] < memory[addr2],
                        _ => false
                    };

                    if (!condition)
                    {
                        while (i + 1 < commands.Length && !commands[i + 1].StartsWith("G") && commands[i + 1] != "!:") i++;
                    }
                    break;

                case "G":
                    i++;
                    if (i < commands.Length)
                    {
                        string target = commands[i].Replace("_", "");
                        if (labels.ContainsKey(target)) i = labels[target];
                        else if (int.TryParse(target, out int pos)) i = pos - 1;
                    }
                    break;

                case "!:": i = commands.Length; break;
                case "LK":
                    string input = Console.ReadLine() ?? "";
                    byte[] bytes = Encoding.UTF8.GetBytes(input);
                    for (int k = 0; k < bytes.Length && (p + k) < memory.Length; k++)
                        memory[p + k] = bytes[k];
                    break;
                case "CLS": Console.Clear(); break;
                case "CLM": Array.Clear(memory, 0, memory.Length); break;
                case "R": while (Console.ReadKey(true).KeyChar != (char)memory[p]) ; break;
                case "RF":
                    string rfPath = commands[++i];
                    if (File.Exists(rfPath)) Console.WriteLine(File.ReadAllText(rfPath));
                    break;
                case "SLP":
                    if (i + 1 < commands.Length && int.TryParse(commands[++i], out int ms))
                        Thread.Sleep(ms);
                    break;
                case "WF":
                    string wfFile = commands[++i];
                    string wfCtx = commands[++i];
                    File.WriteAllText(wfFile, wfCtx);
                    break;
                case "RFTM":
                    string rftmFile = commands[++i];
                    if (File.Exists(rftmFile))
                    {
                        byte[] b = File.ReadAllBytes(rftmFile);
                        for (int k = 0; k < b.Length && (p + k) < memory.Length; k++)
                            memory[p + k] = b[k];
                    }
                    break;
                case "COLOR":
                    if (int.TryParse(commands[++i], out int colorCode))
                        Console.ForegroundColor = (ConsoleColor)(colorCode % 16);
                    break;

                default:
                    if (cmd.StartsWith("G") && cmd.Length > 1)
                    {
                        string t = cmd.Substring(1).Replace("_", "");
                        if (labels.ContainsKey(t)) i = labels[t];
                        else if (int.TryParse(t, out int p2)) i = p2 - 1;
                    }
                    else if (cmd.StartsWith("V") && byte.TryParse(cmd.Substring(1), out byte v))
                    {
                        memory[p] = v;
                    }
                    else if (cmd.StartsWith("JP"))
                    {
                        string val = cmd.Substring(2).Trim();
                        if (string.IsNullOrEmpty(val)) val = commands[++i];
                        p = int.Parse(val);
                    }
                    else if (labels.ContainsKey(cmd.Replace("_", "")))
                    {
                        continue;
                    }
                    else if (cmd.StartsWith("RMEM")) RMem(cmd.Replace("RMEM", "").Trim());
                    else throw new Exception($"CP5 Syntax Error: {cmd} at {i}");
                    break;
            }
        }
    }

    private static void RMem(string com)
    {
        switch (com)
        {
            case "0": Console.ReadKey(true); break;
            case "1": Console.Write((char)memory[p]); p = (p < memory.Length - 1) ? p + 1 : 0; break;
            case "4": memory[p]++; break;
            case "6": memory[p]--; break;
            case "P": Console.Beep(1000, 50); break;
            case "?": Console.WriteLine($"\n[DEBUG] P:{p} Val:{memory[p]}"); break;
        }
    }
}
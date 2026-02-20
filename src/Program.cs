using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chipemu
{
    internal class Program
    {
        static CP5 cp5 = new CP5();
        static void Main(string[] args)
        {
            CP5 cp5 = new CP5();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: cp5 <filename> [-mem <size>] [-sp <start_p>] or cp5 --help");
                return;
            }

            if (args[0] == "--help")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"
============================================================
   CP-5 Emulator v1.2 | Syntax & Commands Guide
============================================================
 [Memory & Navigation]
  V[val]   : Set cell value (0-255). Example: V72
  JP [ptr] : Jump pointer to address. Example: JP 100
  B / C    : Move pointer Forward / Backward
  CLM      : Clear all memory (reset to 0)

 [I/O Operations]
  COLOR    : Set console text color. Example: COLOR Red
  1 / F    : Output current cell as CHAR / NUMBER
  LK       : Read line from keyboard into memory
  P        - Play Beep sound (1000Hz)
  CLS      - Clear console screen
  0        - Wait for any key (Pause)
  ?        - Show debug info (Pointer & Cell value)

 [Control Flow]
  : [label]: Define a jump point
  G [label]: Jump to label or line number
  IF [a] [op] [b] : [cmd]
             Ops: EQL (==), BG (>), BL (<)
  !:       : Emergency stop / Terminate

 [File System]
  WF [file] [txt] : Write text to file
  RFTM [file]     : Read binary file into memory
  RF [file]       : Display file content to console
============================================================
");
                Console.ResetColor();
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-mem" && i + 1 < args.Length)
                    {
                        if (int.TryParse(args[i + 1], out int size)) cp5.SetMemSize(size);
                    }
                    if (args[i] == "-sp" && i + 1 < args.Length)
                    {
                        if (int.TryParse(args[i + 1], out int startP)) cp5.SetPointer(startP);
                    }
                }

                try
                {
                    cp5.Prog(args[0]);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (ex.Message.Contains("CP5"))
                        Console.WriteLine($"CP5 Error: \n{ex.Message}");
                    else
                        Console.WriteLine($"Win32Exception:\n{ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}

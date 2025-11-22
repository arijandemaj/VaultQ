using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.CLI.Helpers
{
    internal static class InputHelper
    {
        public static char[] Input(string text, bool hide)
        {
            char[] passwordArrayTemp = new char[128];
            int length = 0;

            Console.Write(text);
            ConsoleKeyInfo consoleKeyInfo;
            while (true)
            {
                consoleKeyInfo = Console.ReadKey(intercept: true);

                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if(consoleKeyInfo.Key == ConsoleKey.Backspace)
                {
                    if (length > 0)
                    {
                        length--;
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    passwordArrayTemp[length++] = consoleKeyInfo.KeyChar;

                    if (hide == true)
                        Console.Write("*");
                    else
                        Console.Write(consoleKeyInfo.KeyChar);
                }

            }

            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = passwordArrayTemp[i];
            }

            Array.Clear(passwordArrayTemp, 0, passwordArrayTemp.Length);

            return password;
        }

 


    }
}

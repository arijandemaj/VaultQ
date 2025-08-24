using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.CLI.Helpers
{
    internal static class PasswordHelper
    {
        public static char[] PromptPassword(string text = "password: ")
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
                    Console.Write("*");
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

        public static bool ValidatePassword(string password)
        {
            if(password.Length < 6)
            {
                return false;
            }

            return true;
        }


    }
}

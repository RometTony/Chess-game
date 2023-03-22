using static System.Console;

namespace CheckersGame2;

     public class Menu
     {
          private int _selectedIndex;
          private string[] _options;
          private string _prompt;

          public Menu(string prompt, string[] options)
          {
               _prompt = prompt;
               _options = options;
               _selectedIndex = 0;
          }
          private void DisplayOptions()
          {
               WriteLine(_prompt);
               for (int i = 0; i < _options.Length; i++)
               {
                    string currentOption = _options[i];
                    string prefix;
                    if (i == _selectedIndex)
                    {
                         prefix = "@";
                         ForegroundColor = ConsoleColor.Black;
                         BackgroundColor = ConsoleColor.White;
                    }
                    else
                    {
                         prefix = "";
                         ForegroundColor = ConsoleColor.White;
                         BackgroundColor = ConsoleColor.Black;
                        
                    }
                    
                    WriteLine($"{prefix}<<{currentOption}>>");
               }
               ResetColor();
          }

          public int Run()
          {
               ConsoleKey keyPressed;
               do
               {
                    Clear();
                    DisplayOptions();

                    ConsoleKeyInfo keyInfo = ReadKey(true);
                    keyPressed = keyInfo.Key;
                    //Update selected index based on arrow keys.
                    if (keyPressed == ConsoleKey.UpArrow)
                    {
                         _selectedIndex--;
                         if (_selectedIndex == -1)
                         {
                              _selectedIndex = _options.Length - 1;
                         }
                    }
                    else if (keyPressed == ConsoleKey.DownArrow)
                    {
                         _selectedIndex++;
                         if (_selectedIndex == _options.Length)
                         {
                              _selectedIndex = 0;
                         }
                    }
                    
               } while (keyPressed != ConsoleKey.Enter);
               
               return _selectedIndex;
          }
     }
     
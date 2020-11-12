using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Введите путь к .ini файлу");
            string path = Console.ReadLine();*/
            string debugdir = Directory.GetCurrentDirectory();
            string path = $"{debugdir}\\Пример_исходного_файла.ini";
            List<List<string>> reslist = new List<List<string>>();
            Regex r = new Regex("\\S|\\n");
            Regex section = new Regex(@"^(\n)?\[(\d|\w)+\]$");
            Regex parameter = new Regex(@"^(\n)?\w+\=(\w|\d|(\"".*\"")|(\'.*\'))+$");
            using (FileStream stream = new FileStream(path,FileMode.Open))
            {
                string curstr = "";
                while (true)
                {
                    var b = stream.ReadByte();
                    if (b == -1) break;
                    var letter = (char)b;
                    if (!r.IsMatch(letter.ToString())) continue;
                    if (letter == '\n' && curstr.Length > 0)
                    {
                        if (curstr[curstr.Length - 1] == ']')
                        {
                            if (!section.IsMatch(curstr))
                                throw new ArgumentException("Файл содержит строку с неверным форматом секции [SECTION_NAME]");
                             reslist.Add(new List<string> { curstr });
                        }
                        else if(curstr != "\n")
                        {
                            if (!parameter.IsMatch(curstr))
                                throw new ArgumentException("Файл содержит строку с неверным форматом параметра NAME=VALUE");
                            reslist[reslist.Count - 1].Add(curstr);
                        }
                        curstr = "";
                    }
                    curstr += letter;
                }
            }
            var resstr = string.Join("\n", reslist
                .OrderBy(x => x[0])
                .Select(list =>
                {
                    return list[0] + string.Join("",list
                        .Skip(1)
                        .OrderBy(x=>x));
                })
                .ToArray());
            using(FileStream stream = new FileStream(debugdir + "\\result.ini", FileMode.Create))
            {
                var resbyte = Encoding.UTF8.GetBytes(resstr);
                stream.Write(resbyte, 0, resbyte.Length);
            }
        }
    }
}

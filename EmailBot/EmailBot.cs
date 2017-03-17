using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailBot
{
    class EmailBot
    {
        static void Main(string[] args)
        {

            var filePath = AppDomain.CurrentDomain.BaseDirectory;

            while (filePath != null && filePath != "")
            {
                if(File.Exists(filePath + @"\EmailList.csv"))
                {
                    filePath = filePath + @"\EmailList.csv";
                    break;
                }
                filePath = Directory.GetParent(filePath)?.FullName;
            }

            if (!filePath.Contains("EmailList.csv"))
            {
                Console.WriteLine("EmailList.csv not found.");
            }

            var workbook = ReadWorkbook(filePath);

            var toEmailList = GetToEmailList(workbook);

        }


        static List<string> GetToEmailList(List<string> workbook)
        {
            var toEmailList = new List<string>();

            foreach(var line in workbook)
            {
                var lineList = line.Split(',').ToList();

                var date = lineList[2]; // Probably better way to do this than hard coding

                var timeDifference = DateTime.Now - DateTime.Parse(date);

                var derf = timeDifference.Days;

                if (timeDifference.Days == 0 || timeDifference.Days == 60) 
                {
                    Console.WriteLine(line);
                }
              
            }

            return toEmailList;

        }

        static List<string> ReadWorkbook(string filePath)
        {
            var workbook = new List<string>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                sr.ReadLine(); // Skip the column names line
                while ((line = sr.ReadLine()) != null)
                {
                    workbook.Add(line);
                }
            }

            return workbook;

        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailBot
{
    class EmailBot
    {
        static void Main(string[] args)
        {
            var filePath = GetDocumentFilePath("EmailList.csv");

            if (filePath.Length == 0 || filePath == null)
            {
                Console.WriteLine("EmailList.csv not found! Call Zak :)");
                return;
            }

            var workbook = ReadWorkbook(filePath);

            var toEmailList = GetToEmailList(workbook);

            SendEmails(toEmailList);

        }


        static void SendEmails(List<string> EmailData)
        {
            var filePath = GetDocumentFilePath("config.txt");

            if (filePath == null || filePath.Length == 0)
            {
                Console.WriteLine("config.txt not found! Call Zak :)");
                return;
            }


            var bodyFilePath = GetDocumentFilePath("body.txt");
            if (bodyFilePath == null || bodyFilePath.Length == 0)
            {
                Console.WriteLine("body.txt not found! Call Zak :)");
                return;
            }

            var configData = new StreamReader(filePath);

            var email = configData.ReadLine().Trim().Replace("Email:", "");
            var password = configData.ReadLine().Trim().Replace("Password:", "");

            var emailList = EmailData.Select(x => x.Split(',')[1]).ToList();


            string fromPassword = password;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                
            };
            var fromAddress = new MailAddress(email, "noreply@gmail.com");

            for(int i = 0; i < emailList.Count; i++)
            {
                var toAddress = new MailAddress(emailList[i], "Steven Fry");
                smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "It's time to order CPAP supplies!",
                    Body = GetEmailBody(bodyFilePath, EmailData),
                    IsBodyHtml = true,
                })
                {

                    smtp.Send(message);
                }
            }

        }

        static string GetEmailBody(string filePath, List<string> EmailData)
        {
            var body = File.ReadAllText(filePath);

            // Add some replaces in here

            return body;
        }
        

        static List<string> GetToEmailList(List<string> workbook)
        {
            // I really hope there's a better way to do something like this besides repeating it
            var toEmailList = workbook.Where(x => /*(DateTime.Now - DateTime.Parse(x.Split(',').ToList()[2])).Days == 0 ||*/ (DateTime.Now - DateTime.Parse(x.Split(',').ToList()[2])).Days == 60)
                               .Select(x => x).ToList();

            return toEmailList;

        }

        static string GetDocumentFilePath(string fileName)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory;

            while (filePath != null && filePath != "")
            {
                if (File.Exists(filePath + @"\" + fileName))
                {
                    filePath = filePath + @"\" + fileName;
                    break;
                }
                filePath = Directory.GetParent(filePath)?.FullName;
            }

            return filePath;
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

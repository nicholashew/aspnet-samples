using Microsoft.VisualStudio.TestTools.UnitTesting;
using SftpDataExport.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SftpDataExport.Tests
{
    [TestClass]
    public class CsvHelperTests
    {
        public static List<Person> Persons = new List<Person>
        {
            new Person { Name = "baby", Age = 1, Salary = 0, BirthDate = new DateTime(2018, 1, 1) },
            new Person { Name = "kid", Age = 5, Salary = 8.50, BirthDate = new DateTime(2013, 2, 3) },
            new Person { Name = "student", Age = 15, Salary = 350, BirthDate = new DateTime(2003, 11, 15) },
            new Person { Name = "adult", Age = 21, Salary = 1500, BirthDate = new DateTime(1997, 4, 1) },
            new Person { Name = "profesional", Age = 30, Salary = 3000,BirthDate = new DateTime(1988, 3, 12) }
        };

        [TestMethod]
        public async Task ReadCsvRecordsTestAsync()
        {
            var persons = await CsvUtil.GetRecordsAsync<Person>("Sample/persons.csv");
            Assert.AreEqual(persons?.Count, 5);
            Assert.AreEqual(persons?[0].Name, "baby");
            Assert.AreEqual(persons?[0].Age, 1);
            Assert.AreEqual(persons?[0].Salary, 0);
            Assert.AreEqual(persons?[0].BirthDate, new DateTime(2018, 1, 1));
        }

        [TestMethod]
        public async Task WriteCsvRecordsTestAsync()
        {
            FileHelper.CreateDirectory("temp");
            string tempPath = "temp/" + DateTime.Now.ToString("yyyyMMddhhmmss.fff") + ".csv";
            bool success = false;

            try
            {                
                await CsvUtil.WriteRecordsAsync(Persons, tempPath);
            }
            catch
            {
                success = false;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    success = true;
                    File.Delete(tempPath);
                }
            }

            Assert.IsTrue(success);
        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public double Salary { get; set; }
            public DateTime BirthDate { get; set; }
        }
    }
}

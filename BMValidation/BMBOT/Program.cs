using BMBOT.DataAccessLayer;
using BMBOT.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ServiceReference1;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Configuration;
using System.Threading.Tasks;

namespace BMBOT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to continue");
            DateTime runDate = DateTime.Now.Date;
            if (args != null && args.Length > 0 &&  DateTime.TryParse(args[0], out runDate))
            {
                runDate = runDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            Console.ReadLine();

            validateFilesFromLocation(runDate);

            using (ApplicationContext _application = new ApplicationContext())
            {
                var botprofiles = _application.BOTProfile.ToList();
                var botTemplates = _application.BOTTemplate.ToList();

                var bottempplatesProfileMapping = _application.BOTProfileTemplateMapping.ToList();
                foreach (var profile in botprofiles.Where(x => x.IsActive == "Y"))
                {
                    foreach (var template in bottempplatesProfileMapping.Where(x => x.ProfileId == profile.ProfileId))
                    {
                        var templateToBeProcessed = botTemplates.FirstOrDefault(x => x.TemplateId == template.TemplateId);
                        List<string> textFiles = new List<string>();
                        if (templateToBeProcessed.OutBoundFileLocation.EndsWith(@"\"))
                        {
                            DirectoryInfo dinfo = new DirectoryInfo(templateToBeProcessed.OutBoundFileLocation);
                            FileInfo[] Files = dinfo.GetFiles("*.txt");
                            foreach (FileInfo file in Files)
                            {
                                textFiles.Add(file.FullName);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(templateToBeProcessed.OutBoundFileLocation))
                        {
                            textFiles.Add(templateToBeProcessed.OutBoundFileLocation);
                        }
                        foreach (var file in textFiles)
                        {
                            ProcessRecords(templateToBeProcessed, file);
                        }

                    }

                }
            }
            Console.WriteLine("Process Completed");
            Console.ReadLine();

        }

        public static void validateFilesFromLocation(DateTime runDate)
        {
            using (ApplicationContext _application = new ApplicationContext())
            {

                // Validate if the file is moved from location.

                var botFileLocationConfiguration = _application.BOTFileLocationConfiguration.ToList();

                var pickedFileConfiguration = botFileLocationConfiguration.FirstOrDefault(x => x.ValidationType == "PICKED");

                FileCheckError fileNotPicked = new FileCheckError();
                fileNotPicked.FileObjects = new List<FileObject>();
                if (pickedFileConfiguration.FileLocation.EndsWith(@"\"))
                {
                    DirectoryInfo dPickedFileConfiguration = new DirectoryInfo(pickedFileConfiguration.FileLocation);
                    List<FileInfo> pickedFiles = dPickedFileConfiguration.GetFiles("*.txt").ToList();
                    fileNotPicked.RunDate = runDate.ToString();
                    foreach (var file in pickedFiles)
                    {
                        //TimeSpan? timespan = pickedFileConfiguration.ValidationTime;
                        DateTime timeToCompare = file.CreationTime;
                        if (runDate > timeToCompare)
                        {
                            FileObject errorFile = new FileObject();
                            errorFile.Date = timeToCompare.ToString();
                            errorFile.FileName = file.Name;
                            errorFile.Issue = "Not Transferred";
                            fileNotPicked.FileObjects.Add(errorFile);
                        }
                    }
                    fileNotPicked.FilesCount = fileNotPicked.FileObjects.Count().ToString();
                }

                FileCheckError fileErrorNotCreated = new FileCheckError();
                fileErrorNotCreated.FileObjects = new List<FileObject>();

                foreach (var fileconfiguration in botFileLocationConfiguration)
                {
                    if (fileconfiguration.FileLocation.EndsWith(@"\"))
                    {
                        DirectoryInfo dFileconfiguration = new DirectoryInfo(fileconfiguration.FileLocation);
                        List<FileInfo> files = dFileconfiguration.GetFiles("*.txt").ToList();
                        DateTime batchRunDate = DateTime.Now.Date;
                        TimeSpan? timespan = fileconfiguration.ValidationTime;
                        fileErrorNotCreated.RunDate = runDate.ToString();
                        switch (fileconfiguration.Frequency)
                        {
                            case "DAILY":
                                if (timespan.HasValue)
                                {
                                    batchRunDate = batchRunDate.Add(timespan.Value);
                                }
                                if (!files.Any(x => x.Name.Contains(fileconfiguration.FileNameStartsWith) && x.CreationTime > batchRunDate))
                                {
                                    FileObject fileNotCreated = new FileObject();
                                    fileNotCreated.Date = batchRunDate.ToString();
                                    fileNotCreated.FileName = fileconfiguration.FileNameStartsWith;
                                    fileNotCreated.Issue = "Not Created";
                                    fileErrorNotCreated.FileObjects.Add(fileNotCreated);
                                    //error
                                }
                                break;
                            case "WEEKLY":
                                //TimeSpan? timespan = fileconfiguration.ValidationTime;
                                if ((int)batchRunDate.DayOfWeek == fileconfiguration.DayOfTheWeek)
                                {
                                    if (timespan.HasValue)
                                    {
                                        batchRunDate = batchRunDate.Add(timespan.Value);
                                    }
                                    if (!files.Any(x => x.Name.Contains(fileconfiguration.FileNameStartsWith) && x.CreationTime > batchRunDate))
                                    {
                                        FileObject fileNotCreated = new FileObject();
                                        fileNotCreated.Date = batchRunDate.ToString();
                                        fileNotCreated.FileName = fileconfiguration.FileNameStartsWith;
                                        fileNotCreated.Issue = "Not Created";
                                        fileErrorNotCreated.FileObjects.Add(fileNotCreated);
                                    }
                                }
                                break;
                            case "MONTHLY":
                                //TimeSpan? timespan = fileconfiguration.ValidationTime;
                                if (fileconfiguration.ValidationDate.HasValue && batchRunDate == fileconfiguration.ValidationDate.Value.Date)
                                {
                                    if (timespan.HasValue)
                                    {
                                        batchRunDate = batchRunDate.Add(timespan.Value);
                                    }
                                    if (!files.Any(x => x.Name.Contains(fileconfiguration.FileNameStartsWith) && x.CreationTime > batchRunDate))
                                    {
                                        FileObject fileNotCreated = new FileObject();
                                        fileNotCreated.Date = batchRunDate.ToString();
                                        fileNotCreated.FileName = fileconfiguration.FileNameStartsWith;
                                        fileNotCreated.Issue = "Not Created";
                                        fileErrorNotCreated.FileObjects.Add(fileNotCreated);
                                    }
                                }
                                break;
                        }
                    }
                }

                fileErrorNotCreated.FilesCount = fileErrorNotCreated.FileObjects.Count().ToString();
                List<string> toaddress = new List<string>() { "rajatarora9@deloitte.com" };
                if (fileErrorNotCreated.FileObjects.Count > 0)
                {
                    string validationObj = ToXML(fileErrorNotCreated);
                    //call email api to send email for file not created
                    string path = Path.GetFullPath("C:/Users/rajatarora9/Desktop/BMBot/Benefit-Management-Bot/BMValidation/BMBOT/XSLTFormat/Transform1.xslt");
                    string xsltformat = File.ReadAllText(path);
                    sendEmail(toaddress, validationObj, "File Not Created", xsltformat);
                }
                if (fileNotPicked.FileObjects.Count > 0)
                {
                    string validationObj = ToXML(fileNotPicked);
                    //call email api to send email for file not picked
                    string path = Path.GetFullPath("C:/Users/rajatarora9/Desktop/BMBot/Benefit-Management-Bot/BMValidation/BMBOT/XSLTFormat/Transform1.xslt");
                    string xsltformat = File.ReadAllText(path);
                    sendEmail(toaddress, validationObj, "File Not Picked",xsltformat);
                }
            }
        }

        public static string ToXML(Object obj)
        {
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        private static void ProcessRecords(BOTTemplate template, string textFile)
        {

            List<Record> records = new List<Record>();
            List<BOTTemplateColumns> columns = new List<BOTTemplateColumns>();
            List<BOTTemplateValidationMapping> validationMapping = new List<BOTTemplateValidationMapping>();
            List<BOTTemplateValidationMapping> botTemplateValidationMappings = new List<BOTTemplateValidationMapping>();
            List<BOTTemplateValidationMapping> AllbotTemplateValidationMappings = new List<BOTTemplateValidationMapping>();
            int noOfRecordToBeParsed = 0;
            string fileName = Path.GetFileName(template.OutBoundFileLocation);
            using (ApplicationContext _application = new ApplicationContext())
            {
                columns = _application.BOTTemplateColumns.Where(x => x.TemplateId == template.TemplateId).ToList();
                validationMapping = _application.BOTTemplateValidationMapping.ToList();

            }
            Console.WriteLine("Reading File - " + fileName);
            if (!string.IsNullOrWhiteSpace(textFile))
            {
                var fileContent = File.ReadAllLines(textFile);
                //var fileContent = File.ReadAllLines(template.OutBoundFileLocation);
                int lineCount = fileContent.Length;
                noOfRecordToBeParsed = lineCount;
                int startLine = 1;
                startLine = startLine + (template.NoOfLineToBeExcludedFromTop.HasValue ? template.NoOfLineToBeExcludedFromTop.Value : 0);
                    if (template.NoOfLineToBeExcludedFromBottom.HasValue)
                    {
                        noOfRecordToBeParsed = noOfRecordToBeParsed - template.NoOfLineToBeExcludedFromBottom.Value;
                    }
                    if (lineCount > 0)
                    {
                        for (int i = startLine; i <= noOfRecordToBeParsed; i++)
                        {
                            Record record = new Record();
                            record.RecordId = i;
                            record.RecordText = fileContent[i - 1];
                            if (string.IsNullOrWhiteSpace(template.ExcludeLineStartText) || !record.RecordText.StartsWith(template.ExcludeLineStartText))
                            {
                                records.Add(record);
                            }
                        }
                    }
            }

            Console.WriteLine("Validating File - " + fileName);
            foreach (var record in records)
            {
                record.Columns = new List<Column>();
                record.Errors = new List<Error>();

                foreach (var column in columns)
                {
                    double tempValue = 0;
                    botTemplateValidationMappings = validationMapping.Where(x => !string.IsNullOrWhiteSpace(x.ColumnNames) && x.ColumnNames.Trim().Split(",").ToList().Contains(column.BOTTemplateColumnId.ToString())).ToList();
                    Column fileColumn = new Column();
                    fileColumn.TextValue = record.RecordText.Substring(column.MinPosition - 1, (column.MaxPosition - column.MinPosition) + 1);
                    if (column.IsIdentifier == "Y")
                    {
                        record.Identifier = fileColumn.TextValue;
                    }
                    fileColumn.ColumnName = column.ColumnName;
                    fileColumn.BOTTemplateColumnId = column.BOTTemplateColumnId;

                    if (column.DataType == "DOUBLE")
                    {
                        double.TryParse(fileColumn.TextValue, out tempValue);
                    }

                    foreach (var validation in botTemplateValidationMappings)
                    {
                        switch (validation.ValidationName)
                        {
                            case "REQUIRED":
                                if (string.IsNullOrWhiteSpace(fileColumn.TextValue))
                                {
                                    record.Errors.Add(new Error()
                                    {
                                        ColumnName = column.ColumnName,
                                        ColumnValue = fileColumn.TextValue,
                                        ErrorDescription = (column.ColumnName + " is a mandatory field"),
                                        Identifier = record.Identifier
                                    });
                                }
                                break;
                            case "MINVALUE":
                                if (tempValue < column.MinValue)
                                {
                                    record.Errors.Add(new Error()
                                    {
                                        ColumnName = column.ColumnName,
                                        ColumnValue = fileColumn.TextValue,
                                        ErrorDescription = (column.ColumnName + " should be greater than " + column.MaxValue),
                                        Identifier = record.Identifier
                                    });
                                }
                                break;
                            case "MAXVALUE":
                                if (tempValue > column.MaxValue)
                                {
                                    record.Errors.Add(new Error()
                                    {
                                        ColumnName = column.ColumnName,
                                        ColumnValue = fileColumn.TextValue,
                                        ErrorDescription = (column.ColumnName + " should be less than "+ column.MaxValue),
                                        Identifier = record.Identifier
                                    });
                                }
                                break;
                            case "REGEX":
                                Match match = Regex.Match(fileColumn.TextValue.TrimEnd(), column.RegularExpression, RegexOptions.IgnoreCase);
                                if (!match.Success)
                                {
                                    record.Errors.Add(new Error()
                                    {
                                        ColumnName = column.ColumnName,
                                        ColumnValue = fileColumn.TextValue,
                                        ErrorDescription = (validation.ValidationName.ToLower() + " validation error"),
                                        Identifier = record.Identifier
                                    });
                                }
                                break;
                            case "VALIDVALUES":
                                List<string> validValues = column.ValidValues.Split(',').ToList();
                                if (!validValues.Contains(fileColumn.TextValue))
                                {
                                    record.Errors.Add(new Error()
                                    {
                                        ColumnName = column.ColumnName,
                                        ColumnValue = fileColumn.TextValue,
                                        ErrorDescription = ("Valid values for this field is/are " + column.ValidValues),
                                        Identifier = record.Identifier
                                    });
                                }
                                break;


                        }
                    }

                    record.Columns.Add(fileColumn);

                }
            }

            if (validationMapping.Any(x => x.TemplateId == template.TemplateId && x.ValidationName == "DUPLICATE"))
            {
                var duplicateValidations = validationMapping.Where(x => x.TemplateId == template.TemplateId && x.ValidationName == "DUPLICATE").ToList();
                foreach (var duplicateValidation in duplicateValidations)
                {
                    List<string> duplicateColumns = duplicateValidation.ColumnNames.Split(',').ToList();
                    foreach (var record in records)
                    {
                        foreach (var duplicateColumn in duplicateColumns)
                        {
                            if (record.Columns.Any(x => x.BOTTemplateColumnId.ToString() == duplicateColumn))
                            {
                                var column = record.Columns.FirstOrDefault(x => x.BOTTemplateColumnId.ToString() == duplicateColumn);
                                if (column.DataType == "DOUBLE")
                                {
                                    record.DuplicateText = record.DuplicateText + column.DecimalValue.ToString();
                                }
                                else
                                {
                                    record.DuplicateText = record.DuplicateText + column.TextValue.ToString();
                                }
                            }
                        }
                    }
                    var duplicateRecords = records.GroupBy(x => x.DuplicateText)
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key)
                                .ToList();
                    foreach (var duplicateRecord in duplicateRecords)
                    {
                        var tempRecords = records.Where(x => x.DuplicateText == duplicateRecord).ToList();
                        tempRecords.ForEach(x => x.Errors.Add(new Error()
                        {
                            Identifier = x.Identifier,
                            ColumnName = "",
                            ColumnValue = "",
                            ErrorDescription = "Duplicate validation error in records - " + string.Join(",", tempRecords.Where(z => z.DuplicateText == duplicateRecord).Select(y => y.RecordId).ToList())
                        }));
                    }

                }

            }


            List<Record> ErrorRecords = new List<Record>();
            foreach (Record item in records)
            {
                if (item.Errors.Count > 0)
                {
                    ErrorRecords.Add(item);
                }

            }

            if (ErrorRecords.Any())
            {
                Console.WriteLine("Sending Email For File - "+ fileName);
                string Subject = fileName + " Validation Results";
                List<string> toaddress = new List<string>() { "rajatarora9@deloitte.com" };


                ValidationObject obj = new ValidationObject();
                obj.totalRecords = records.Count(); ;
                obj.failedRecords = ErrorRecords.Count();
                foreach (var parenterror in ErrorRecords)
                {
                    foreach (var childerror in parenterror.Errors)
                    {
                        EmailErrorRecord emailrecord = new EmailErrorRecord();
                        emailrecord.identifier = parenterror.Identifier;
                        emailrecord.lineNumber = parenterror.RecordId;
                        emailrecord.recordType = childerror.ColumnName;
                        emailrecord.recordValue = childerror.ColumnValue;
                        emailrecord.validationMessage = childerror.ErrorDescription;
                        obj.erorrecords.Add(emailrecord);
                    }

                }

                string validationObj = ToXML(obj);

                string path = Path.GetFullPath("C:/Users/rajatarora9/Desktop/BMBot/Benefit-Management-Bot/BMValidation/BMBOT/XSLTFormat/Transform.xslt");
                string xsltformat = File.ReadAllText(path);

                sendEmail(toaddress, validationObj, Subject, xsltformat);
                
            }
        }

        public static void sendEmail(List<string> toaddress, string validationObj, string Subject, string xsltFormat)
        {
            ServiceReference1.Service1Client emailservice = new Service1Client();
            string host = ConfigurationManager.AppSettings["Host"];
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string username = ConfigurationManager.AppSettings["Username"];
            string password = ConfigurationManager.AppSettings["Password"];
            string port = ConfigurationManager.AppSettings["Port"];

            try
            {
                Task<bool> res = emailservice.SendEmailAsync(toaddress.ToArray(), validationObj, Subject, host, fromEmail, username, password, port, xsltFormat);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occoured - " + ex.Message);
                Console.ReadKey();
            }
        }
    }

    public class ValidationObject
    {
        public ValidationObject()
        {
            erorrecords = new List<EmailErrorRecord>();
        }

        public int totalRecords { get; set; }
        public int failedRecords { get; set; }

        public List<EmailErrorRecord> erorrecords { get; set; }
    }

    public class Record
    {
        public List<Column> Columns { get; set; }

        public int RecordId { get; set; }

        public string RecordText { get; set; }


        public List<Error> Errors { get; set; }
        public string DuplicateText { get; set; }
        public string Identifier { get;  set; }
    }

    public class Error
    {
        public string ColumnName { get; set; }

        public string Identifier { get; set; }
        public string ColumnValue { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class Parameters
    {
        public String ParamName { get; set; }
        public String ParamType { get; set; }
        public String ParamValue { get; set; }
    }

    public class EmailErrorRecord
    {
        public int lineNumber { get; set; }

        public string identifier { get; set; }

        public string validationMessage { get; set; }

        public string recordType { get; set; }

        public string recordValue { get; set; }
    }
}

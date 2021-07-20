using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfEmailService;

namespace WCFEmailServiceCOnsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new Service1();

            List<String> ToAddress = new List<string>();
            ToAddress.Add("rajatarora9@deloitte.com");
            DocValidation validationObj = new DocValidation();

            Record r = new Record();
            r.RecordNumber = 1;
            r.identifier = "SSN1";
            r.fieldName = "Amount";
            r.fieldValue = "5000";
            r.validation = "MaxLimit";
            r.minValue = 1;
            r.maxValue = 500;

            validationObj.Records.Add(r);

            //Console.WriteLine(service.SendEmail(ToAddress,validationObj, "Benefit Management Validatio Bot"));
            Console.ReadKey();
        }
    }

    public class DocValidation
    {
        public DocValidation()
        {
            Records = new List<Record>();
        }
        public List<Record> Records;
    }

    public class Record
    {
        public int RecordNumber { get; set; }
        public string identifier { get; set; }
        public string fieldName { get; set; }
        public string fieldValue { get; set; }
        public string validation { get; set; }
        public int minValue { get; set; }
        public int maxValue { get; set; }
    }
}

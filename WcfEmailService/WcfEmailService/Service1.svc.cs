using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace WcfEmailService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {

        public string ToXML(Object obj)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        public bool SendEmail(List<String> ToAddress, Object ValidationObject, String Subject)
        {
            MailMessage mailmessage = new MailMessage();
            mailmessage.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"]);
            mailmessage.Subject = Subject;
            mailmessage.IsBodyHtml = true;

            foreach (var item in ToAddress)
            {
                mailmessage.To.Add(new MailAddress(item));
            }
            string ObjToXML = ToXML(ValidationObject);

            string XSLTString = File.ReadAllText(@"..\..\..\WcfEmailService\Transform.xslt");

            TextReader tr2 = new StringReader(XSLTString);
            var tr22 = new XmlTextReader(tr2);
            var xsltransform = new XslCompiledTransform();
            xsltransform.Load(tr22);


            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            XmlReader reader = XmlReader.Create(new StringReader(ObjToXML));
            xsltransform.Transform(reader, null, tw);
            mailmessage.Body = tw.ToString(); ;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["Host"];
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
            smtp.UseDefaultCredentials = true;
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            smtp.Credentials = NetworkCred;

            Console.WriteLine("Sending Email......");
            smtp.Send(mailmessage);
            return true;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}

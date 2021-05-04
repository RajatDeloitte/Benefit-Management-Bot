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
using Outlook = Microsoft.Office.Interop.Outlook;

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

            Console.WriteLine("Sending Email......");


            Outlook.Application oApp = new Outlook.Application();
            Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
            oMsg.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            oMsg.Subject = Subject;
            Outlook.Recipients oRecips = oMsg.Recipients;
            foreach (string recipient in ToAddress)
            {
                oRecips.Add(recipient);
            }
            oMsg.HTMLBody = tw.ToString();
            oMsg.Send();


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

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
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Newtonsoft.Json;

namespace WcfEmailService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [KnownType(typeof(Object))]
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

        public Object XMLToObject(string XMLString, Object oObject)
        {

            XmlSerializer oXmlSerializer = new XmlSerializer(oObject.GetType());
            oObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
            return oObject;
        }

        public bool SendEmail(string[] ToAddress,string ObjToXML, string Subject,string host,string FromEmail,string username,
                                                                                                        string password,string port, string xsltformat)
        {
            object obj1 = new object();
            MailMessage mailmessage = new MailMessage();
            mailmessage.From = new MailAddress(FromEmail);
            mailmessage.Subject = Subject;
            mailmessage.IsBodyHtml = true;

            foreach (var item in ToAddress)
            {
                mailmessage.To.Add(new MailAddress(item));
            }
            //string ObjToXML = ToXML(ValidationObject);
            //string path = HttpContext.Current.Server.MapPath("~/Transform1.xslt");
            string XSLTString = xsltformat;

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
            smtp.Host = host;
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential(username, password);
            smtp.UseDefaultCredentials = false;
            smtp.Port = int.Parse(port);
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

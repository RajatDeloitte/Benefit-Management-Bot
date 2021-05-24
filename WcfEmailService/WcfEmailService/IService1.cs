using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfEmailService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebGet(UriTemplate = "/HelloWorld/")]
        bool SendEmail(string[] ToAddress, string ObjToXML, string Subject, string host, string FromEmail, string username,
                                                                                                        string password, string port);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [DataContract]
    public partial class SendEmailRequest
    {
        [DataMember]
        public string[] ToAddress;

        [DataMember]
        public object ValidationObject;
        
        [DataMember]
        public string Subject;


        public SendEmailRequest()
        {
        }

        public SendEmailRequest(string[] ToAddress, object ValidationObject, string Subject)
        {
            this.ToAddress = ToAddress;
            this.ValidationObject = ValidationObject;
            this.Subject = Subject;
        }
    }
}

using System.Collections.Generic;
using System.Xml.Serialization;
using Elivn.AliyunOssSdk.Api.Common;

namespace Elivn.AliyunOssSdk.Api.Object.GetAcl
{
    [XmlRoot("AccessControlPolicy")]
    public class GetObjectAclResult
    {
        [XmlElement("Owner")]
        public Owner Owner { get; set; }

        [XmlArray("AccessControlList")]
        [XmlArrayItem("Grant")]
        public List<string> Grants { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSManager.Commons
{
    public class XmlHelper<T> where T :class
    {
        public XmlHelper()
        {

        }
        public static void SerialToXml(string path,T t)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter sw = new StreamWriter(path);
            serializer.Serialize(sw, t);
            sw.Close();
        }
        public static T Deserialize(string path)
        {
            T t;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader sr = new StreamReader(path);
            t = (T)serializer.Deserialize(sr);
            sr.Close();
            return t;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace OPCUASubscriber.CosmUpload
{
    class Cosm
    {
        const string baseUri = "http://api.cosm.com";
        const string apiKey = "sN8NVF3oRQ4GnwxGd1YSLUPzn8-SAKxJa0hqdHhwNm1Vdz0g";
        const string feedId = "/v2/feeds/68554";


        public static void upload(string value)
        {
            value = "gtf,20";
            try
            {
                WebRequest request = CreateRequest(value);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                
                Console.WriteLine(responseFromServer);
            }
            catch (Exception e)
            {
               Console.WriteLine( e.ToString());
            }
 
        }

        static WebRequest CreateRequest( string value)
        {
            Console.WriteLine(value + " " + baseUri );
            String jsonObj = @"{""version"":""1.0.0"", ""datastreams"":[{""current_value"":""27"".""id"":""15""}] }";
            byte[] buffer = Encoding.ASCII.GetBytes(value);
            
            //HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(baseUri+feedId);
            WebRequest request = WebRequest.Create("http://api.cosm.com/v2/feeds/68554/gtf.csv");
            
            ((HttpWebRequest)request).KeepAlive = false;
            ((HttpWebRequest) request).UserAgent = "Example Client";
            request.Headers.Add("X-ApiKey", apiKey);
            request.Method = "PUT";
            request.ContentLength = buffer.Length;
            request.ContentType = "text/csv";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(buffer, 0, buffer.Length);
            //dataStream.Flush();
            dataStream.Close();
            return request;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using OpcLabs.EasyOpc.UA;
using System.Threading;
using OPCUASubscriber.CosmUpload;
using System.IO;

namespace OPCUASubscriber
{
    class OpcUaTest
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 ) {
                usage(); 
            }
            else
            {
                OpcUaTest opt = new OpcUaTest();
                opt.SubscribeToUA(args);
            }
            
        }

        public static void usage() {
            Console.WriteLine("First argument mut be path conf file, now stopping");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }


        public void SubscribeToUA(String[] args)
        {
            //getting the nodeids and loading them
            List<EasyUAMonitoredItemArguments> nodeList = readConf(args);
            

            
            EasyUAClient easyUAClient = new EasyUAClient();
            try
            {
                
                
                //NodeId ni = new NodeId("ns=2;s=PowerHub.LU1.WatchdogDevelopment");
                UAEndpointDescriptor ud = new UAEndpointDescriptor("opc.tcp://test-opc-ua.powerhub.dk:32402");
                EasyUAClient.EngineParameters.CertificateAcceptancePolicy.AcceptAnyCertificate = true;
                easyUAClient.MonitoredItemChanged += easyUAClient_MonitoredItemChanged;
                //EasyUAClient.EngineParameters.ApplicationCertificateStore = "";
                //UANodeId nid = new UANodeId("ns=2;s=Danfoss.DANFOSS_01.WatchdogReceived");
                //easyUAClient.SubscribeMonitoredItem(ud, nid, 1000);
                //easyUAClient.MonitoredItemChanged += easyUAClient_MonitoredItemChanged;

                easyUAClient.SubscribeMultipleMonitoredItems(nodeList.ToArray <EasyUAMonitoredItemArguments>());


                //String Text = easyUAClient.ReadValue("opc.tcp://test-opc-ua.powerhub.dk:32402", nid).ToString();
                //String Text = easyUAClient.ReadValue( ud, nid).ToString();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }

            while (true)
            {
                Thread.Sleep(100);
            }


        }

        static void easyUAClient_MonitoredItemChanged(object sender, EasyUAMonitoredItemChangedEventArgs e)
        {
            // Display value
            // Remark: Production code would check e.Exception before accessing e.AttributeData.
            Console.WriteLine( e.Arguments.NodeId +" " + e.AttributeData.ServerTimestamp+" " + e.AttributeData.Value );
        }

        public List<EasyUAMonitoredItemArguments> readConf(String[] args)
        {
            var lines = File.ReadLines(args[0]).Select(a => a.Split(';'));
            var csv = from line in lines
                      select (from piece in line
                              select piece);
            //Build a list of the nodeId's
            List<EasyUAMonitoredItemArguments> nodeList = new List<EasyUAMonitoredItemArguments>();
            foreach (var item in csv) {
                UANodeId nid = new UANodeId("ns=2;s=" + item.ToArray<String>()[0] + "." + item.ToArray<String>()[1] + "." + item.ToArray<String>()[2]);
                UAEndpointDescriptor ud = new UAEndpointDescriptor("opc.tcp://test-opc-ua.powerhub.dk:32402");

                EasyUAMonitoredItemArguments euia = new EasyUAMonitoredItemArguments(null, ud, nid);

                nodeList.Add(euia);
            }

            return nodeList;
        }


    }


}

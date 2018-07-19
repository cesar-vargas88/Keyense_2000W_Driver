using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyense_2000W_Driver
{
    class Keyence2000W
    {
        private string  ip;
        private int     port;
        private bool    working;
        private string  data;
        private TCPClient tcpClient;

        public Keyence2000W(string Ip, int Port)
        {
            ip      = Ip;
            port    = Port;
            working = false;
            data    = "";
        }

        public string Trigger(string On, string Off, int TimeOut)
        {
            if (!working)
            {
                working = true;

                try
                {
                    Ping pingSender = new Ping();
                    PingReply reply = pingSender.Send(ip, 400);

                    if (reply.Status == IPStatus.Success)
                    {
                        tcpClient = new TCPClient(ip, port, TimeOut);
                        tcpClient.Connect();

                        try
                        {
                            Task t = Task.Run(() =>
                            {
                                tcpClient.SendMessage(On);
                                data = tcpClient.ReceiveMessage();
                            });

                            TimeSpan ts = TimeSpan.FromMilliseconds(TimeOut);

                            if (!t.Wait(ts))
                            {
                                tcpClient.SendMessage(Off);
                                data = tcpClient.ReceiveMessage();

                                if (!data.Contains("ERROR"))
                                    data = "ERROR\r";
                            }

                            tcpClient.Disconnect();
                            working = false;
                            return data;
                        }
                        catch(Exception)
                        {
                            try
                            {
                                tcpClient.Disconnect();
                                working = false;
                                return "PROCESS_FAIL\r";
                            }
                            catch(Exception)
                            {
                                working = false;
                                return "PROCESS_FAIL\r";
                            }
                        }
                    }
                    else
                    {
                        working = false;
                        return "NO_CONECTION\r";
                    } 
                }
                catch(Exception)
                {
                    working = false;
                    return "PROCESS_FAIL\r";
                }
            }
            return "TASK_RUNNING\r";
        }
    }
}

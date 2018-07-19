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
                                data = "NO_DATA";
                            }

                            tcpClient.Disconnect();
                            working = false;
                            return data;
                        }
                        catch(Exception ee)
                        {
                            try
                            {
                                tcpClient.Disconnect();
                                working = false;
                                return "EXEPTION_1";
                            }
                            catch(Exception eee)
                            {
                                working = false;
                                return "EXEPTION_2";
                            }
                        }
                        
                        /*tcpClient = new TCPClient(ip, port, TimeOut);
                        Console.WriteLine(tcpClient.Connect());
                        Thread.Sleep(5);
                        tcpClient.SendMessage(On);
                        data = tcpClient.ReceiveMessage();
                        working = false;
                        tcpClient.SendMessage(Off);
                        tcpClient.Disconnect();*/
                    }
                    else
                    {
                        working = false;
                        return "ERROR_CONECTION";
                    } 
                }
                catch(Exception e)
                {
                    return "EXEPTION_0";
                }
            }
            return "TASK_RUNNING";
        }
    }
}

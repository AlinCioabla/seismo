﻿using System;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using LiveCharts;
using System.Windows.Controls;

namespace seismograf
{
    public class Axis
    {
        [JsonProperty(PropertyName = "data")]
        public double[][] data { get; set; }
        [JsonProperty(PropertyName = "timeIntervalMs")]
        public string timeIntervalMs { get; set; }
        [JsonProperty(PropertyName = "updateFrequencyMs")]
        public string updateFrequencyMs { get; set; }
        [JsonProperty(PropertyName = "axis")]
        public string axis { get; set; }
    }

    public class JsonObject
    {
        [JsonProperty(PropertyName = "type")]
        public string typejson { get; set; }
        [JsonProperty(PropertyName = "data")]
        public Axis forJson { get; set; }

    }
    public partial class Form1 : Form
    {
        NetworkStream stream;
        TcpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "Waiting for a client.";
            TcpListener server = new TcpListener(IPAddress.Parse("192.168.1.6"), 80);

            server.Start();

            client = server.AcceptTcpClient();

            label4.Text = "A client connected.";

            stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream

            while (client.Available < 3)
            {
                // wait for enough bytes to be available
            }

            Byte[] bytes = new Byte[client.Available];

            stream.Read(bytes, 0, bytes.Length);

            //translate bytes of request to string
            String data = Encoding.UTF8.GetString(bytes);

            if (Regex.IsMatch(data, "^GET"))
            {

            }
            else
            {

            }

            if (new System.Text.RegularExpressions.Regex("^GET").IsMatch(data))
            {
                const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                    + "Connection: Upgrade" + eol
                    + "Upgrade: websocket" + eol
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        System.Security.Cryptography.SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + eol
                    + eol);

                stream.Write(response, 0, response.Length);
            }
            button1.Visible = false;

        }
        private void SetPointsOnGraph(JsonObject jsObj)
        {
            // Asta nu e gata
            if(jsObj.forJson.axis == "x")
            {
                for (int i = 0; i < jsObj.forJson.data.Length; i++)
                        angularGaugeX.Value = (double)(jsObj.forJson.data[i][1]);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stream != null && stream.DataAvailable)
            {
                Byte[] bytes1 = new Byte[client.Available];
                stream.Read(bytes1, 0, bytes1.Length);
                var b = GetDecodedData(bytes1, bytes1.Length);
                //JObject json = JObject.Parse(b);
                var a = JsonConvert.DeserializeObject<JsonObject>(b);
                if(a!=null)
                {
                    SetPointsOnGraph(a);
                }
                label4.Text = b;
            }
        }
        public static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }
            if (dataLength < 0) return "";
            return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
        }
    }
}

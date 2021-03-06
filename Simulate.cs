﻿using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IoTHubDataSimulator
{
    public class Simulate
    {
        // Define the connection string to connect to IoT Hub
        private const string DeviceConnectionString = "";
                    
        public int sensorInstances = 8;

        public int currentSensorInstance;

        public string SendJSONData;


        public int currentErrorCount;


        static void Main(string[] args)
        {

            Console.WriteLine("Main Started");

            Simulate s = new Simulate();
            s.startup();

            Console.WriteLine("Main Finished");

            Console.ReadKey();

        }


        class MySensorData
        {
            public String Name { get; set; }
            public String SensorType { get; set; }
            public String TimeStamp { get; set; }
            public double DataValue { get; set; }
            public String UnitOfMeasure { get; set; }
            public String Location { get; set; }
            public String DataType { get; set; }
            public String MeasurementID { get; set; }
        }

        public void startup()
        {
            
            Timer EventTimer = new Timer();
            EventTimer.Elapsed += new ElapsedEventHandler(TimerCallback);
            EventTimer.Interval = 10000;
            EventTimer.Start();


        }

        private void TimerCallback(object source, ElapsedEventArgs e)
        {

            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

            // Display the date/time when this method got called.
            Console.WriteLine("In TimerCallback: " + DateTime.Now);

            currentErrorCount += 1;


            Console.WriteLine("Sending Date, Error Count is: " + currentErrorCount);

            for (int i = 1; i <= sensorInstances; i++)
            {

                currentSensorInstance = i;


                if (currentErrorCount >= 2)
                {
                    if (currentSensorInstance == 7)
                    {


                        MySensorData SensorInstance = new MySensorData();
                        Random random = new Random();
                        double nominalPSI = random.Next(80, 142);


                        SensorInstance.Name = "PipeLineSensor_" + currentSensorInstance;
                        SensorInstance.SensorType = "PL123";
                        SensorInstance.TimeStamp = DateTime.Now.ToString();

                        SensorInstance.DataValue = nominalPSI;
                        SensorInstance.DataType = "FLOW_PSI";

                        SensorInstance.UnitOfMeasure = "PSI";
                        SensorInstance.MeasurementID = Guid.NewGuid().ToString();
                        SensorInstance.Location = "NULL";

                        SendJSONData = JsonConvert.SerializeObject(SensorInstance);


                        Console.WriteLine("Sending ERROR!");
                        // Send an event
                        SendEvent(deviceClient).Wait();
                        Console.WriteLine("Error Sent!");

                    }
                    else
                    {

                        GenerateData(currentSensorInstance);

                        // Send an event
                        SendEvent(deviceClient).Wait();
                    }


                }

                else

                {

                    GenerateData(currentSensorInstance);

                    // Send an event
                    SendEvent(deviceClient).Wait();

                }


            }







        }




        private void GenerateData(int csi)
        {

            MySensorData SensorInstance = new MySensorData();

            Random random = new Random();
            double nominalPSI = random.Next(245, 251);
            
            SensorInstance.Name = "PipeLineSensor_" + csi;
            SensorInstance.SensorType = "PL123";
            SensorInstance.TimeStamp = DateTime.Now.ToString();

            SensorInstance.DataValue = nominalPSI;
            SensorInstance.DataType = "FLOW_PSI";

            SensorInstance.UnitOfMeasure = "PSI";
            SensorInstance.MeasurementID = Guid.NewGuid().ToString();
            SensorInstance.Location = "NULL";

            SendJSONData = JsonConvert.SerializeObject(SensorInstance);





        }


              
        
        //  and send it to IoT Hub.
        private async Task SendEvent(DeviceClient deviceClient)
        {
            Console.WriteLine("Sending Data: " + SendJSONData);
            Debug.WriteLine("Sending Data: " + SendJSONData);
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(SendJSONData));
            await deviceClient.SendEventAsync(eventMessage);
        }
        
    }
}

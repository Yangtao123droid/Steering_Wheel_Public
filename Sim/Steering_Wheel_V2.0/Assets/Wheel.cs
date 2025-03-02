using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.IO.Ports;
using Microsoft.Win32;
using System;
using System.Text;
using System.Linq.Expressions;

public class Wheel : MonoBehaviour
{
    
    public float setAngle = 0f; //设置角度
    public float angle = 0f; // 当前旋转角度
    public float speed = 0f;  //旋转速度
    public byte[] byte1 = new byte[]{ 171, 220, 170, 62 };
    public byte[] byte2 = new byte[4];
    public byte[] byte3 = new byte[4];
    public byte[] byte4 = new byte[4];
    public float Data1,Data2,Data3,Data4;
    public byte[] controlmode = new byte[] {0x00};

    public byte[] SetAngle = new byte[] {0x00};
    private SerialPort serialPort;
    // Start is called before the first frame update
    void Start()
    {
        
        /**
        运行模式
        **/
        try
        {
                serialPort = new SerialPort("COM6", 256000, Parity.None, 8, StopBits.One);
                serialPort.Open();
                //使用线程
                Thread dataReceivethread = new Thread(new ThreadStart(DataReceived));
                Thread dataSendthread = new Thread(new ThreadStart(DataSend));
                dataReceivethread.Start();
                dataSendthread.Start();
                Debug.Log("COM Init Success");
        }
        catch (Exception ex)
        {
            serialPort = new SerialPort();
            Debug.Log(ex);
        }
    }

    private void DataReceived()
    {
        while(true)
        {
            int count = serialPort.BytesToRead;
            byte[] bytes = { 171, 220, 170, 62 }; // 这是float 1.0的IEEE 754表示
            if(count > 0)
            {   
                byte[] readBuffer = new byte[count];
                try
                {
                    serialPort.Read(readBuffer, 0, count);
                        if(readBuffer[0] == 0x00 && readBuffer[1] == 0x00 && readBuffer[2] == 0x80 && readBuffer[3] == 0x7f)
                        {
                            for(int i = 0; i < 4; i++)
                            {
                                byte1[i] = readBuffer[4 + i];
                                byte2[i] = readBuffer[8 + i];
                                byte3[i] = readBuffer[12 + i];
                                byte4[i] = readBuffer[16 + i];
                            }
                            Data1 = BitConverter.ToSingle(byte1, 0);
                            Data2 = BitConverter.ToSingle(byte2, 0);
                            Data3 = BitConverter.ToSingle(byte3, 0);
                            Debug.Log(Data2);
                        }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
            Thread.Sleep(1); 
        }
    }

    private void DataSend()
    {
        float float1Value = 0.0f;
        float float2Value = 111.321f;
        byte[] header = new byte[] {0x06, 0x08};
        byte[] float1 = BitConverter.GetBytes(float1Value);
        byte[] float2 = BitConverter.GetBytes(float2Value);
        while(true)
        {
            try
            {
                serialPort.Write(header, 0, header.Length);
                serialPort.Write(float1, 0, float1.Length);
                serialPort.Write(float2, 0, float2.Length);
                serialPort.Write(controlmode, 0, controlmode.Length);
                serialPort.Write(SetAngle, 0, SetAngle.Length);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            Thread.Sleep(1);
        }
    }  

    public float Data3_Last = 0.0f;
    void update()
    {

        var ui = FindObjectOfType<MainUI>();
        ui.data.current = Data2;
        controlmode[0] = ui.data.controlCode;
        if(Data3 == 0.0f && Data3 != Data3_Last)
        {
            ui.data.controlCode = 0x00;
        }
        Data3_Last = Data3;

        if(ui.data.controlCode == 0x02)
        {
            Thread.Sleep(10);
            ui.data.controlCode = 0x00; 
        }   
        SetAngle[0] = ui.data.rotate;
    }  

    // Update is called once per frame
    void Update()
    {
        setAngle = Data1;
        update();
        transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
        angle += speed * Time.deltaTime;
        if(angle > setAngle + 2)
        {   
            speed = -500;
        }
        else if(angle < setAngle - 2)
        {
            speed = 500;
        }
        else
        {
            speed = 0;
        }
    }
}

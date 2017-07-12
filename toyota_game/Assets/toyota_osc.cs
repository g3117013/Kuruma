using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.IO.Ports;

using System.Collections.Generic;
using System.IO;

namespace UnityStandardAssets.Vehicles.Car
{

    public class SerialReceiver : MonoBehaviour
    {
        private static SerialPort sp = new SerialPort("COM3", 115200);
        public string sendData = "";

        // Use this for initialization
        public void Start()
        {
            OpenConnection();
        }

        // Update is called once per frame
        public void Update()
        {
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     Debug.Log("Send message");
            //     sp.Write(sendData);
            // }
        }

        public void Send()
        {
            sp.Write(sendData);
            //Debug.Log("Send message" + sendData);
            sendData = "";
        }


        public void OnApplicationQuit()
        {
            sp.Close();
        }

        private void OpenConnection()
        {
            if (sp != null)
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                    Debug.LogError("Failed to open Serial Port, already open!");
                }
                else
                {
                    sp.Open();
                    sp.ReadTimeout = 50;
                    Debug.Log("Open Serial port Rでログとれる");
                }
            }
        }
    }

 

    [RequireComponent(typeof(CarController))]
    public class toyota_osc : MonoBehaviour
    {

        #region Field

        public string serverId = "MaxMSP";
        public string serverIp = "127.0.0.1";
        public int serverPort = 12000;

        public KeyCode debugKey = KeyCode.S;
        public string debugMessage = "/sample";

        public bool enableShowDebugLog = true;

        private CarController m_Car; // the car controller we want to use

        public GameObject BackCamera;
        public GameObject FrontCamera;

        public SerialReceiver SerialConnection;

        private int RecCount = 0;
        private int RecTiming = 10;
        private int ArrayCount = 0;


        private string SteerLog = "";
        private double nowSteer = 0;
        private Vector3 nowVector;
        private List<Vector3> SteerList = new List<Vector3>();
        private List<Vector3> PosList = new List<Vector3>();

        private string AngleLog = "";

        private double guideAngle = 0;
        private int guideAngleInt = 0;

        public enum GearType
        {
            Forward,
            Backward
        }

        public GearType Gear { get; set; }


        #endregion Field

        public toyota_osc() : base()
        {
            Gear = GearType.Backward;
        }


        void Start()
        {
            OSCHandler.Instance.Init(this.serverId, this.serverIp, this.serverPort);
            ShowDebugLog();

            BackCamera = GameObject.Find("BackCamera");
            FrontCamera = GameObject.Find("FrontCamera");

            BackCamera.SetActive(true);
            FrontCamera.SetActive(false);


            // シリアル通信開始
            SerialConnection = new SerialReceiver();
            SerialConnection.Start();

        }

        // Update is called once per frame
        void Update()
        {
            RecCount++;

            Vector3 tmp = GameObject.Find("Car").transform.position;
            GameObject.Find("Car").transform.position = new Vector3(tmp.x, tmp.y, tmp.z);

            PosList.Add(tmp);

            // if (Input.GetKeyDown(this.debugKey))
            // {
            //SendDebugMessageToClient();
            //}

            if (Input.GetKeyDown(KeyCode.R))
            {
                // Debug.Log("SteerLog = " + SteerLog);
                // SteerLog = "";

                //vector
                StreamWriter sw;
                FileInfo fi;
                DateTime dateTime= DateTime.Now;
                string filename = Convert.ToString(dateTime);
                filename = filename.Replace("/", "_");
                filename = filename.Replace(":", "_");
                filename = filename.Replace(" ", "_");

                Debug.Log("filename=" + filename);

                fi = new FileInfo(Application.dataPath + "/"+filename+".txt");
                sw = fi.AppendText();

                
                string steerLogString = "";
                for (int i = 0; i < SteerList.Count; i++)
                {
                    steerLogString += (SteerList[i].x + "," + SteerList[i].y + "," + SteerList[i].z + ":" + PosList[i].x + "," + PosList[i].y + "," + PosList[i].z + "?");
                }
                sw.WriteLine(steerLogString);
                sw.Flush();
                sw.Close();

                Debug.Log("CSVファイルを出力しました。");

            }

            // Debug.Log("update");
            // カメラとギア変えるやつ
            if (Input.GetKeyDown("joystick button 12"))
            {
                if (BackCamera.activeSelf)
                {
                    BackCamera.SetActive(false);
                    FrontCamera.SetActive(true);
                    Gear = GearType.Forward;
                    Debug.Log("Forward");
                }
                else if (FrontCamera.activeSelf)
                {
                    BackCamera.SetActive(true);
                    FrontCamera.SetActive(false);
                    Gear = GearType.Backward;
                    Debug.Log("Backward");
                }
            }
            // Debug.Log(tmp.x);

            // SerialConnection.Update();

            string sendData = "";
            if (CrossPlatformInputManager.GetAxis("Horizontal") < 0) {
                sendData = "250\n";
            } else {
                sendData = "251\n";
            }

            SerialConnection.sendData = sendData;
            SerialConnection.Send();

            Debug.Log("update + " + sendData);



            //Nフレームに1回 記録と配列読み込み
            if (RecCount % RecTiming == 0) {
                //お手本データがなくなったらガイド無くす処理をかく
                
                //記録用　現在の舵角取得
                nowSteer = CrossPlatformInputManager.GetAxis("Horizontal");
                //舵角の記録
                SteerLog += nowSteer + ",";



                //お手本データ
                guideAngle = Double.Parse(GetComponent<CSVReader>().GuideArray[ArrayCount]);
                //お手本データ*74
                guideAngleInt = (int)(guideAngle * 74.0);

                //向きベクトル取得
                float angleDir = transform.eulerAngles.z * (Mathf.PI / 180.0f);
                Vector3 dir = new Vector3(Mathf.Cos(angleDir), Mathf.Sin(angleDir), 0.0f);

                SteerList.Add(dir);

                

                
                //
                //VecterLog += tmp;
                sendData = 400 + guideAngleInt + "\n";

                SerialConnection.sendData = sendData;
                SerialConnection.Send();

                Debug.Log("otehon = " + sendData);

                ArrayCount++;
            }

            int rot = (int)(CrossPlatformInputManager.GetAxis("Horizontal") * 74.0);
            rot = (rot < 0) ? -rot : rot;
            sendData = rot.ToString() + "\n";

            SerialConnection.sendData = sendData;
            SerialConnection.Send();

            Debug.Log("current rot = " + sendData);



        }

        //private void SendDebugMessageToClient()
        //{
        //    OSCHandler.Instance.SendMessageToClient


        //        //(this.serverId, this.debugMessage, Time.timeSinceLevelLoad);
        //        //小野さん部分
        //        //(this.serverId, this.debugMessage, CrossPlatformInputManager.GetAxis("Horizontal") * 9);

        //        //24球・左右450°のステアリング
        //        //(this.serverId, this.debugMessage, CrossPlatformInputManager.GetAxis("Horizontal") * 29);

        //        //60球・左右450°のステアリング
        //        (this.serverId, this.debugMessage, CrossPlatformInputManager.GetAxis("Horizontal") * 74);

        //    ShowDebugLog();
        //}

        private void ShowDebugLog()
        {
            if (this.enableShowDebugLog == false)
            {
                return;
            }

            Debug.Log("OSC (" + this.serverId + ", "
                              + this.serverIp + ", "
                              + this.serverPort + ")");
        }
        // 車を動かす処理
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            //Debug.Log("update");
            //// 
            //if (Input.GetKeyDown("joystick button 14"))
            //{
            //    //if (BackCamera.activeSelf)
            //    //{
            //    //    BackCamera.SetActive(false);
            //    //    FrontCamera.SetActive(true);
            //    //    Gear = GearType.Forward;
            //    //    Debug.Log("Forward");
            //    //}
            //    //else if (FrontCamera.activeSelf)
            //    //{
            //    //    BackCamera.SetActive(true);
            //    //    FrontCamera.SetActive(false);
            //    //    Gear = GearType.Backward;
            //    //    Debug.Log("Backward");
            //    //}
            //    //return;
            //    Debug.Log("test");
            //}

            // pass the input to the car!
            var steering = CrossPlatformInputManager.GetAxis("Horizontal");

            // -1.0 - 1.0
            // アクセルペダルを踏んでいない時に、0.0
            // アクセルペダルを踏み切った時に、1.0
            // ブレーキペダルを踏み切った時に、-1.0
            float velocity = Mathf.Clamp(CrossPlatformInputManager.GetAxis("Vertical"), 0, 1);
            float footbrake = Mathf.Clamp(CrossPlatformInputManager.GetAxis("Vertical"), -1, 0);


            //if (Gear == GearType.Forward)
            //{
            //    velocity *= -1;

            //    if (velocity <= 0.01)
            //    {
            //        velocity = 0;
            //    }
            //}


            //Debug.Log("velocity = " + velocity + "footbrake = " + footbrake);


#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");


            //m_Car.Move(steering, velocity, velocity, handbrake);
            m_Car.Move_toyota(steering, velocity, footbrake, handbrake, (int)Gear);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}

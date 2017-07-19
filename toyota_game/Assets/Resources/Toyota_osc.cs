using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;



namespace UnityStandardAssets.Vehicles.Car
{
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

        }

        void Update()
        {
            Vector3 tmp = GameObject.Find("Car").transform.position;
            GameObject.Find("Car").transform.position = new Vector3(tmp.x, tmp.y, tmp.z);
            // if (Input.GetKeyDown(this.debugKey))
            // {
            SendDebugMessageToClient();
            //}

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

        }

        private void SendDebugMessageToClient()
        {
            OSCHandler.Instance.SendMessageToClient


                //(this.serverId, this.debugMessage, Time.timeSinceLevelLoad);

                (this.serverId, this.debugMessage, CrossPlatformInputManager.GetAxis("Horizontal") * 9);


            ShowDebugLog();
        }

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

/* This is an example to show how to connect to 2 HM-10 devices
 * that are connected together via their serial pins and send data
 * back and forth between them.
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using Assets.Scripts.GameState;

public class BluetoothInput : MonoBehaviour
{
    #region Singleton Implementation
    private static BluetoothInput _instance = null;
    private BluetoothInput() { }
    public static BluetoothInput Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BluetoothInput();
            }
            return _instance;
        }
    }

    public static void Recycle()
    {
        _instance = null;
    }
    #endregion


    private string DeviceName = "DSD TECH";
    private string ServiceUUID = "FFE0";
    private string Characteristic = "FFE1";

	//public Text HM10_Status;
	//public Text BluetoothStatus;

	enum States
	{
		None,
		Scan,
		Connect,
		Subscribe,
		Unsubscribe,
		Disconnect,
		Communication,
	}

	private bool _workingFoundDevice = true;
	private bool _connected = false;
	private float _timeout = 0f;
	private States _state = States.None;
	private bool _foundID = false;

	// this is our hm10 device
	private string _hm10;

	void Reset()
	{
		_workingFoundDevice = false;	// used to guard against trying to connect to a second device while still connecting to the first
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_foundID = false;
		_hm10 = null;
	}

	void SetState(States newState, float timeout)
	{
		_state = newState;
		_timeout = timeout;
	}

	void StartProcess()
	{
		//BluetoothStatus.text = "Initializing...";
        Debug.Log("Initializing...");

		Reset ();
		BluetoothLEHardwareInterface.Initialize(true, false, () => {
			SetState(States.Scan, 0.1f);
			//BluetoothStatus.text = "Initialized";
            Debug.Log("Initialized");

        }, (error) => {
			BluetoothLEHardwareInterface.Log("Error: " + error);
            Debug.Log("Error: " + error);
        });
	}

    void Awake()
    {
        //Check if instance already exists
        if (_instance == null)
        {
            //if not, set instance to this
            _instance = this;
            StartProcess();
            //HM10_Status.text = "Bluetooth Input Starting";
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            Debug.Log("Bluetooth Input Starting");
        }
        else if (_instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        
    }

    // Use this for initialization
    void Start()
	{
		//HM10_Status.text = "Bluetooth Input Starting";
        //DontDestroyOnLoad(gameObject);
        //StartProcess();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = 0f;

				switch (_state)
				{
				    case States.None:
					    break;

				    case States.Scan:
					    //BluetoothStatus.text = "Scanning for Dartboard...";
                        Debug.Log("Scanning for Dartboard...");

                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

						    // we only want to look at devices that have the name we are looking for
						    // this is the best way to filter out devices
						    if (name.Contains (DeviceName))
						    {
							    _workingFoundDevice = true;

							    // it is always a good idea to stop scanning while you connect to a device
							    // and get things set up
							    BluetoothLEHardwareInterface.StopScan();
							    //BluetoothStatus.text = "";

							    // add it to the list and set to connect to it
							    _hm10 = address;

							    //HM10_Status.text = "Found Dartboard";
                                Debug.Log("Found Dartboard");

                                SetState (States.Connect, 0.5f);

							    _workingFoundDevice = false;
						    }

					    }, null, false, false);
					    break;

				    case States.Connect:
					    // set these flags
					    _foundID = false;

					    //HM10_Status.text = "Connecting to Dartboard";
                        Debug.Log("Connecting to Dartboard");

                        // note that the first parameter is the address, not the name. I have not fixed this because
                        // of backwards compatiblity.
                        // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
                        // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
                        // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
                        BluetoothLEHardwareInterface.ConnectToPeripheral (_hm10, null, null, (address, serviceUUID, characteristicUUID) => {

                            Debug.Log("Connect To Peripheral. serviceUUID : " + serviceUUID + "; ServiceUUID : " + ServiceUUID);
                            if (IsEqual (serviceUUID, ServiceUUID))
						    {
							    // if we have found the characteristic that we are waiting for
							    // set the state. make sure there is enough timeout that if the
							    // device is still enumerating other characteristics it finishes
							    // before we try to subscribe
							    if (IsEqual (characteristicUUID, Characteristic))
							    {
								    _connected = true;
								    SetState (States.Subscribe, 2f);

								    //HM10_Status.text = "Connected to Dartboard";
                                    Debug.Log("Connected to Dartboard");
                                }
						    }
					    }, (disconnectedAddress) => {
						    BluetoothLEHardwareInterface.Log ("Device disconnected: " + disconnectedAddress);
						    //HM10_Status.text = "Disconnected";
                            Debug.Log("Device disconnected: " + disconnectedAddress);
                            // keep trying to connect
                            SetState(States.Connect, 0.5f);
                        });
					    break;

				    case States.Subscribe:
					    //HM10_Status.text = "Subscribing to Dartboard";
                        Debug.Log("Subscribing to Dartboard");

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_hm10, ServiceUUID, Characteristic, null, (address, characteristicUUID, bytes) => {
						   //HM10_Status.text = "Received Serial: " + Encoding.UTF8.GetString (bytes);
                            Debug.Log("Received Serial: " + Encoding.UTF8.GetString(bytes));

                            if (GameController.Instance != null)
                            {
                                GameController.Instance.ThrowDart(Encoding.UTF8.GetString(bytes));
                            }
					    });

					    // set to the none state and the user can start sending and receiving data
					    _state = States.None;
					    //HM10_Status.text = "Waiting...";
                        Debug.Log("Waiting for dartboard input...");
                        GameSetup.Instance.DartboardConnected = true;

                        break;

				    case States.Unsubscribe:
					    BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_hm10, ServiceUUID, Characteristic, null);
                        Debug.Log("Unsubscribing to Dartboard");
                        SetState (States.Disconnect, 4f);
					    break;

				    case States.Disconnect:
					    if (_connected)
					    {
						    BluetoothLEHardwareInterface.DisconnectPeripheral (_hm10, (address) => {
							    BluetoothLEHardwareInterface.DeInitialize (() => {
								
								    _connected = false;
								    _state = States.None;
							    });
						    });
					    }
					    else
					    {
						    BluetoothLEHardwareInterface.DeInitialize (() => {
							
							    _state = States.None;
						    });
					    }
                        Debug.Log("State : Disconnected");
                        break;
				}
			}
		}
	}

	string FullUUID (string uuid)
	{
		return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
	}
	
	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);

		return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
	}

}

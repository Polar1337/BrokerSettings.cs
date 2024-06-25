using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrokerSettings : MonoBehaviour
{
    #region Settings
    /// <summary>
    /// Broker Host Address
    /// </summary>
    public string brokerAddress = "XXXCENSOREDXXX";

    /// <summary>
    /// Broker Port
    /// </summary>
    public int brokerPort = 8883;

    /// <summary>
    /// Broker Username
    /// </summary>
    public string userName = "XXXCENSOREDXXX";

    /// <summary>
    /// Broker Password
    /// </summary>
    public string password = "XXXCENSOREDXXX";

    /// <summary>
    /// Client ID
    /// </summary>
    public string clientId = "UnityClient";
    #endregion

    #region Paramter for Unity
    /// <summary>
    /// Text to display the connection status
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _connectionText;

    /// <summary>
    /// Text to display the process level
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_level;

    /// <summary>
    /// Text to display the power on val
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _power_on_val;

    /// <summary>
    /// Text to display the process name
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_name_val;
   
    /// <summary>
    /// Text to display the current username
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _current_username;

    /// <summary>
    /// Text to display the start status
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_started_val;

    /// <summary>
    /// Text to display the stop status
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_finished_val;

    /// <summary>
    /// Text to display the aborted status
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_aborted_val;

    /// <summary>
    /// Text to display the current processed val
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _current_processed_val;
   
    /// <summary>
    /// Text to display the current number of workpieces
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _current_workpiece_num;

    /// <summary>
    /// Text to display the current process type
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _current_processed_typed_val;

    /// <summary>
    /// Text to display the process duration
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _process_time;
    #endregion

    private IMqttClient _mqttClient;
    private SynchronizationContext _currentThread;


    // Start is called before the first frame update
    async void Start()
    {
        // Save current thread in a variable
        _currentThread = System.Threading.SynchronizationContext.Current;

        var mqttFactory = new MqttFactory();

        _mqttClient = mqttFactory.CreateMqttClient();

        // Set up the connection
        var options = new MqttClientOptionsBuilder()
            //.WithClientId(clientId) // it only works without this line
            .WithTcpServer(brokerAddress, brokerPort)
            .WithTlsOptions(o =>
            {
                o.WithCertificateValidationHandler(_ => true);
            })
            .WithCredentials(userName, password)
            .Build();

        // Add handlers
        _mqttClient.ConnectedAsync += e =>
        {
            Debug.Log("Connected to the broker");

            _currentThread.Post(_ =>
            {
                _connectionText.text = "Connected";
            }, null);

            return Task.CompletedTask;
        };

        _mqttClient.DisconnectedAsync += e =>
        {
            Debug.Log("Client disconnected");

            _currentThread.Post(_ =>
            {
                _connectionText.text = "Disconnected";
            }, null);

            return Task.CompletedTask;
        };

        _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;

        // Connect to the broker
        await _mqttClient.ConnectAsync(options);

        Debug.Log("Connected to the broker");

        // Subscribe to a topic
        await _mqttClient.SubscribeAsync(mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
            {
                f.WithTopic("#");
            })
            .Build());
    }

    private Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        Debug.Log($"Received message from topic {args.ApplicationMessage.Topic} message: {args.ApplicationMessage.ConvertPayloadToString()}");

        var msg = args.ApplicationMessage;

        switch (msg.Topic)
        {
            case "DHBW_Stanzmaschine/process_level":
                // Update the process level text
                _currentThread.Post(_ =>
                {
                    _process_level.text = $"Process level: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/power_on_val":
                // Update the power on val
                _currentThread.Post(_ =>
                {
                    _power_on_val.text = $"Power on val: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/current_username":
                // Update the current username
                _currentThread.Post(_ =>
                {
                    _current_username.text = $"Username: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/processname_val":
                // Update the prozess name
                _currentThread.Post(_ =>
                {
                    _process_name_val.text = $"Processname: {msg.ConvertPayloadToString()}";
                }, null);
                break;

            case "DHBW_Stanzmaschine/process_started_val":
                // Update the process_started_val
                _currentThread.Post(_ =>
                {
                    _process_started_val.text = $"Started val: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/process_finished_val":
                // Update the process_finished_val
                _currentThread.Post(_ =>
                {
                    _process_finished_val.text = $"Finished val: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/process_aborted_val":
                // Update the process_aborted_val
                _currentThread.Post(_ =>
                {
                    _process_aborted_val.text = $"Aborted val: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/current_processed_val":
                // Update the current_processed_val
                _currentThread.Post(_ =>
                {
                    _current_processed_val.text = $"Processed val: {msg.ConvertPayloadToString()}";
                }, null);
                break;

            case "DHBW_Stanzmaschine/current_workpiece_num":
                // Update the number of workpieces
                _currentThread.Post(_ =>
                {
                    _current_workpiece_num.text = $"Current workpiece num: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/current_processed_typed_val":
                // Update the process type
                _currentThread.Post(_ =>
                {
                    _current_processed_typed_val.text = $"Processed Type val: {msg.ConvertPayloadToString()}";
                }, null);
                break;
            case "DHBW_Stanzmaschine/process_time":
                // Update the duration of process
                _currentThread.Post(_ =>
                {
                    _process_time.text = $" Process duration: {msg.ConvertPayloadToString()}";
                }, null);
                break;


            // If the topic is unkown log a warning
            default:
                Debug.LogWarning($"Unknown topic {msg.Topic}");
                break;
        }

        return Task.CompletedTask;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

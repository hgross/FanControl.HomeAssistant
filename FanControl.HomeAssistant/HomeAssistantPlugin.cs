using FanControl.Plugins;
using System.Collections.Generic;

namespace FanControl.HomeAssistant
{

    internal class HomeAssistantConfig {
        public string HassAuthToken {get; set;}
        public string HassURL {get; set;}
    }

    public class HomeAssistantPlugin : IPlugin2
    {
        public string Name => "HomeAssistant";
        private readonly IPluginLogger _logger;
        private readonly IPluginDialog _dialog;

        private HomeAssistantConfig _hassConfig;
        private List<HomeAssistantSensor> sensors = new List<HomeAssistantSensor>();


        public HomeAssistantPlugin(IPluginLogger logger, IPluginDialog dialog)
        {
            _logger = logger;
            _dialog = dialog;
            _hassConfig = new HomeAssistantConfig() {
                HassAuthToken="xxx", 
                HassURL="http://smarthome-infrastructure.home.arpa:8123"
            };
            sensors.Add(new HomeAssistantSensor("sensor.office_hue_motion_temperature", _hassConfig, _logger));
            sensors.Add(new HomeAssistantSensor("sensor.thermostat_office_actual_temperature", _hassConfig, _logger));
        }

        public void Close()
        {
            // _logger.Log("----------------------------");
            // _logger.Log("Closing Plugin");
        }

        public void Initialize()
        {
            // _logger.Log("----------------------------");
            // _logger.Log("Initializing Plugin");
            // TODO: read sensor id's, auth token and connection details from a config file

        }

        public void Load(IPluginSensorsContainer container)
        {
            // _logger.Log("----------------------------");
            // _logger.Log("Loading Sensors");
            foreach (var sensor in this.sensors)
            {
                container.TempSensors.Add(sensor);    
            }
            
        }

        // Called every 1Hz to update all sensors
        public void Update()
        {
            // _logger.Log("----------------------------");
            // _logger.Log("Updating Sensors");
        }
    }

}

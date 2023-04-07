using FanControl.Plugins;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace FanControl.HomeAssistant
{
    /// <summary>
    /// FanControl Plugin API implementation for HomeAssistant temperature sensors.
    /// </summary>
    public class HomeAssistantPlugin : IPlugin2
    {
        public static readonly string LOG_PREFIX = "[FanControl.HomeAssistant] ";
        public string Name => "HomeAssistant";
        private readonly IPluginLogger _logger;
        private readonly IPluginDialog _dialog;
        private HomeAssistantConfig _hassConfig;
        private readonly List<HomeAssistantSensor> _sensors;


        public HomeAssistantPlugin(IPluginLogger logger, IPluginDialog dialog)
        {
            _logger = logger;
            _dialog = dialog;
            _sensors = new List<HomeAssistantSensor>();
        }

        public void Close()
        {
            // nothing to do
        }

        public void Initialize()
        {
            string cwd_path = Directory.GetCurrentDirectory();
            var config_directory_path = Path.Combine(cwd_path, "Configurations");
            var config_json_path = Path.Combine(config_directory_path, "FanControl.HomeAssistant.json");

            // check existance
            if(!File.Exists(config_json_path)) {
                // create default config and write to file, then print error
                var default_config = new HomeAssistantConfig();
                default_config.HomeAssistantURL = "http://example.com:8123";
                default_config.HomeAssistantAuthToken = "your_long_lived_access_token_created_from_home_assistants_user_configuration";
                default_config.sensors.Add(new HomeAssistantSensorConfig{ EntityId="sensors.your_temp_sensor_entity_id", InitialFallbackValue=10.0f, PollingInterval=10 });
                var serialized_default_config = JsonConvert.SerializeObject(default_config);
                File.WriteAllText(config_json_path, serialized_default_config);
                
                // log & show dialog
                var error_message = $"Error: Could not find a config file for the FanControl.HomeAssistant plugin at {config_directory_path}. Creating {config_json_path}. Please edit it to your needs and restart FanControl.";
                _logger.Log(LOG_PREFIX + error_message);
                _dialog.ShowMessageDialog(error_message);
            } 

            // Read config
            try
            {
                string hassConfigRaw = File.ReadAllText(config_json_path);
                var hassConfig = JsonConvert.DeserializeObject<HomeAssistantConfig>(hassConfigRaw);
                _hassConfig = hassConfig;
                
                // TODO: some additional validation - nice to have.
            }
            catch (System.Exception e)
            {
                var error_message = $"Error: Could not parse configuration for the FanControl.HomeAssistant plugin at {config_json_path}. Please edit it to be valid and restart FanControl. You can delete {config_json_path} and restart FanControl to get back the default config.";
                _logger.Log(LOG_PREFIX + error_message);
                _logger.Log(LOG_PREFIX + "Not loading any FanControl.HomeAssistant sensors.");
                _logger.Log(LOG_PREFIX + $"Parsing exception hint: {e.Message}" + Environment.NewLine + e.StackTrace);
                _dialog.ShowMessageDialog(error_message);
            }
        }

        public void Load(IPluginSensorsContainer container)
        {
            // Create plugin sensors from config
            foreach (var hassSensorConfig in _hassConfig.sensors)
            {
                _sensors.Add(new HomeAssistantSensor(hassSensorConfig, _hassConfig, _logger));
            }  

            // hand over sensor instances to FanControl
            foreach (var sensor in this._sensors)
            {
                container.TempSensors.Add(sensor);
            }
        }

        // Called every 1Hz to update all sensors
        public void Update()
        {
            // unused - implemented in the individual sensor instances
        }
    }

}

using FanControl.Plugins;
using RestSharp;
using Newtonsoft.Json;

namespace FanControl.HomeAssistant
{
    /// <summary>
    /// HASS implementation of  a sensor. 
    /// Periodically polls the state of a sensor entity from Home Assistant.
    /// </summary>
    public class HomeAssistantSensor : IPluginSensor
    {
        private HomeAssistantConfig _hassConfig;
        private HomeAssistantSensorConfig _hassSensorConfig;
        private IPluginLogger _logger;
        private long _cycle = 0;

        internal HomeAssistantSensor(HomeAssistantSensorConfig hassSensorConfig, HomeAssistantConfig hassConfig, IPluginLogger logger)
        {
            _hassConfig = hassConfig;
            _hassSensorConfig = hassSensorConfig;
            _logger = logger;

            Id = _hassSensorConfig.EntityId;
            Name = _hassSensorConfig.EntityId; ; // temporary, will be overwritten on first update
            Value = _hassSensorConfig.InitialFallbackValue; // initial value to avoid NaN

            if (_hassSensorConfig.PollingInterval < 1)
            {
                _logger.Log(HomeAssistantPlugin.LOG_PREFIX + $"Error: Configured polling interval of {_hassSensorConfig.PollingInterval} seconds for sensor {_hassSensorConfig.EntityId} is invalid. Falling back to default value of {HomeAssistantSensorConfig.POLLING_INTERVAL_DEFAULT}.");
                _hassSensorConfig.PollingInterval = HomeAssistantSensorConfig.POLLING_INTERVAL_DEFAULT;
            }
        }

        public string Name { get; }

        public float? Value { get; internal set; }

        public string Id { get; }

        // https://developers.home-assistant.io/docs/api/rest
        public void Update()
        {
            // called in 1 hz cycles by FC
            _cycle += 1;
            bool is_first_cycle = _cycle == 1;

            // only update every pollingIntevall-th cycle
            if (!is_first_cycle && (_cycle % _hassSensorConfig.PollingInterval != 0))
            {
                return;
            }

            _poll_data();
        }

        /// <summary>
        /// HomeAssistant REST API Response structure 
        /// </summary>
        private class HassEntityAttributes
        {
            public string state_class { get; set; }
            public string unit_of_measurement { get; set; }
            public string device_class { get; set; }
            public string friendly_name { get; set; }
        }

        /// <summary>
        /// HomeAssistant REST API Response structure 
        /// </summary>
        private class HassTemperatureSesnorResponse
        {
            public float state { get; set; }
            public string entity_id { get; set; }
            public string last_changed { get; set; }
            public string last_updated { get; set; }
            public HassEntityAttributes attributes { get; set; }
            public override string ToString()
            {
                return $"SensorData for {this.attributes.friendly_name} ({this.entity_id}): {this.state} {this.attributes.unit_of_measurement}";
            }
        }

        /// <summary>
        /// Asynchronously fetches the state of the entity id from the HomeAssistant REST API.
        /// </summary>
        /// <returns></returns>
        private async void _poll_data()
        {
            try
            {
                // Expected format is this
                /*  HTTP/1.1 200 OK
                    Content-Type: application/json
                    Content-Length: 265
                    Content-Encoding: deflate
                    Date: Thu, 06 Apr 2023 20:35:28 GMT
                    Server: Python/3.10 aiohttp/3.8.4
                    Connection: close

                    {
                        "entity_id": "sensor.office_hue_motion_temperature",
                        "state": "21.2",
                        "attributes": {
                            "state_class": "measurement",
                            "unit_of_measurement": "°C",
                            "device_class": "temperature",
                            "friendly_name": "Sensor Office Temperature"
                        },
                        "last_changed": "2023-04-06T20:27:27.542739+00:00",
                        "last_updated": "2023-04-06T20:27:27.542739+00:00",
                        "context": {
                            "id": "01GXC41CDPMV245AW4X5ADKF4X",
                            "parent_id": null,
                            "user_id": null
                        }
                    }
                 */

                // get the state from the REST API
                var client = new RestClient(_hassConfig.HomeAssistantURL);
                client.AddDefaultHeader("Authorization", "Bearer " + _hassConfig.HomeAssistantAuthToken);
                var request = new RestRequest("api/states/{EntityId}", Method.Get);
                request.AddUrlSegment("EntityId", Id);
                var response = await client.ExecuteAsync(request);
                var resp_code = (int)response.StatusCode;

                // Handle response based on code.
                if (resp_code == 200)
                {
                    string rawResponse = response.Content;
                    HassTemperatureSesnorResponse sensorData = JsonConvert.DeserializeObject<HassTemperatureSesnorResponse>(rawResponse);
                    Value = sensorData.state;
                }
                else
                {
                    // todo: maybe some error logic for 404s / invalid configs. Nice to have.
                    _logger.Log(HomeAssistantPlugin.LOG_PREFIX + $"Error retrieving state for sensor {Id} with status code {resp_code}");
                }
            }
            catch (System.Exception e)
            {
                _logger.Log(HomeAssistantPlugin.LOG_PREFIX + $"Error polling state of {Id} -> {e.Message}");
                _logger.Log(HomeAssistantPlugin.LOG_PREFIX + e.StackTrace);
            }

        }
    }
}

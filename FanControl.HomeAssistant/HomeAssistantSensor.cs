﻿using FanControl.Plugins;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using Newtonsoft.Json;

namespace FanControl.HomeAssistant
{
    public class HomeAssistantSensor : IPluginSensor
    {
        private HomeAssistantConfig _hassConfig;
        private IPluginLogger _logger;
        private int _pollingInterval;
        private long cycle = 0;

        internal HomeAssistantSensor(string id, HomeAssistantConfig hassConfig, IPluginLogger logger, int pollingInterval = 30)
        {
            Id = id;
            Name = id; // temporary, will be overwritten on first update
            _hassConfig = hassConfig;
            _logger = logger;
            _pollingInterval = pollingInterval;

            if (pollingInterval < 1)
            {
                _logger.Log($"Error: Configured polling interval of {pollingInterval} seconds for sensor {id} is invalid. Falling back to default value.");
                pollingInterval = 30;
            }
        }

        public string Name { get; }

        public float? Value { get; internal set; }

        public string Id { get; }

        // https://developers.home-assistant.io/docs/api/rest
        public void Update()
        {
            // called in 1 hz cycles by FC
            cycle += 1;

            // only update every _pollingIntevall-th cycle
            if (cycle % _pollingInterval != 0)
            {
                return;
            }

            _poll_data();
        }


        private class HassEntityAttributes
        {
            public string state_class { get; set; }
            public string temperature_valid { get; set; }
            public string unit_of_measurement { get; set; }
            public string device_class { get; set; }
            public string friendly_name { get; set; }
        }

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


        private async void _poll_data()
        {
            try
            {
                //_logger.Log($"Updating {Id} ...");

                // Expected format is
                /*                 HTTP/1.1 200 OK
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
                                        "temperature_valid": true,
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

                var client = new RestClient(_hassConfig.HassURL);
                client.AddDefaultHeader("Authorization", "Bearer " + _hassConfig.HassAuthToken);
                var request = new RestRequest("api/states/{EntityId}", Method.Get);
                request.AddUrlSegment("EntityId", Id);
                var response = await client.ExecuteAsync(request);
                var resp_code = (int)response.StatusCode;

                if (resp_code == 200)
                {
                    string rawResponse = response.Content;
                    HassTemperatureSesnorResponse sensorData = JsonConvert.DeserializeObject<HassTemperatureSesnorResponse>(rawResponse);
                    //_logger.Log("Received sensor data: " + sensorData.ToString());
                    Value = sensorData.state;
                }
                else
                {
                    // todo: maybe some error logic for 404s / invalid configs. Nice to have.
                    _logger.Log($"Error retrieving state for sensor {Id} with status code {resp_code}");
                }
                //_logger.Log($"{response}");
            }
            catch (System.Exception e)
            {
                _logger.Log($"Error polling state of {Id} -> {e.Message}");
                _logger.Log("" + e.StackTrace);
            }

        }
    }
}

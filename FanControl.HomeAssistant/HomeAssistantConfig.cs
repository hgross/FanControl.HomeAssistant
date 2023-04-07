using System.Collections.Generic;

namespace FanControl.HomeAssistant
{
    public class HomeAssistantConfig {
        /// <summary>
        /// The auth token used to retrieve sensor data.
        /// Create one from the HomeAssistant UI https://www.home-assistant.io/docs/authentication/#your-account-profile
        /// </summary>
        /// <value></value>
        public string HomeAssistantAuthToken {get; set;}

        /// <summary>
        /// The Home Assistant URL of your installation including protocol and port.
        /// Example: http://smarthome-infrastructure.home.arpa:8123
        /// </summary>
        /// <value></value>
        public string HomeAssistantURL {get; set;}

        /// <summary>
        /// List of sensors you want to periodically retrieve.
        /// </summary>
        /// <value></value>
        public List<HomeAssistantSensorConfig> sensors {get; set;} = new List<HomeAssistantSensorConfig>();
    }

    public class HomeAssistantSensorConfig
    {
        /// <summary>
        /// Used when the configured interval is not plausible.
        /// </summary>
        public static int POLLING_INTERVAL_DEFAULT = 30;
        /// <summary>
        /// The default fallback temperature a sensor uses before it has been retrieved state data once.
        /// </summary>
        public static float DEFAULT_FALLBACK_VALUE = 20.0f;
        
        /// <summary>
        /// The home assistant entity id to resolve. Must be a tempearture sensor.
        /// Example: sensor.office_hue_motion_temperature
        /// </summary>
        /// <value></value>
        public string EntityId { get; set;}
        
        /// <summary>
        /// The interval in seconds to poll data from HomeAssistant.
        /// </summary>
        /// <value></value>
        public int PollingInterval {get; set;} = POLLING_INTERVAL_DEFAULT;

        /// <summary>
        /// An initial value that is used to initialize the sensor. 
        /// Will be overridden on first successful retrieval from HomeAssistant.
        /// Note that this value is used when home assistant is down or can't be reached initially.
        /// </summary>
        /// <value></value>
        public float InitialFallbackValue {get; set;} = DEFAULT_FALLBACK_VALUE;
    }
}

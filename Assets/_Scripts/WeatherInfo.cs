using System.Text;
using System.Xml;
using UnityEngine;

public class WeatherInfo
{
    // Example API Call Response:
    //
    // <current>
    // <city id="3163858" name="Zocca">
    // <coord lon="10.99" lat="44.34"/>
    // <country>IT</country>
    // <timezone>7200</timezone>
    // <sun rise="2022-08-30T04:36:27" set="2022-08-30T17:57:28"/>
    // </city>
    // <temperature value="298.48" min="297.56" max="300.05" unit="kelvin"/>
    // <feels_like value="298.74" unit="kelvin"/>
    // <humidity value="64" unit="%"/>
    // <pressure value="1015" unit="hPa"/>
    // <wind>
    // <speed value="0.62" unit="m/s" name="Calm"/>
    // <gusts value="1.18"/>
    // <direction value="349" code="N" name="North"/>
    // </wind>
    // <clouds value="100" name="overcast clouds"/>
    // <visibility value="10000"/>
    // <precipitation value="3.37" mode="rain" unit="1h"/>
    // <weather number="501" value="moderate rain" icon="10d"/>
    // <lastupdate value="2022-08-30T14:45:57"/>
    // </current>

    #region Data Properties

    public string CityID { get; }
    public string CityName { get; }
    public string CountryCode { get; }
    public string Timezone { get; }
    public string Sunrise { get; }
    public string Sunset { get; }

    public float Temperature { get; }
    public float MinTemperature { get; }
    public float MaxTemperature { get; }

    public float FeelsLike { get; }
    public string Humidity { get; }
    public string Pressure { get; }

    public string WindSpeed { get; }
    public string WindGusts { get; }
    public string WindDirection { get; }

    public string Clouds { get; }
    public string Visibility { get; }
    public string Precipitation { get; }
    public string WeatherNumber { get; }
    public string WeatherValue { get; }
    public string WeatherIcon { get; }

    public string LastUpdate { get; }

    #endregion

    public WeatherInfo(string xmlData)
    {
        // Parse the XML data here
        var doc = new XmlDocument();
        doc.LoadXml(xmlData);

        var cityNode = doc.SelectSingleNode("/current/city");

        if (cityNode == null)
        {
            Debug.LogError("City node is null!");
            return;
        }

        // Debug.Log($"CITY NODE: {cityNode.OuterXml}");

        CityID = cityNode.Attributes["id"].Value;
        CityName = cityNode.Attributes["name"].Value;
        CountryCode = cityNode.SelectSingleNode("country").InnerText;
        Timezone = cityNode.SelectSingleNode("timezone").InnerText;

        var sunNode = cityNode.SelectSingleNode("sun");
        Sunrise = sunNode.Attributes["rise"].Value;
        Sunset = sunNode.Attributes["set"].Value;

        var temperatureNode = doc.SelectSingleNode("/current/temperature");
        Temperature = float.Parse(temperatureNode.Attributes["value"].Value);
        MinTemperature = float.Parse(temperatureNode.Attributes["min"].Value);
        MaxTemperature = float.Parse(temperatureNode.Attributes["max"].Value);

        var feelsLikeNode = doc.SelectSingleNode("/current/feels_like");
        FeelsLike = float.Parse(feelsLikeNode.Attributes["value"].Value);

        Humidity = GetSingleNodeAttribute(doc, "/current/humidity", "value");
        Pressure = GetSingleNodeAttribute(doc, "/current/pressure", "value");

        var windNode = doc.SelectSingleNode("/current/wind");
        WindSpeed = GetSingleNodeAttribute(windNode, "speed", "value");
        WindGusts = GetSingleNodeAttribute(windNode, "gusts", "value");
        WindDirection = GetSingleNodeAttribute(windNode, "direction", "value");

        Clouds = GetSingleNodeAttribute(doc, "/current/clouds", "value");
        Visibility = GetSingleNodeAttribute(doc, "/current/visibility", "value");
        Precipitation = GetSingleNodeAttribute(doc, "/current/precipitation", "value");

        var weatherNode = doc.SelectSingleNode("/current/weather");
        WeatherNumber = weatherNode.Attributes["number"].Value;
        WeatherValue = weatherNode.Attributes["value"].Value;
        WeatherIcon = weatherNode.Attributes["icon"].Value;

        LastUpdate = doc.SelectSingleNode("/current/lastupdate").Attributes["value"].Value;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"City ID: {CityID}");
        sb.AppendLine($"City Name: {CityName}");
        sb.AppendLine($"Country Code: {CountryCode}");

        sb.AppendLine($"Timezone: {Timezone}");
        sb.AppendLine($"Sunrise: {Sunrise}");
        sb.AppendLine($"Sunset: {Sunset}");

        sb.AppendLine($"Temperature: {KelvinToFahrenheit(Temperature)} Fahrenheit");
        sb.AppendLine($"Min Temperature: {KelvinToFahrenheit(MinTemperature)} Fahrenheit");
        sb.AppendLine($"Max Temperature: {KelvinToFahrenheit(MaxTemperature)} Fahrenheit");

        sb.AppendLine($"Feels Like: {FeelsLike}");
        sb.AppendLine($"Humidity: {Humidity}");
        sb.AppendLine($"Pressure: {Pressure}");

        // sb.AppendLine($"Wind Speed: {WindSpeed}");
        // sb.AppendLine($"Wind Gusts: {WindGusts}");
        // sb.AppendLine($"Wind Direction: {WindDirection}");

        sb.AppendLine($"Clouds: {Clouds}");
        sb.AppendLine($"Visibility: {Visibility}");
        sb.AppendLine($"Precipitation: {Precipitation}");

        // sb.AppendLine($"Weather Number: {WeatherNumber}");
        sb.AppendLine($"Weather: {WeatherValue}");
        // sb.AppendLine($"Weather Icon: {WeatherIcon}");

        sb.AppendLine($"Last Update: {LastUpdate}");

        return sb.ToString();
    }

    private static string GetSingleNodeAttribute(XmlNode node, string nodePath, string attributeName)
    {
        var docNode = node.SelectSingleNode(nodePath);

        if (docNode == null)
            return string.Empty;

        var attribute = docNode.Attributes[attributeName];

        if (attribute == null)
            return string.Empty;

        var value = attribute.Value;

        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value;
    }

    private static float KelvinToFahrenheit(float kelvin)
    {
        return kelvin * 9 / 5 - 459.67f;
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class WeatherManagerController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private TMP_Text currentCityText;
    [SerializeField] private TMP_Text weatherInfoText;

    [Header("Sun Light Settings")] [SerializeField]
    private Light sunLight;

    [SerializeField] private Color nightColor;
    [SerializeField] private Color dayColor;
    [SerializeField] private float minIntensity = 5f;
    [SerializeField] private float maxIntensity = 120f;

    [Header("Skybox Materials")] [SerializeField]
    private Material clearSky;

    [SerializeField] private Material cloudySky;
    [SerializeField] private Material rainySky;
    [SerializeField] private Material snowySky;

    /// <summary>
    /// List of the different cities that we want to get the weather data for.
    /// </summary>
    [SerializeField] private CityInfo[] cities;

    #endregion

    #region Private Fields

    /// <summary>
    /// Dictionary to keep track of the weather data for each city.
    /// This is used so that we are not constantly making API calls to get the weather data.
    /// </summary>
    private Dictionary<CityInfo, WeatherInfo> _weatherData = new();

    /// <summary>
    /// An instance of WeatherManager that will be used to get the weather data.
    /// </summary>
    private WeatherManager _weatherManager;

    /// <summary>
    /// The index of the city that we are currently getting the weather data for.
    /// </summary>
    private int _cityIndex = 0;

    #endregion

    /// <summary>
    /// The current city that we are getting the weather data for.
    /// </summary>
    private CityInfo CurrentCity => cities[_cityIndex];

    private void Awake()
    {
        // Create an instance of WeatherManager
        _weatherManager = new WeatherManager();
    }

    private void Update()
    {
        // Update the input to change the city index
        UpdateInput();

        // Update the text elements on screen
        UpdateText();

        // Update the weather effects
        if (_weatherData.ContainsKey(CurrentCity))
            SetWeatherEffects(_weatherData[CurrentCity]);
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetCityData(CurrentCity);

        // Change the city index based on the arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousCity();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextCity();
    }

    private void UpdateText()
    {
        // Update the current city text
        currentCityText.text = $"{CurrentCity.CityName}, {CurrentCity.CountryCode}";

        // Update the weather info text
        if (_weatherData.ContainsKey(CurrentCity))
            SetWeatherInfoText(_weatherData[CurrentCity]);
        else
            weatherInfoText.text = "No weather data available!\nPress SPACE to get weather data.";
    }

    private void OnXMLDataLoaded(string data)
    {
        Debug.Log(data);

        // Parse the data for the weather
        var weatherInfo = new WeatherInfo(data);

        // Add the weather data to the dictionary
        _weatherData[CurrentCity] = weatherInfo;

        // Log the weather data
        Debug.Log(weatherInfo.ToString());
    }

    private void GetCityData(CityInfo cityInfo)
    {
        // Check if we already have the weather data for this city
        if (_weatherData.ContainsKey(cityInfo))
        {
            Debug.Log($"Weather data for {cityInfo.CityName} already exists.");
            return;
        }

        // Get the weather data for the city
        StartCoroutine(_weatherManager.GetWeatherXML(CurrentCity, OnXMLDataLoaded));
    }

    private void SetWeatherInfoText(WeatherInfo weatherInfo)
    {
        weatherInfoText.text = weatherInfo.ToString();
    }

    private void SetWeatherEffects(WeatherInfo weatherInfo)
    {
        // Set the sunlight based on the weather data
        SetSunlight(weatherInfo);

        // Set the skybox based on the weather data
        SetSkybox(weatherInfo);
    }

    private void SetSunlight(WeatherInfo weatherInfo)
    {
        // Set the light color based on the time of day

        // Get the current time
        var currentTime = DateTime.Now;

        // Parse the timezone difference
        var timezoneDiff = int.Parse(weatherInfo.Timezone) / 3600;

        // Adjust the time based on the timezone of the city
        currentTime = currentTime.AddHours(timezoneDiff);

        // Get the sunrise and sunset times
        var sunriseTime = DateTime.Parse(weatherInfo.Sunrise);

        var sunsetTime = DateTime.Parse(weatherInfo.Sunset);

        // Check if it is nighttime
        if (currentTime < sunriseTime || currentTime > sunsetTime)
            sunLight.color = nightColor;

        else
            sunLight.color = dayColor;

        // Set the intensity of the lights based on the temperature
        const float minTemp = 32;
        const float maxTemp = 100;

        // Get the fahrenheit temperature
        var tempF = WeatherInfo.KelvinToFahrenheit(weatherInfo.Temperature);

        // Normalize the temperature
        var normalizedTemp = Mathf.InverseLerp(minTemp, maxTemp, tempF);

        // Set the intensity of the light based on the temperature
        sunLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedTemp);
    }

    private void SetSkybox(WeatherInfo weatherInfo)
    {
        // Set the skybox material based on the weather data
        switch (weatherInfo.WeatherIcon.Substring(0, 2))
        {
            case "01":
                RenderSettings.skybox = clearSky;
                break;

            case "02":
            case "03":
            case "04":
                RenderSettings.skybox = cloudySky;
                break;

            case "09":
            case "10":
            case "11":
            case "50":
                RenderSettings.skybox = rainySky;
                break;

            case "13":
                RenderSettings.skybox = snowySky;
                break;

            default:
                RenderSettings.skybox = clearSky;
                break;
        }
    }

    public void NextCity()
    {
        _cityIndex = (_cityIndex + 1) % cities.Length;
    }

    public void PreviousCity()
    {
        _cityIndex = (_cityIndex - 1 + cities.Length) % cities.Length;
    }
}
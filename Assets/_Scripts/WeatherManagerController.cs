using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class WeatherManagerController : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// List of the different cities that we want to get the weather data for.
    /// </summary>
    [SerializeField] private CityInfo[] cities;


    [SerializeField] private TMP_Text currentCityText;
    [SerializeField] private TMP_Text weatherInfoText;

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
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetCityData(CurrentCity);

        // Change the city index based on the arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            _cityIndex = (_cityIndex - 1 + cities.Length) % cities.Length;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            _cityIndex = (_cityIndex + 1) % cities.Length;
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
}
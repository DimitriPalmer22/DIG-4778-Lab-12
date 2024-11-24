using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class WeatherManagerController : MonoBehaviour
{
    /// <summary>
    /// List of the different cities that we want to get the weather data for.
    /// </summary>
    [SerializeField] private CityInfo[] cities;

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
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(_weatherManager.GetWeatherXML(CurrentCity, OnXMLDataLoaded));

        // Change the city index based on the arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            _cityIndex = (_cityIndex - 1 + cities.Length) % cities.Length;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            _cityIndex = (_cityIndex + 1) % cities.Length;

        Debug.Log($"Current City: {CurrentCity.CityName}, {_cityIndex}");
    }

    private void OnXMLDataLoaded(string data)
    {
        Debug.Log(data);
    }
}
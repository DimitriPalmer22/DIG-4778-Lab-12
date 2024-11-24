using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager
{
    private const string apiKey = "4a6ff515e0366186752fd3385a05bc9f";

    private string xmlApi = $"http://api.openweathermap.org/data/2.5/weather?q=Orlando,us&mode=xml&appid={apiKey}";

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        using var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                Debug.LogError($"network problem: {request.error}");
                break;

            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError($"response error: {request.responseCode}");
                break;

            default:
                callback(request.downloadHandler.text);
                break;
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, callback);
    }
}
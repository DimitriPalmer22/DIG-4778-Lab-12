using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager
{
    private const string apiKey = "4a6ff515e0366186752fd3385a05bc9f";

    private static Dictionary<string, Texture2D> _images = new();

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        Debug.Log($"Requesting data from {url}");

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

    public IEnumerator GetWeatherXML(CityInfo cityInfo, Action<string> callback)
    {
        var apiString =
            $"http://api.openweathermap.org/data/2.5/weather?q={cityInfo.CityName},{cityInfo.CountryCode}&mode=xml&appid={apiKey}";

        return CallAPI(apiString, callback);
    }

    private static IEnumerator DownloadImage(string webImage, Action<Texture2D> callback)
    {
        using var request = UnityWebRequestTexture.GetTexture(webImage);

        yield return request.SendWebRequest();

        // Halt while the image is being downloaded
        while (!request.isDone)
            yield return null;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to download image: {request.error}");
            yield break;
        }

        callback(DownloadHandlerTexture.GetContent(request));
    }

    public static void GetWebImage(MonoBehaviour script, string address, Action<Texture2D> callback)
    {
        // If the image is not already in the dictionary, download it
        if (!_images.TryGetValue(address, out var image))
        {
            script.StartCoroutine(DownloadImage(address, texture2D =>
            {
                // Get the image and add it to the dictionary
                _images[address] = texture2D;

                // Then, call the callback
                callback(texture2D);
            }));
        }

        // Otherwise, call the callback with the image from the dictionary
        else
            callback(image);
    }
}
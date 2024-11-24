using UnityEngine;

[CreateAssetMenu(fileName = "CityInfo", menuName = "City Info")]
public class CityInfo : ScriptableObject
{
    [SerializeField] private string cityName;
    [SerializeField] private string countryCode;

    public string CityName => cityName;
    public string CountryCode => countryCode;
}
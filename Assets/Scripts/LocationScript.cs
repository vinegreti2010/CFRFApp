using System.Collections;
using UnityEngine;

public class LocationScript : MonoBehaviour {
    private Coroutine startLocationRoutine, getLocationRoutine;
    private float latitude, longitude, accuracy;
    private int timesCollected;
    private GameObject gpsNotFound;
    public bool hasCoordinate { get; set; }

    // Use this for initialization
    void Start() {
        gpsNotFound = GameObject.Find("gpsNotFound");
        gpsNotFound.SetActive(false);
        latitude = 0;
        longitude = 0;
        accuracy = 0;
        timesCollected = 0;
        startLocationRoutine = null;
        getLocationRoutine = null;
        hasCoordinate = false;
        startLocationRoutine = StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService() {
        yield return new WaitForSeconds(2);
        Debug.Log(Input.location.status);
        while(Input.location.status != LocationServiceStatus.Running) {
            Debug.Log(Input.location.status + " - " + Input.location.isEnabledByUser);
            gpsNotFound.SetActive(true);
            // Verifica se o serviço de localização do dispositivo está ativo

            if(!Input.location.isEnabledByUser)
                yield return new WaitForSeconds(1);

            // Start service before querying location
            Input.location.Start();
            yield return new WaitForSeconds(1);

            //// Wait until service initializes
            //int maxWait = 20;
            //while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            //    yield return new WaitForSeconds(1);
            //    maxWait--;
            //}

            //// Service didn't initialize in 20 seconds
            //if(maxWait < 1) {
            //    yield return new WaitForSeconds(1);
            //}

            //// Connection has failed
            //if(Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped) {
            //    yield return new WaitForSeconds(1);
            //}
        }
        Debug.Log(Input.location.status);
        gpsNotFound.SetActive(false);
        // Access granted and location value could be retrieved
        getLocationRoutine = StartCoroutine(IncrementLocation());
        StopCoroutine(startLocationRoutine);
    }

    IEnumerator IncrementLocation() {
        if(Input.location.status == LocationServiceStatus.Running && Input.location.isEnabledByUser) {
            hasCoordinate = true;
            while(true) {
                yield return new WaitForSeconds(1);
                latitude += Input.location.lastData.latitude;
                longitude += Input.location.lastData.longitude;
                accuracy += Input.location.lastData.horizontalAccuracy;
                ++timesCollected;
            }
        } else {
            startLocationRoutine = StartCoroutine(StartLocationService());
            StopCoroutine(getLocationRoutine);
        }
    }

    public void StopLocation() {
        StopCoroutine(getLocationRoutine);
    }

    public float GetLatitude() {
        return timesCollected == 0 ? 0.0f : latitude / timesCollected;
    }

    public float GetLongitude() {
        return timesCollected == 0 ? 0.0f : longitude / timesCollected;
    }

    public float GetAccuracy() {
        return timesCollected == 0 ? 0.0f : accuracy / timesCollected;
    }

    private void OnButtonPressed(string _buttonPressed) {
        Debug.Log("Button pressed: " + _buttonPressed);
    }
}
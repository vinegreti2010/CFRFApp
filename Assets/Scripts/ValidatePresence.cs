using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ValidatePresence : MonoBehaviour {
    private InputField StudentCodeInput;
    private Toggle remember;
    CameraScript photoObject;
    NetworkScript network;
    LocationScript location;
    Informations informations;

    void Start() {
        StudentCodeInput = GameObject.Find("StudentCodeInput").GetComponent<InputField>();
        remember = GameObject.Find("RememberCheckBox").GetComponent<Toggle>();
        photoObject = GameObject.Find("TakePicButton").GetComponent<CameraScript>();
        network = gameObject.GetComponent<NetworkScript>();
        location = gameObject.GetComponent<LocationScript>();
        informations = new Informations();
    }

    private void Update() {
        if(photoObject.HasPhoto() && !string.IsNullOrEmpty(StudentCodeInput.text) && location.hasCoordinate && network.hasNetwork) {
            if(StudentCodeInput.text.Length == StudentCodeInput.characterLimit)
                gameObject.GetComponent<Button>().interactable = true;
            else
                gameObject.GetComponent<Button>().interactable = false;
        } else
            gameObject.GetComponent<Button>().interactable = false;
    }

    public void Clicked() {
        location.StopLocation();

        informations.Code = StudentCodeInput.text;
        informations.Photo = photoObject.GetPhotos(0);
        informations.Photo1 = photoObject.GetPhotos(1);
        informations.Photo2 = photoObject.GetPhotos(2);
        informations.Latitude = location.GetLatitude();
        informations.Longitude = location.GetLongitude();
        informations.Accuracy = location.GetAccuracy();

        if(remember.isOn == true)
            PlayerPrefs.SetString("StudentCode", StudentCodeInput.text);
        else if(StudentCodeInput.text == PlayerPrefs.GetString("StudentCode", ""))
            PlayerPrefs.DeleteKey("StudentCode");

        photoObject.DestroyObject();
        WebRequest request;
        try {
            request = network.SendMessage(informations);

            NPBinding.UI.ShowToast("informações enviadas", VoxelBusters.NativePlugins.eToastMessageLength.SHORT);

            ResponseInfo response = network.GetWebResponse(request);
            ResponseHandler(response);
        } catch(Exception e) {
            Debug.Log(e.Message);
            NPBinding.UI.ShowAlertDialogWithSingleButton("Erro", e.Message, "Ok", OnButtonPressed);
        }
    }

    public void PrintServerMessage(string serverMessage) {
        NPBinding.UI.ShowAlertDialogWithSingleButton("Status", serverMessage, "OK", OnButtonPressed);
    }

    private void ResponseHandler(ResponseInfo responseInfo) {
        //if(response.StatusCode == HttpStatusCode.OK) {
       
        
        NPBinding.UI.ShowAlertDialogWithSingleButton(responseInfo.header, responseInfo.message, "Ok", OnButtonPressed);
        //}
    }

    private void OnButtonPressed(string _buttonPressed) {
        Debug.Log("Button pressed: " + _buttonPressed);
    }
}
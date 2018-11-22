using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {
    public RawImage background;
    public AspectRatioFitter fit;
    private bool cameraAvailable, firstInstruction = true, isRoutineRunning = false;
    private WebCamTexture frontCamera;
    Texture2D photo = null;
    List<Texture2D> photos = null;
    Coroutine takePicsRoutine = null;

    public void OpenCamera() {
        background.enabled = true;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0) {
            NPBinding.UI.ShowAlertDialogWithSingleButton("Erro", "Nenhuma camera encontrada", "Ok", OnButtonPressed);
            Debug.Log("No Camera Detected");
            cameraAvailable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++) {
            if(devices[i].isFrontFacing)
                frontCamera = new WebCamTexture(devices[i].name, 400, 750);
        }

        if(frontCamera == null) {
            NPBinding.UI.ShowAlertDialogWithSingleButton("Erro", "Camera frontal não encontrada", "Ok", OnButtonPressed);
            Debug.Log("Unable to find frontal camera");
            return;
        }

        if(firstInstruction) {
            NPBinding.UI.ShowAlertDialogWithSingleButton("Instrução", "Toque para capturar a imagem", "Ok", OnButtonPressed);
            firstInstruction = false;
        }

        frontCamera.Play();
        background.texture = frontCamera;

        cameraAvailable = true;
        try {
            DestroyObject();
        } catch { }
    }

    private void Update() {
        if(background.enabled == true) {
            if(!cameraAvailable)
                return;

            float ratio = (float)frontCamera.width / (float)frontCamera.height;
            fit.aspectRatio = ratio;

            float scaleY = frontCamera.videoVerticallyMirrored ? -1f : 1f;
            background.rectTransform.localScale = new Vector3(1f, -scaleY, -1f);

            int orient = frontCamera.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);

            if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetKey(KeyCode.E))) {
                TakePic();
                /*if(!isRoutineRunning) {
                    background.enabled = false;
                    frontCamera.Stop();
                }*/
                return;
            }
        }
    }

    private void TakePic() {
        if(photos == null)
            photos = new List<Texture2D>();
        photo = new Texture2D(frontCamera.width, frontCamera.height);

        //for(int i = 0; i < 3; i++) {
            if(frontCamera.isPlaying) {
                photo.SetPixels(frontCamera.GetPixels());
                photo.Apply();
                photos.Add(photo);
                isRoutineRunning = true;
            //Thread.Sleep(700);
            takePicsRoutine = StartCoroutine(TakeAnotherPics());
        } else
                Debug.Log("erro");
        //}
    }

    public bool HasPhoto() {
        return (photos == null || photos.Count < 3) ? false : true;
    }

    public byte[] GetPhotos(int i) {
        //Encode to PNG
        byte[] imageBytes = photos[i].EncodeToJPG();
        Debug.Log(i + " - " + imageBytes.Length);
        return imageBytes;
    }

    public void DestroyObject() {
        photo = null;
        photos.Clear();
        photos = null;
    }

    private IEnumerator TakeAnotherPics() {
        photo = new Texture2D(frontCamera.width, frontCamera.height);
        //yield return new WaitForSeconds(0.3f);
        for(int i = 0; i < 2; i++) {
            photo.SetPixels(frontCamera.GetPixels());
            photo.Apply();
            photos.Add(photo);
            yield return new WaitForSeconds(1);
        }
        
        isRoutineRunning = false;
        background.enabled = false;
        frontCamera.Stop();
        StopCoroutine(takePicsRoutine);
    }

    private void OnButtonPressed(string _buttonPressed) {
        Debug.Log("Button pressed: " + _buttonPressed);
    }
}
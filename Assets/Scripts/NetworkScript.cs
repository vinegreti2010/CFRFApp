using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class NetworkScript : MonoBehaviour {

    private WebRequest webRequest;
    Coroutine checkAvailable;
    private GameObject netNotFound;
    public bool hasNetwork { get; set; }
    public void Start() {
        netNotFound = GameObject.Find("netNotFound");
        checkAvailable = StartCoroutine(IsNetworkOn());
        netNotFound.SetActive(false);
    }

    private IEnumerator IsNetworkOn() {
        while(true) {
            if(Application.internetReachability == NetworkReachability.NotReachable) {
                netNotFound.SetActive(true);
                hasNetwork = false;
                yield return new WaitForSeconds(1);
            } else {
                netNotFound.SetActive(false);
                hasNetwork = true;
                yield return new WaitForSeconds(3);
            }
        }
    }

    public WebRequest SendMessage(Informations informations) {
        string jSonString = JsonUtility.ToJson(informations, false);

        if(String.IsNullOrEmpty(jSonString))
            throw new Exception("1");

        try {
            PreparaWebRequest("CFRF", "POST");

            using(var streamWriter = new StreamWriter(webRequest.GetRequestStream())) {
                streamWriter.Write(jSonString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            return webRequest;
        } catch {
            throw new Exception("Não foi possível enviar as informações");
        }
    }

    private void PreparaWebRequest(string operacao, string metodo) {
        string url = "http://192.168.0.104:62761/"; //Local
        //string url = "https://cfrfapp.azurewebsites.net/"; //Azure Server
        try {
            webRequest = WebRequest.Create(url + operacao);
            webRequest.Method = metodo;
            webRequest.ContentType = "application/json; charset=utf-8";
        } catch {
            throw new Exception("Servidor inacessível, favor contatar o suporte");
        }
    }

    public ResponseInfo GetWebResponse(WebRequest webRequest) {
        try {
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            var responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();
            responseText = responseText.Replace("\\", "");
            responseText = responseText.Substring(1, responseText.Length - 2);
            ResponseInfo responseInfo = JsonUtility.FromJson<ResponseInfo>(responseText);
            return responseInfo;
        } catch(Exception e) {
            Debug.Log(e.Message);
            throw new Exception("Não foi receber resopsta do servidor");
        } finally {
            webRequest.Abort();
            StopCoroutine(checkAvailable);
        }
    }
}
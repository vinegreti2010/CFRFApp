using UnityEngine;
using UnityEngine.UI;

public class StudentCode : MonoBehaviour {
    // Use this for initialization
    void Start () {
        InputField StudentCodeInput;
        StudentCodeInput = gameObject.GetComponent<InputField>();
        StudentCodeInput.onEndEdit.AddListener(delegate { LockInput(StudentCodeInput); });
        string savedCode = PlayerPrefs.GetString("StudentCode", "");
        if(savedCode != "") {
            gameObject.GetComponent<InputField>().text = savedCode;
        }
    }

    void LockInput(InputField inputField) {
        string studentCode = inputField.text;

        if (studentCode.Substring(0, 1) == "-" || studentCode.Length < 12) {
            NPBinding.UI.ShowAlertDialogWithSingleButton("Alerta", "O código deve conter todos os números da sua carteirinha de estudante, e apenas os números", "Ok", OnButtonPressed);
            return;
        }
    }

    private void OnButtonPressed(string _buttonPressed) {
        Debug.Log("Button pressed: " + _buttonPressed);
    }
}

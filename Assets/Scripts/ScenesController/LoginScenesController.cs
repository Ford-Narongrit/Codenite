using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginScenesController : MonoBehaviour
{
    public string nextSceneName = "HomeScene";
    public TMP_InputField username;
    public TMP_InputField password;
    public GameObject loadingPanel;
    private int inputSelected;
    public void login()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(APIHelper.instance.login(username.text, password.text,
        () =>
        {
            loadingPanel.SetActive(false);
            SceneManager.LoadScene(nextSceneName);
        },
        () =>
        {
            loadingPanel.SetActive(false);
            AlertController.Instance.showAlert("Login Fail",
            "username or password is incorrect.",
            "Close", () =>
            {
                password.text = "";
            });
        }
        ));
    }

    public void OncilckRegister()
    {
        Application.OpenURL(ENVConfig.homepage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            inputSelected--;
            if (inputSelected < 0) inputSelected = 1;
            SelectInputField();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            inputSelected++;
            if (inputSelected > 1) inputSelected = 0;
            SelectInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            login();
        }
    }

    private void SelectInputField()
    {
        switch (inputSelected)
        {
            case 0:
                username.Select();
                break;
            case 1:
                password.Select();
                break;
        }
    }

    public void usernameSelected() => inputSelected = 0;
    public void passwordSelected() => inputSelected = 1;
}

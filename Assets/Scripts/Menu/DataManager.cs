using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    [SerializeField] Text nameBox;

    public static DataManager Instance;

    public string playerName = "";

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
		}

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }

	//Sets the playerName
	public void UpdateName() {
        playerName = nameBox.text;
	}
}

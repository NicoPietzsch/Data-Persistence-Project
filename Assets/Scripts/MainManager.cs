using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour {
	public Brick BrickPrefab;
	public int LineCount = 6;
	public Rigidbody Ball;

	public Text HighScoreText;
	public Text ScoreText;
	public GameObject GameOverText;

	private bool m_Started = false;
	private int m_Points;
	private int highScore = 0;

	private string playerName;
	private bool m_GameOver = false;


	// Start is called before the first frame update
	void Start() {
		LoadScore();

		playerName = DataManager.Instance.playerName;
		ScoreText.text = $"Score : {m_Points} Player : {playerName}";

		const float step = 0.6f;
		int perLine = Mathf.FloorToInt(4.0f / step);

		int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
		for (int i = 0; i < LineCount; ++i) {
			for (int x = 0; x < perLine; ++x) {
				Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
				var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
				brick.PointValue = pointCountArray[i];
				brick.onDestroyed.AddListener(AddPoint);
			}
		}
	}

	private void Update() {
		if (!m_Started) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				m_Started = true;
				float randomDirection = Random.Range(-1.0f, 1.0f);
				Vector3 forceDir = new Vector3(randomDirection, 1, 0);
				forceDir.Normalize();

				Ball.transform.SetParent(null);
				Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
			}
		} else if (m_GameOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (highScore < m_Points) {
					SaveScore();
				}
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}

	void AddPoint(int point) {
		m_Points += point;
		ScoreText.text = $"Score : {m_Points} Player : {playerName}";

		if (m_Points > highScore) {
			HighScoreText.text = $"Score : {m_Points} Player : {playerName}";
		}
	}

	public void GameOver() {
		m_GameOver = true;
		GameOverText.SetActive(true);
	}

	private void OnApplicationQuit() {
		if (highScore < m_Points) {
			SaveScore();
		}
	}

	public void SaveScore() {
		SaveData data = new SaveData();
		data.score = m_Points;
		data.playerName = DataManager.Instance.playerName;

		string json = JsonUtility.ToJson(data);

		File.WriteAllText(Application.persistentDataPath + "/highscore.json", json);
	}

	public void LoadScore() {
		string path = Application.persistentDataPath + "/highscore.json";
		if (File.Exists(path)) {
			string json = File.ReadAllText(path);
			SaveData data = JsonUtility.FromJson<SaveData>(json);

			highScore = data.score;
			HighScoreText.text = $"Best Score : {highScore} Player : {data.playerName}";
		}
	}

	[System.Serializable]
	class SaveData {
		public int score;
		public string playerName;
	}
}

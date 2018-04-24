using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour {

    private string connectionString;

    private List<HighScore> highscores = new List<HighScore>();

    public GameObject scorePrefab;

    public Transform scoreParent;

    public int topRanks;

    public int savedScores;

    public InputField enterName;

    public GameObject nameDialog;

	// Use this for initialization
	void Start () {
        connectionString  = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";

       // InsertScore("potato", 70);
        
        DeleteExtraScores();

        ShowScores();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            nameDialog.SetActive(!nameDialog.activeSelf);
        }
	}
    public void EnterName()
    {
        if (enterName.text != string.Empty)
        {
            int score = UnityEngine.Random.Range(1, 50);
            InsertScore(enterName.text, score);
            enterName.text = string.Empty;

            ShowScores();
        }
    }

    private void InsertScore(string name, int newScore)
    {
        GetScores();
        int hsCount = highscores.Count;

        if(highscores.Count > 0)
        {
            HighScore lowestScore = highscores[highscores.Count - 1];
            if (lowestScore != null && savedScores > 0 && highscores.Count >= savedScores && newScore > lowestScore.Score)
            {
                DeleteScore(lowestScore.ID);
                hsCount--;
            }
        }
        if (hsCount < savedScores)
        {
            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();

                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    string sqlQuery = String.Format("INSERT INTO HighScores(Name,LevelsCompleted) VALUES(\"{0}\",\"{1}\")", name, newScore);

                    dbCmd.CommandText = sqlQuery;
                    dbCmd.ExecuteScalar();
                    dbConnection.Close();


                }

            }
        }
        

    }

    private void GetScores()
    {
        highscores.Clear();
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd =dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM HighScores";

                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highscores.Add(new HighScore(reader.GetInt32(0), reader.GetInt32(2), reader.GetString(1), reader.GetDateTime(3)));
                        
                    }

                    dbConnection.Close();
                    reader.Close();
                }
            }

        }

        highscores.Sort();
    }

    private void DeleteScore(int id)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", id);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();


            }

        }
    }

    private void ShowScores()
    {
        GetScores();

        foreach (GameObject score in GameObject.FindGameObjectsWithTag("Score"))
        {
            Destroy(score);
        }

        for (int i = 0; i < topRanks; i++)
        {
            if (i <= highscores.Count - 1)
            {
                GameObject tmpObject = Instantiate(scorePrefab);

                HighScore tmpScore = highscores[i];

                tmpObject.GetComponent<HighScoreScript>().SetScore(tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString());

                tmpObject.transform.SetParent(scoreParent);

                tmpObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void DeleteExtraScores()
    {
        GetScores();

        if (savedScores <= highscores.Count)
        {
            int deleteCount = highscores.Count - savedScores;
            highscores.Reverse();

            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();

                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    for (int i = 0; i < deleteCount; i++)
                    {
                        string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID = \"{0}\"", highscores[i].ID);
                       
                        dbCmd.CommandText = sqlQuery;
                        dbCmd.ExecuteScalar();
                        
                    }
                    dbConnection.Close();


                }

            }
        }
    }
}

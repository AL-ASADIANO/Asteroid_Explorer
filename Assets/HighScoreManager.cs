using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public class HighScoreManager : MonoBehaviour {

    private string connectionString;

    private List<HighScore> highscores = new List<HighScore>();

    public GameObject scorePrefab;

    public Transform scoreParent;

    public int topRanks;

    public int savedScores;

	// Use this for initialization
	void Start () {
        connectionString  = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";
        
        DeleteExtraScores();

        ShowScores();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void InsertScore(string name, int newScore)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("INSERT INTO HighScores(Name,LevelsCompleted) VALUES(\"{0}\",\"{1}\")",name,newScore);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();

               
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

using System;
using System.Data;
using System.Collections;
using Mono.Data.Sqlite;
using UnityEngine;

public class HighScoreManager : MonoBehaviour {

    private string connectionString;

	// Use this for initialization
	void Start () {
        connectionString  = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";
        InsertScore("Duncan", 0);
        DeleteScore(2);
        GetScores();


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
                        Debug.Log(reader.GetString(1) + reader.GetInt32(2));
                        
                    }

                    dbConnection.Close();
                    reader.Close();
                }
            }

        }

        
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
}

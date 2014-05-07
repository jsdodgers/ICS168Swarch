
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

namespace Swarch {
public class SQLiteDB
{

	SqliteConnection dbcon = null;
	public SQLiteDB ()
	{

	}

	public void dbPrintAll() {
		SqliteCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT username, password FROM users";
		dbcmd.CommandText = sql;
		SqliteDataReader reader = dbcmd.ExecuteReader();
		string s = "";
		while (reader.Read()) {
			string username = reader.GetString(0);
			string password = reader.GetString(1);
			string t = "Username: " + username + "  Password: " + password;
			s += t + "\n";
		}
		Debug.Log(s);
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
	}

	public string getPassword(string username) {
		SqliteCommand dbcmd = dbcon.CreateCommand();
		string sql = "SELECT password FROM users WHERE username=?";
		dbcmd.CommandText = sql;
		SqliteParameter param = new SqliteParameter();
		param.Value = username;
		dbcmd.Parameters.Add(param);
		SqliteDataReader reader = dbcmd.ExecuteReader();
		string s = null;
		while (reader.Read()) {
			s = reader.GetString(0);
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		return s;
	}

	public bool addUser(string username, string password) {
		SqliteCommand dbcmd = dbcon.CreateCommand();
		string sql = "INSERT INTO users VALUES (?,?);";
		dbcmd.CommandText = sql;
		SqliteParameter param1 = new SqliteParameter();
		SqliteParameter param2 = new SqliteParameter();
		param1.Value = username;
		param2.Value = password;
		dbcmd.Parameters.Add(param1);
		dbcmd.Parameters.Add(param2);
		SqliteDataReader reader = dbcmd.ExecuteReader();
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		return true;
	}

	public bool dbConnect(string database) {
		if (dbcon!=null) dbDisconnect();
		string connectionString = "URI=file:Assets/Databases/" + database;
		dbcon = new SqliteConnection(connectionString);
		dbcon.Open();
		if (dbcon!=null) return true;
		return false;

	}

	public void dbDisconnect() {
		dbcon.Close();
		dbcon = null;
	}

}
}


using System;
using Mono.Data.Sqlite;
//using System.Data.Sqlite;
using System.Data;

namespace SwarchServer
{
    public class SqliteDB
    {
	    SqliteConnection dbcon = null;
	    public SqliteDB ()
	    {
            
	    }

	    // for debugging purposes
	    public void dbPrintAll() {
		    SqliteCommand dbcmd = dbcon.CreateCommand();
		    string Sql = "SELECT username, password FROM users";
		    dbcmd.CommandText = Sql;
		    SqliteDataReader reader = dbcmd.ExecuteReader();
		    string s = "";
		    while (reader.Read()) {
			    string username = reader.GetString(0);
			    string password = reader.GetString(1);
			    string t = "Username: " + username + "  Password: " + password;
			    s += t + "\n";
		    }
            Console.WriteLine(s);
		    reader.Close();
		    reader = null;
		    dbcmd.Dispose();
		    dbcmd = null;
	    }

	    // this retrieves the password from the database to
	    // compare in another method to the password that the user inputs
	    public string getPassword(string username) {
		    SqliteCommand dbcmd = dbcon.CreateCommand();
		    string Sql = "SELECT password FROM users WHERE username=?";
		    dbcmd.CommandText = Sql;
		    SqliteParameter param = new SqliteParameter();
		    param.Value = username;
		    dbcmd.Parameters.Add(param);
		    SqliteDataReader reader = dbcmd.ExecuteReader();
		    string s = null;
		    while (reader.Read()) {
			    s = reader.GetString(0);
		    }

		    // for security reasons...
		    // close reader
		    reader.Close();
		    reader = null;
		    // dispose of database commands
		    dbcmd.Dispose();
		    dbcmd = null;

		    return s;
	    }

	    // add users to the database
	    public bool addUser(string username, string password) {
		    SqliteCommand dbcmd = dbcon.CreateCommand();
		    string Sql = "INSERT INTO users VALUES (?,?);";
		    dbcmd.CommandText = Sql;
		    SqliteParameter param1 = new SqliteParameter();
		    SqliteParameter param2 = new SqliteParameter();
		    param1.Value = username;
		    param2.Value = password;
		    dbcmd.Parameters.Add(param1);
		    dbcmd.Parameters.Add(param2);
		    SqliteDataReader reader = dbcmd.ExecuteReader();


		    reader.Close();
		    reader = null;
		    // dispose of database commands
		    dbcmd.Dispose();
		    dbcmd = null;

		    // should only be called if conditions are right
		    // for adding users; always return true
		    return true;
	    }

	    // finds the database file on the user's harddrive to open
	    public bool dbConnect(string database) {
		    if (dbcon!=null) dbDisconnect();
            string connectionString = "Data Source=" + database;
		    dbcon = new SqliteConnection(connectionString);
            //dbcon.Open();
		    if (dbcon!=null) return true;
		    return false;

	    }

	    // closes database file
	    public void dbDisconnect() {
		//    dbcon.Close();
		    dbcon = null;
	    }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // For mySql connection

namespace ConnectToDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            // Om det inte är lokalt skrivs en ip-adress istället för localhost
            string server = "localhost";
            string userid = "root";
            string password = "Pa55word";
            string database = "kurs1";

            //string server = "172.16.117.80";
            //string userid = "root";
            //string password = "Pa55w.rd";
            //string database = "netkurs";

            // connection string
            string cs = "server=" + server + ";userid=" + userid + ";password=" + password + ";database=" + database;

                // connection, create and open
                var con = new MySqlConnection(cs);
                con.Open();

                // try connection
                Console.WriteLine($"MySql version: {con.ServerVersion}");


            // call create function
            createPerson(con);

            // call update function
            // int Id = 4;
            // changePerson(con, Id);

            // call delete function
            // int Id = 7;
            // deletePerson(con, Id);

            // call read function
            // readPerson(con);

            Console.ReadKey();

        }

        static void deletePerson(MySqlConnection connection, int Id)
        {
            // SQL query string
            string sql = "DELETE FROM person WHERE Id =" + Id;

            // command, request with sql query and sql connection. 
            // cmd = response from db
            var cmd = new MySqlCommand(sql, connection);

            // execute query
            cmd.BeginExecuteNonQuery();
        }

        static void changePerson(MySqlConnection connection, int Id)
        {

            // SQL query string
            string sql = "UPDATE person SET Firstname = 'Johnny', Age = 10 WHERE Id =" + Id;

            // command, request with sql query and sql connection. 
            // cmd = response from db
            var cmd = new MySqlCommand(sql, connection);

            // execute query
            cmd.BeginExecuteNonQuery();

        }

        static void createPerson(MySqlConnection connection)
        {
            // SQL query string
            //string sql = "INSERT INTO person(Firstname, Lastname, Age, City, Ip_adress) VALUES('Jo', 'Doe', 45, 'Sthlm', '172.66.00.00')";

            string sql = "INSERT INTO chat_history(date, senderIP, recipientIP, nickname, message) VALUES('datum', 'ngot', 'något', 'hej', 'hej')";

            // command, request with sql query and sql connection. 
            // cmd = response from db
            var cmd = new MySqlCommand(sql, connection);

            // execute query
            cmd.BeginExecuteNonQuery();
        }

        static void readPerson(MySqlConnection connection)
        {
            // SQL query string
            //string sql = "SELECT *  FROM person";
            string sql = "SELECT *  FROM chat_history";

            // command, request with sql query and sql connection. 
            // cmd = response from db
            var cmd = new MySqlCommand(sql, connection);

            // execute reader
            MySqlDataReader reader = cmd.ExecuteReader();

            //// Show data
            //while(reader.Read())
            //{
            //    Console.WriteLine("Person " + reader["Id"] + ": " 
            //                                + reader["Firstname"] + " " 
            //                                + reader["Lastname"] + ", "
            //                                + reader["Age"] + " y");
            //}

            // Show data
            while (reader.Read())
            {
                Console.WriteLine("Person " + reader["Id"] + ": "
                                            + reader["date"]);
            }

            // close the reader
            reader.Close();
           
        }
    }
}

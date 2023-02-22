using MySql.Data.MySqlClient;
using System;

namespace Congratulator
{
    internal class PersonDate
    {
        private int Id
        { get;}
        public string Surname
        { get; set; }

        public DateTime Date
        { get; set; }
        public void Print()
        {
            Console.WriteLine("{0,40}", Surname + " - " + Date.ToString("d"));
        }
        public void WriteToDB(MySqlConnection conn)
        {
            string query = "INSERT INTO person (surname, dob) values (@Surname, @Date)";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@Surname", Surname);
            command.Parameters.AddWithValue("@Date", Date);
            command.ExecuteNonQuery();
        }
        public void DeleteFromBD(MySqlConnection conn)
        {
            string query = "DELETE FROM person WHERE surname = @Surname AND dob = @Date ";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@Surname", Surname);
            command.Parameters.AddWithValue("@Date", Date);
            command.ExecuteNonQuery();
        }
        public void ChangeInBD(MySqlConnection conn)
        {
            string query = "UPDATE person SET surname = @Surname, dob = @Date WHERE id = @Id ";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@Id", Id);
            command.Parameters.AddWithValue("@Surname", Surname);
            command.Parameters.AddWithValue("@Date", Date);
            command.ExecuteNonQuery();
        }
    }
}

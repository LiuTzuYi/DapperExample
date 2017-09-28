using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace DapperExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\App_Data\UserInfo.mdf;Integrated Security=True;";
            // 查詢
            //foreach (var user in GetAllUser2(connectionString)) {
            //    Console.WriteLine("UserID:{0}, UserEmail:{1}",user.UserId,user.Email);
            //}

            
            //var userIdNamePair = GetAllUser3NamePair(connectionString);
            //foreach (var key in userIdNamePair.Keys) {
            //    Console.WriteLine("Key[UserId]:{0},Value[UserName]:{1}",key,userIdNamePair[key]);
            //}


            // 寫入...
            List<User> insertUsers = new List<User>();
            insertUsers.Add(new User() { UserId="U007", Email="ggg@email.com",Name="user007", Password="12345", RegisterOn=new DateTime(2017,5,5), IsEnable=false });
            insertUsers.Add(new User() { UserId = "U008", Email = "hhh@email.com", Name = "user008", Password = "12345", RegisterOn = new DateTime(2017, 10, 5), IsEnable = false });
            insertUsers.Add(new User() { UserId = "U009", Email = "iii@email.com", Name = "user009", Password = "12345", RegisterOn = new DateTime(2017, 1, 1), IsEnable = true });
            int result = Insert(connectionString, insertUsers);
            Console.WriteLine(result);
            Console.ReadKey();
        }

        private static int Insert(string connectionString, List<User> users) {
            int effectCounter = 0;
            string insertCommand = @"insert into Users values (@UserId,@Email,@Password,@Name,@RegisterOn,@IsEnable)";
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                try
                {
                    conn.Open();
                    effectCounter = conn.Execute(insertCommand, users.ToArray());
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return effectCounter;
        }
        private static Dictionary<string, string> GetAllUser3NamePair(string connectionString) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                try
                {
                    conn.Open();
                    var dynamicUsers = conn.Query("select * from Users where IsEnable=@IsEnable", new { IsEnable = 1 });
                    foreach (var user in dynamicUsers)
                    {
                        dictionary[user.UserId] = user.Name;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return dictionary;
        }

        public static List<User> GetAllUser2(string connectionString) {
            List<User> users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                try
                {
                    conn.Open();
                    int condition1 = 0;
                    string condition2 = "12345";
                    users = conn.Query<User>("select * from Users where IsEnable=@IsEnable and [Password]=@Password", new { IsEnable = condition1,Password=condition2 }).ToList();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return users;
        }
        public static List<User> GetAlUsers(string connectionString) {
            List<User> users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select * from Users";

                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        User user = CreateUser(dr);
                        if (user != null)
                            users.Add(user);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return users;
        }
        public static User CreateUser(SqlDataReader reader) {
            try
            {
                return new User()
                {
                    Email = Convert.ToString(reader["Email"]).Trim(),
                    IsEnable = Convert.ToBoolean(reader["IsEnable"]),
                    Name = Convert.ToString(reader["Name"]).Trim(),
                    Password = Convert.ToString(reader["Password"]).Trim(),
                    RegisterOn = Convert.ToDateTime(reader["RegisterOn"]),
                    UserId = Convert.ToString(reader["UserId"]).Trim()
                };
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}

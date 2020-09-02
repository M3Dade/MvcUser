using MvcUser.Data;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MvcUser.Models
{
    public class UserQuery
    {
        public AppDb Db { get; }

        public UserQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<User> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Username`, `Email`, `Password` FROM `User` WHERE `ID` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }
        public async Task<User> FindOneAsync(string email)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Username`, `Email`, `Password` FROM `User` WHERE `Email` = @email";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@email",
                DbType = DbType.String,
                Value = email,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<User> FindOneAsync(string email, string password)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Username`, `Email`, `Password` FROM `User` WHERE `Email` = @email AND `Password`=@password";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@email",
                DbType = DbType.String,
                Value = email,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@password",
                DbType = DbType.String,
                Value = password,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<User>> LatestPostsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Username`, `Email`, 'Password' FROM `User` ORDER BY `Id` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `User`";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }

        public async Task<List<User>> GetAllUser()
        {
            List<User> list = new List<User>();
            //连接数据库
            //using (MySqlConnection Connection = GetConnection())
            //{
            Db.Connection.Open();
            //查找数据库里面的表
            MySqlCommand cmd = new MySqlCommand("select * from user", Db.Connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                //读取数据
                while (reader.Read())
                {
                    list.Add(new User()
                    {
                        ID = reader.GetInt32("ID"),                     //Id = Convert.ToInt32(reader["Id"])
                        Username = reader.GetString("Username"),        //reader["Name"].ToString()
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        IsValid = !Convert.IsDBNull(reader["IsValid"])
                    }); ;
                }
                //  }
            }
            return list;
        }


        private async Task<List<User>> ReadAllAsync(DbDataReader reader)    //MySqlDataReader查询数据库
        {
            var posts = new List<User>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new User(Db)
                    {
                        ID = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        //IsValid = !Convert.IsDBNull(reader["IsValid"])
                        /*
                        ID = reader.GetInt32("ID"),                     //Id = Convert.ToInt32(reader["Id"])
                        Username = reader.GetString("Username"),        //reader["Name"].ToString()
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        IsValid = !Convert.
                        */
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

    }
}

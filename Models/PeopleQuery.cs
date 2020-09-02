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
    public class PeopleQuery
    {
        public AppDb Db { get; }

        public PeopleQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<People> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Name`, `Level`, `Phone` FROM `People` WHERE `ID` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }
        public async Task<People> FindOneAsync(string Name)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Name`, `Level`, `Phone` FROM `People` WHERE `Name` = @Name";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Name,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<People> FindOneAsync(string email, string password)
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

        public async Task<List<People>> LatestPostsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Username`, `Email`, 'Password' FROM `User` ORDER BY `Id` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `People`";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }


        public async Task<List<People>> ReadAllAsync(DbDataReader reader)    //MySqlDataReader查询数据库
        {
            var posts = new List<People>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new People(Db)
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Level = reader.GetString(2),
                        Phone = reader.GetString(3),
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

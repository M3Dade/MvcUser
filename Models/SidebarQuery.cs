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
    public class SidebarQuery
    {
        public AppDb Db { get; }

        public SidebarQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Sidebar> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Name`, `Link`, `Password` FROM `Siderbar` WHERE `ID` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }
        public async Task<Sidebar> FindOneAsync(string email)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Name`, `Link`, `Parent` FROM `Siderbar` WHERE `Link` = @Link";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@email",
                DbType = DbType.String,
                Value = email,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<Sidebar> FindOneAsync(string email, string password)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `ID`, `Name`, `Link`, `Parent` FROM `Siderbar` WHERE `Link` = @email AND `Password`=@password";
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

        public async Task<List<Sidebar>> LatestPostsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `Name`, `Link`, 'Parent' FROM `Siderbar` ORDER BY `Id` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `Sidebar`";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }


        private async Task<List<Sidebar>> ReadAllAsync(DbDataReader reader)    //MySqlDataReader查询数据库
        {
            var posts = new List<Sidebar>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Sidebar(Db)
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Link = reader.GetString(2),
                        Parent = reader.GetString(3),
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

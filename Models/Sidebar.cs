using MvcUser.Data;
using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace MvcUser.Models
{
    [Table("Sidebar")]
    public class Sidebar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Parent { get; set; }
        public string Value { get; set; }

        internal AppDb Db { get; set; }

        public Sidebar()
        {
        }

        internal Sidebar(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `Sidebar` (`Name`, `Link`, `Parent`) VALUES (@Name, @Link, @Parent);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();   //插入、更新和删除数据
            Id = (int)cmd.LastInsertedId;
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `Sidebar` SET `Name` = @Name, `Link` = @Link, `Parent` = @Parent  WHERE `Id` = @Id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `Sidebar` WHERE `Id` = @Id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Id",
                DbType = DbType.Int32,
                Value = Id,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Name,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Link",
                DbType = DbType.String,
                Value = Link,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Parent",
                DbType = DbType.String,
                Value = Parent,
            });

        }
    }
}

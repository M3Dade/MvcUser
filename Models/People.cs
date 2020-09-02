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
    [Table("People")]
    public class People
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Phone { get; set; }

        internal AppDb Db { get; set; }

        //internal MvcUserContext Db { get; set; }

        public People()
        {
        }

        internal People(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `People` (`Name`, `Level`, `Phone`) VALUES (@Name, @Level, @Phone);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();   //插入、更新和删除数据
            ID = (int)cmd.LastInsertedId;
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `People` SET `Name` = @Name, `Level` = @Level, `Phone` = @Phone  WHERE `ID` = @ID;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `People` WHERE `ID` = @ID;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@ID",
                DbType = DbType.Int32,
                Value = ID,
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
                ParameterName = "@Level",
                DbType = DbType.String,
                Value = Level,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Phone",
                DbType = DbType.String,
                Value = Phone,
            });

        }
    }
}

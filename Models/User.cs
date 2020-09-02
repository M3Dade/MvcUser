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
    [Table("User")]
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsValid { get; set; }

        internal AppDb Db { get; set; }
        /*
         * public int ID { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        public bool IsValid { get; set; }
         */

        //internal MvcUserContext Db { get; set; }

        public User()
        {
        }

        internal User(AppDb db)
        {
            Db = db;
        }


        public void Insert()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `User` (`Username`, `Email`, `Password`) VALUES @Username, @Email, @Password);";
            BindParams(cmd);
            cmd.ExecuteNonQuery();
            ID = (int)cmd.LastInsertedId;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `User` (`Username`, `Email`, `Password`) VALUES (@Username, @Email, @Password);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();   //插入、更新和删除数据
            ID = (int)cmd.LastInsertedId;
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `USer` SET `Email` = @Email, `Password` = @Password WHERE `ID` = @ID;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `User` WHERE `ID` = @ID;";
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
                ParameterName = "@Username",
                DbType = DbType.String,
                Value = Username,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Email",
                DbType = DbType.String,
                Value = Email,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Password",
                DbType = DbType.String,
                Value = Password,
            });

        }
    }
}

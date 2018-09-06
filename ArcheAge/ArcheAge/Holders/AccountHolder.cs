﻿using LocalCommons.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArcheAge.Properties;
using ArcheAgeLogin.ArcheAge.Structuring;
using ArcheAgeLogin.Properties;

namespace ArcheAge.ArcheAge.Holders
{
    public class AccountHolder
    {
        private static List<Account> m_DbAccounts;

        /// <summary>
        /// Loaded List of Accounts.
        /// </summary>
        public static List<Account> AccountList
        {
            get { return m_DbAccounts; }
        }

        /// <summary>
        /// Gets Account By Name With LINQ Or Return Null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Account GetAccount(string name)
        {
            return m_DbAccounts.FirstOrDefault(acc => acc.Name == name);
        }

        /// <summary>
        /// Fully Load Account Data From Current MySql DataBase.
        /// </summary>
        public static void LoadAccountData()
        {
            m_DbAccounts = new List<Account>();
            MySqlConnection con = new MySqlConnection(Settings.Default.DataBaseConnectionString);
            try
            {
                con.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM `accounts`", con);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Account account = new Account();
                    account.AccessLevel = reader.GetByte("mainaccess");
                    account.AccountId = reader.GetInt64("id");
                    account.Name = reader.GetString("name");
                    account.Password = reader.GetString("password");
                    account.Token = reader.GetString("token");
                    account.LastEnteredTime = reader.GetInt64("last_online");
                    account.LastIp = reader.GetString("last_ip");
                    account.Membership = reader.GetByte("useraccess");
                    account.Characters = reader.GetByte("characters");
                    account.Session = reader.GetInt32("cookie");
                    m_DbAccounts.Add(account);
                }
                command = null;
                reader = null;
            }
            catch(Exception e)
            {
                if(e.Message.IndexOf("using password: YES") >= 0)
                {
                    Logger.Trace("Error: Incorrect username or password");
                }
                else if (e.Message.IndexOf("Unable to connect to any of the specified MySQL hosts")>=0)
                {
                    Logger.Trace("Error: Unable to connect to database");
                }
                else
                {
                    Logger.Trace("Error: Unknown error");
                }
                //Console.ReadKey();
                //Message = "Authentication to host '127.0.0.1' for user 'root' using method 'mysql_native_password' failed with message: Access denied for user 'root'@'localhost' (using password: YES)"
            }
            finally
            {
                con.Close();
                con = null;
            }
            Logger.Trace("Load to {0} accounts", m_DbAccounts.Count);
        }
       
        /// <summary>
        /// Inserts Or Update Existing Account Into your current Login Server MySql DataBase.
        /// </summary>
        /// <param name="account">Your Account Which you want Insert(If Not Exist) Or Update(If Exist)</param>
        public static void InsertOrUpdate(Account account)
        {
            MySqlConnection con = new MySqlConnection(Settings.Default.DataBaseConnectionString);
            try
            {
                con.Open();
                MySqlCommand command = null;
                if (m_DbAccounts.Contains(account))
                {
                    command = new MySqlCommand(
                        "UPDATE `accounts` SET `id` = @id, `name` = @name, `password` = @password, `token` = @token, `mainaccess` = @mainaccess," +
                        " `useraccess` = @useraccess, `last_ip` = @lastip, `last_online` = @lastonline, `cookie` = @cookie, `characters` = @characters" +
                        " WHERE `id` = @aid",
                        con);
                }
                else
                {
                    command = new MySqlCommand(
                        "INSERT INTO `accounts`(id, name, password, token,  mainaccess, useraccess, last_ip, last_online, characters, cookie)" +
                        "VALUES(@id, @name, @password, @token, @mainaccess, @useraccess, @lastip, @lastonline, @characters, @cookie)",
                        con);
                }
                MySqlParameterCollection parameters = command.Parameters;
                parameters.Add("@id", MySqlDbType.Int64).Value = account.AccountId;
                parameters.Add("@name", MySqlDbType.String).Value = account.Name;
                parameters.Add("@password", MySqlDbType.String).Value = account.Password;
                parameters.Add("@token", MySqlDbType.String).Value = account.Token;
                parameters.Add("@mainaccess", MySqlDbType.Byte).Value = account.AccessLevel;
                parameters.Add("@useraccess", MySqlDbType.Byte).Value = account.Membership;
                parameters.Add("@lastip", MySqlDbType.String).Value = account.LastIp;
                parameters.Add("@lastonline", MySqlDbType.Int64).Value = account.LastEnteredTime;
                parameters.Add("@characters", MySqlDbType.Byte).Value = account.Characters;
                parameters.Add("@cookie", MySqlDbType.Int32).Value = account.Session;

                if (m_DbAccounts.Contains(account))
                    parameters.Add("@aid", MySqlDbType.Int64).Value = account.AccountId;

                command.ExecuteNonQuery();
                command = null;
            }
            catch (Exception e)
            {
                Logger.Trace("Cannot InsertOrUpdate template for " + account.Name + ": {0}", e);
            }
            finally
            {
                m_DbAccounts.Add(account);
                con.Close();
                con = null;
            }
        }
    }
}

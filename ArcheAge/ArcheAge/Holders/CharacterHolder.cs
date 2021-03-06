﻿using ArcheAge.ArcheAge.Structuring;
using ArcheAge.Properties;
using LocalCommons.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArcheAge.ArcheAge.Holders
{
    class CharacterHolder
    {
        /// <summary>
        /// Loaded List of Characters.
        /// </summary>
        public static List<Character> CharactersList { get; private set; }

        /// <summary>
        /// Gets Character By CharName With LINQ Or Return Null.
        /// </summary>
        /// <param name="charName"></param>
        /// <returns></returns>
        public static Character GetCharacter(string charName)
        {
            return CharactersList.FirstOrDefault(acc => acc.CharName == charName);
        }

        /// <summary>
        /// Fully Load Characters Data From Current MySql DataBase.
        /// </summary>
        public static List<Character> LoadCharacterData()
        {
            CharactersList = new List<Character>();
            int accountId = 50970;
            int serverid = Settings.Default.Game_Id;
            string connection = "server=" + Settings.Default.DataBase_Host + ";user=" + Settings.Default.DataBase_User + ";database=" + Settings.Default.DataBase_Name + ";port=" + Settings.Default.DataBase_Port + ";password=" + Settings.Default.DataBase_Password + ((!Settings.Default.SSL) ? "; SslMode = none" : "");

            //using (MySqlConnection conn = new MySqlConnection(Settings.Default.DataBaseConnectionString))
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command =
                      new MySqlCommand(
                        "SELECT * FROM characters WHERE AccountID = '" + accountId + "' AND WorldID = '" +
                          serverid + "'", conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Character character = new Character();

                        character.Id = reader.GetInt64("id");
                        character.AccountId = reader.GetInt32("accountid");
                        character.CharGender = reader.GetByte("chargender");
                        character.CharName = reader.GetString("charname");
                        character.CharRace = reader.GetByte("charrace");
                        character.Decor = reader.GetInt32("decor");
                        character.Eyebrow = reader.GetInt32("eyebrow");
                        character.Guid = reader.GetString("guid");
                        character.LeftPupil = reader.GetInt32("leftpupil");
                        character.Level = reader.GetByte("level");
                        character.Lip = reader.GetInt32("lip");
                        character.Modifiers = reader.GetString("modifiers");
                        character.MoveX = reader.GetFloat("movex");
                        character.MoveY = reader.GetFloat("movey");
                        character.RightPupil = reader.GetInt32("rightpupil");
                        character.Rotate = reader.GetFloat("rotate");
                        character.Scale = reader.GetFloat("scale");
                        character.Type[0] = reader.GetInt32("type0");
                        character.Type[1] = reader.GetInt32("type1");
                        character.Type[2] = reader.GetInt32("type2");
                        character.Type[3] = reader.GetInt32("type3");
                        character.Type[4] = reader.GetInt32("type4");
                        character.Type[5] = reader.GetInt32("type5");
                        character.Type[6] = reader.GetInt32("type6");
                        character.Type[7] = reader.GetInt32("type7");
                        character.Type[8] = reader.GetInt32("type8");
                        character.Type[9] = reader.GetInt32("type9");
                        character.Type[10] = reader.GetInt32("type10");
                        character.Type[11] = reader.GetInt32("type11");
                        character.Type[12] = reader.GetInt32("type12");
                        character.Type[13] = reader.GetInt32("type13");
                        character.Type[14] = reader.GetInt32("type14");
                        character.Type[15] = reader.GetInt32("type15");
                        character.Type[16] = reader.GetInt32("type16");
                        character.Type[17] = reader.GetInt32("type17");
                        character.V = reader.GetInt64("v");
                        character.Weight[0] = reader.GetFloat("Weight0");
                        character.Weight[1] = reader.GetFloat("Weight1");
                        character.Weight[2] = reader.GetFloat("Weight2");
                        character.Weight[3] = reader.GetFloat("Weight3");
                        character.Weight[4] = reader.GetFloat("Weight4");
                        character.Weight[5] = reader.GetFloat("Weight5");
                        character.Weight[6] = reader.GetFloat("Weight6");
                        character.Weight[7] = reader.GetFloat("Weight7");
                        character.Weight[8] = reader.GetFloat("Weight8");
                        character.Weight[9] = reader.GetFloat("Weight9");
                        character.Weight[10] = reader.GetFloat("Weight10");
                        character.Weight[11] = reader.GetFloat("Weight11");
                        character.Weight[12] = reader.GetFloat("Weight12");
                        character.Weight[13] = reader.GetFloat("Weight13");
                        character.Weight[14] = reader.GetFloat("Weight14");
                        character.Weight[15] = reader.GetFloat("Weight15");
                        character.Weight[16] = reader.GetFloat("Weight16");
                        character.Weight[17] = reader.GetFloat("Weight17");
                        character.WorldId = reader.GetByte("worldid");
                        character.A[0] = reader.GetByte("a0");
                        character.A[1] = reader.GetByte("a1");
                        character.A[2] = reader.GetByte("a2");
                        CharactersList.Add(character);
                    }

                    command = null;
                    reader = null;
                }
                catch (Exception e)
                {
                    if (e.Message.IndexOf("using password: YES") >= 0)
                    {
                        Logger.Trace("Error: Incorrect username or password");
                    }
                    else if (e.Message.IndexOf("Unable to connect to any of the specified MySQL hosts") >= 0)
                    {
                        Logger.Trace("Error: Unable to connect to database");
                    }
                    else
                    {
                        Logger.Trace("Error: Unknown error +  {0}", e);
                    }
                }
                finally
                {
                }
            }

            Logger.Trace("Load to {0} characters", CharactersList.Count);
            return CharactersList;
        }

        /// <summary>
        /// Inserts Or Update Existing Character Into your current Login Server MySql DataBase.
        /// </summary>
        /// <param name="character">Your Character Which you want Insert(If Not Exist) Or Update(If Exist)</param>
        public static void InsertOrUpdate(Character character)
        {
            using (MySqlConnection conn = new MySqlConnection(Settings.Default.DataBaseConnectionString))
            {

                try
                {
                    conn.Open();  //Устанавливаем соединение с базой данных
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    if (CharactersList.Contains(character))
                    {
                        cmd.CommandText = "UPDATE `characters` SET `id` = @id, `accountid` = @accountid, `chargender` = @chargender, `charname` = @charname," +
                                          " `charrace` = @charrace, `decor` = @decor, `eyebrow` = @eyebrow, `guid` = @guid, `leftPupil` = @leftPupil, `level` = @level," +
                                          " `lip` = @lip, `modifiers` = @modifiers, `movex` = @movex, `movey` = @movey, `rightpupil` = @rightpupil, `rotate` = @rotate," +
                                          " `scale` = @scale, `type0` = @type0, `type1` = @type1, `type2` = @type2, `type3` = @type3, `type4` = @type4, `type5` = @type5," +
                                          " `type6` = @type6, `type7` = @type7, `type8` = @type8, `type9` = @type9, `type10` = @type10, `type11` = @type11, `type12` = @type12," +
                                          " `type13` = @type13, `type14` = @type14, `type15` = @type15, `type16` = @type16, `type17` = @type17, `v` = @v, `Weight0` = @Weight0," +
                                          " `Weight1` = @Weight1, `Weight2` = @Weight2, `Weight3` = @Weight3, `Weight4` = @Weight4, `Weight5` = @Weight5, `Weight6` = @Weight6," +
                                          " `Weight7` = @Weight7, `Weight8` = @Weight8, `Weight9` = @Weight9, `Weight10` = @Weight10, `Weight11` = @Weight11, `Weight12` = @Weight12," +
                                          " `Weight13` = @Weight13, `Weight14` = @Weight14, `Weight15` = @Weight15, `Weight16` = @Weight16, `Weight17` = @Weight17," +
                                          " `worldid` = @worldid, `a0` = @a0, `a1` = @a1, `a2` = @a2 WHERE `charname` = @charname";

                        var count = CharactersList.Count; //
                        cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = count + 1; //incr index key
                    }
                    else
                    {

                        cmd.CommandText = @"INSERT INTO `characters` (id, accountid, chargender, charname, charrace, decor, eyebrow, guid, leftPupil, level, lip, modifiers," +
                            " movex, movey, rightpupil, rotate, scale, type0, type1, type2, type3, type4, type5, type6, type7, type8, type9, type10, type11, type12, type13," +
                            " type14, type15, type16, type17, v, Weight0, Weight1, Weight2, Weight3, Weight4, Weight5, Weight6, Weight7, Weight8, Weight9, Weight10," +
                            " Weight11, Weight12, Weight13, Weight14, Weight15, Weight16, Weight17, Worldid, a0, a1, a2) " +

                            " VALUES (@id, @accountid, @chargender, @charname, @charrace, @decor, @eyebrow, @guid, @leftPupil, @level, @lip, @modifiers, @movex, @movey," +
                            " @rightpupil, @rotate, @scale, @type0, @type1, @type2, @type3, @type4, @type5, @type6, @type7, @type8, @type9, @type10, @type11, @type12," +
                            " @type13, @type14, @type15, @type16, @type17, @v, @Weight0, @Weight1, @Weight2, @Weight3, @Weight4, @Weight5, @Weight6, @Weight7, @Weight8," +
                            " @Weight9, @Weight10, @Weight11, @Weight12, @Weight13, @Weight14, @Weight15, @Weight16, @Weight17, @Worldid, @a0, @a1, @a2)";

                        cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = character.Id;
                        //}

                        //MySqlParameterCollection parameters = cmd.Parameters;

                        cmd.Parameters.Add("@accountid", MySqlDbType.Int32).Value = character.AccountId;
                        cmd.Parameters.Add("@chargender", MySqlDbType.Byte).Value = character.CharGender;
                        cmd.Parameters.Add("@charname", MySqlDbType.String).Value = character.CharName;
                        cmd.Parameters.Add("@charrace", MySqlDbType.Byte).Value = character.CharRace;
                        cmd.Parameters.Add("@decor", MySqlDbType.Int32).Value = character.Decor;
                        cmd.Parameters.Add("@eyebrow", MySqlDbType.Int32).Value = character.Eyebrow;
                        cmd.Parameters.Add("@guid", MySqlDbType.String).Value = character.Guid;
                        cmd.Parameters.Add("@leftPupil", MySqlDbType.Int32).Value = character.LeftPupil;
                        cmd.Parameters.Add("@level", MySqlDbType.Byte).Value = character.Level;
                        cmd.Parameters.Add("@lip", MySqlDbType.Int32).Value = character.Lip;
                        cmd.Parameters.Add("@modifiers", MySqlDbType.String).Value = character.Modifiers;
                        cmd.Parameters.Add("@movex", MySqlDbType.Float).Value = character.MoveX;
                        cmd.Parameters.Add("@movey", MySqlDbType.Float).Value = character.MoveY;
                        cmd.Parameters.Add("@rightpupil", MySqlDbType.Int32).Value = character.RightPupil;
                        cmd.Parameters.Add("@rotate", MySqlDbType.Float).Value = character.Rotate;
                        cmd.Parameters.Add("@scale", MySqlDbType.Float).Value = character.Scale;
                        cmd.Parameters.Add("@type0", MySqlDbType.Int32).Value = character.Type[0];
                        cmd.Parameters.Add("@type1", MySqlDbType.Int32).Value = character.Type[1];
                        cmd.Parameters.Add("@type2", MySqlDbType.Int32).Value = character.Type[2];
                        cmd.Parameters.Add("@type3", MySqlDbType.Int32).Value = character.Type[3];
                        cmd.Parameters.Add("@type4", MySqlDbType.Int32).Value = character.Type[4];
                        cmd.Parameters.Add("@type5", MySqlDbType.Int32).Value = character.Type[5];
                        cmd.Parameters.Add("@type6", MySqlDbType.Int32).Value = character.Type[6];
                        cmd.Parameters.Add("@type7", MySqlDbType.Int32).Value = character.Type[7];
                        cmd.Parameters.Add("@type8", MySqlDbType.Int32).Value = character.Type[8];
                        cmd.Parameters.Add("@type9", MySqlDbType.Int32).Value = character.Type[9];
                        cmd.Parameters.Add("@type10", MySqlDbType.Int32).Value = character.Type[10];
                        cmd.Parameters.Add("@type11", MySqlDbType.Int32).Value = character.Type[11];
                        cmd.Parameters.Add("@type12", MySqlDbType.Int32).Value = character.Type[12];
                        cmd.Parameters.Add("@type13", MySqlDbType.Int32).Value = character.Type[13];
                        cmd.Parameters.Add("@type14", MySqlDbType.Int32).Value = character.Type[14];
                        cmd.Parameters.Add("@type15", MySqlDbType.Int32).Value = character.Type[15];
                        cmd.Parameters.Add("@type16", MySqlDbType.Int32).Value = character.Type[16];
                        cmd.Parameters.Add("@type17", MySqlDbType.Int32).Value = character.Type[17];
                        cmd.Parameters.Add("@v", MySqlDbType.Int64).Value = character.V;
                        cmd.Parameters.Add("@Weight0", MySqlDbType.Int32).Value = character.Weight[0];
                        cmd.Parameters.Add("@Weight1", MySqlDbType.Int32).Value = character.Weight[1];
                        cmd.Parameters.Add("@Weight2", MySqlDbType.Int32).Value = character.Weight[2];
                        cmd.Parameters.Add("@Weight3", MySqlDbType.Int32).Value = character.Weight[3];
                        cmd.Parameters.Add("@Weight4", MySqlDbType.Int32).Value = character.Weight[4];
                        cmd.Parameters.Add("@Weight5", MySqlDbType.Int32).Value = character.Weight[5];
                        cmd.Parameters.Add("@Weight6", MySqlDbType.Int32).Value = character.Weight[6];
                        cmd.Parameters.Add("@Weight7", MySqlDbType.Int32).Value = character.Weight[7];
                        cmd.Parameters.Add("@Weight8", MySqlDbType.Int32).Value = character.Weight[8];
                        cmd.Parameters.Add("@Weight9", MySqlDbType.Int32).Value = character.Weight[9];
                        cmd.Parameters.Add("@Weight10", MySqlDbType.Int32).Value = character.Weight[10];
                        cmd.Parameters.Add("@Weight11", MySqlDbType.Int32).Value = character.Weight[11];
                        cmd.Parameters.Add("@Weight12", MySqlDbType.Int32).Value = character.Weight[12];
                        cmd.Parameters.Add("@Weight13", MySqlDbType.Int32).Value = character.Weight[13];
                        cmd.Parameters.Add("@Weight14", MySqlDbType.Int32).Value = character.Weight[14];
                        cmd.Parameters.Add("@Weight15", MySqlDbType.Int32).Value = character.Weight[15];
                        cmd.Parameters.Add("@Weight16", MySqlDbType.Int32).Value = character.Weight[16];
                        cmd.Parameters.Add("@Weight17", MySqlDbType.Int32).Value = character.Weight[17];
                        cmd.Parameters.Add("@worldid", MySqlDbType.Byte).Value = character.WorldId;
                        cmd.Parameters.Add("@a0", MySqlDbType.Byte).Value = character.A[0];
                        cmd.Parameters.Add("@a1", MySqlDbType.Byte).Value = character.A[1];
                        cmd.Parameters.Add("@a2", MySqlDbType.Byte).Value = character.A[2];

                        CharactersList.Add(character);

                        if (CharactersList.Contains(character))
                        {
                            cmd.Parameters.Add("@acharname", MySqlDbType.String).Value = character.CharName;
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace("Cannot InsertOrUpdate template for " + character.CharName + ": {0}", ex); // ex.Message - написать так, если нужно только сообщение без указания строки в сурсах
                }
                finally
                {
                    //CharactersList.Add(character);
                }
            }
        }
    }
}
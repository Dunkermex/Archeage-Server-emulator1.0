using ArcheAge.ArcheAge.Holders;
using ArcheAge.ArcheAge.Network.Connections;
using LocalCommons.Network;
using System.Collections.Generic;
using ArcheAgeLogin.ArcheAge.Structuring;
using Character = ArcheAge.ArcheAge.Structuring.Character;

namespace ArcheAge.ArcheAge.Network
{
    public sealed class NP_Packet_CharList01_0x0039 : NetPacket
    {
        /// <summary>
        /// пакет для входа в Лобби
        /// author: NLObP
        /// </summary>
        public NP_Packet_CharList01_0x0039(ClientConnection net) : base(01, 0x0039)
        {
            //1.0.1406
            //SC CharacterListPacket
            //            opcode last count charID   len  name                   lvl gndr health mana
            //"8104 DD01  3900   01   01    FF091A00 0B00 4A757374746F636865636B 01  02   03C4   0100 00CE010000B3000000650000000000000000000000000000000000000000005B5B00004618C1000000000000000100000001000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005C5B00004718C1000000000000000100000001000000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005E5B00004818C1000000000000000100000001000000002300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D61700004918C1000000000000000100000001000000009100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B0000000000000000000000000000000000000000EF1700004A18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000003A1800004B18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000007F4D0000D85D00000000000000000000000000001B020000000000000000000000000000070B0B00000000A8B7CF03000000006090A603EFFC104303BE1000000400000000000000000000000000803F0000803F0000000000000000000000000000803FCF0100000000803FA60000000000803F000000008FC2353F0000000000000000000000000000803FE37B8BFFAFECEFFFAFECEFFF000000FF00000000800000EF00EF00EE0017D40000000000001000000000000000063BB900D800EE00D400281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000036007F422F530000000000000C302B5300000000000000000C302B530000000000000000B4412F5300000000C200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001E00000000000000000000000000000000000000000000000000000000000000000000000000000000B33F2F5300000000

            //last 1
            ns.Write((byte)0x01);
            string accid = net.CurrentAccount.AccountId.ToString();

            //Console.WriteLine("-=-=-=-=-=-=-=-=-=--=-=-=-=-" + accid + "-=-=-=-=-=-=-=-=-=--=-=-=-=-");

            List<CharacterListHolder.Characters> charlist = CharacterListHolder.LoadChars(accid);
            int totalchars = charlist.Count;

            //remains hardcoded for now will be done later its defining the first character to be displayed on the list
            //count 1
            ns.Write((byte)totalchars);

            //пробегаем по всем героям
            foreach (CharacterListHolder.Characters chr in charlist)
            {
                //type 4 (charID)
                //D7940100
                //Weirdly this isnt the character id in the database but the type in the database a mistake?
                //For now loads the ID i think its more apropriate as it will always be unique something thats needed here
                ns.Write((int)chr.id);
                //size.name
                //0600 52656D6F7461 (Remota)
                string msg = chr.name;
                ns.WriteUTF8Fixed(msg, msg.Length);
                //CharRace 1, Role Race Follow the fields of the login server 1 Noah, 3 Dwarfs, 4 Elf, 5 Harry Blue, 6 Beast, 8 Battle Demon.
                //03 (Гномы)
                ns.Write((byte)chr.race);
                //CharGender 1, Character gender 1 male, 2 female
                //02 (Ж)
                ns.Write((byte)chr.gender);

                //Everything below here remains hardcoded since the data is missing from database

                //level 1
                //01
                ns.Write((byte)0x01);
                //health 4
                //D0020000 (720)
                ns.Write((int)0x02D0);
                //mana 4
                //9E020000 (670)
                ns.Write((int)0x029E);
                //zone_id 4
                //2C010000
                ns.Write((int)0x012C);
                //type 4 FactionId
                //68000000
                ns.Write((int)0x68);
                //ns.Write((int)0x00); //если 00, то можно в factionName свою фракцию писать
                //size.factionName
                //0000
                //msg = "Моя superb фракция";
                msg = "";
                ns.WriteUTF8Fixed(msg, msg.Length);
                //type 4
                //00000000
                ns.Write((int)0x00);
                //family 4
                //00000000
                ns.Write((int)0x00);
                //{
                // validFlags 4
                ns.Write((int)0x011F8054);
                // { 7, раз, предметы на герое?
                // {1}
                //     type 4 ItemID Head
                ns.Write((int)0x5052);
                //     id 8
                ns.Write((long)0x0501FAA5);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x55);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {2}
                //     type 4 ItemID Chest
                ns.Write((int)0x506D);
                //     id 8
                ns.Write((long)0x0501FAA6);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x46);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {3}
                //     type 4 ItemID Legs
                ns.Write((int)0x5088);
                //     id 8
                ns.Write((long)0x0501FAA7);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x23);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {4}
                //     type 4 ItemID Gloves
                ns.Write((int)0x50A3);
                //     id 8
                ns.Write((long)0x0501FAA8);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x82);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {5}
                //     type 4 itemId Feet
                ns.Write((int)0x50BE);
                //     id 8 ObjectId
                ns.Write((long)0x0501FAA9);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x9B);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {6}
                //     type 4 ItemId
                ns.Write((int)0x17EF);
                //     id 8
                ns.Write((long)0x0501FAAA);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x82);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // {7}
                //     type 4 itemId
                ns.Write((int)0x1821);
                //     id 8 ObjectId
                ns.Write((long)0x0501FAAB);
                //     type 1
                ns.Write((byte)0x00);
                //     flags 1
                ns.Write((byte)0x00);
                //     stackSize 4
                ns.Write((int)0x01);
                //     {
                //         detailType 1 (switch)
                ns.Write((byte)0x01);
                //         {
                //            durability 1
                ns.Write((byte)0x82);
                //            chargeCount 2
                ns.Write((short)0x00);
                //            chargeTime 8
                ns.Write((long)0x00);
                //            scaledA 2
                ns.Write((short)0x00);
                //            scaledB 2
                ns.Write((short)0x00);
                //            { 4, раза
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //                pish 1
                ns.Write((byte)0x00);
                //                pisc 4
                ns.Write((int)0x00);
                //            }
                //         }
                //     }
                //     creationTime 8
                ns.Write((long)0x5AA53D49);
                //     lifespanMins 4
                ns.Write((int)0x00);
                //     type 4
                ns.Write((int)0x00);
                //     worldId 1
                ns.Write((byte)0x01);
                //     unsecureDateTime 8
                ns.Write((long)0x00);
                //     unpackDateTime 8
                ns.Write((long)0x00);
                //     chargeUseSkillTime 8
                ns.Write((long)0x00);
                // }
                // { 3 раза
                // type 4
                ns.Write((int)0x0193);
                // type 4
                ns.Write((int)0x9D88);
                // type 4
                ns.Write((int)0x0221);
                // flags 4
                ns.Write((int)0x00);
                // }
                //}
                //{3 раза
                //ability 1
                ns.Write((byte)0x01);
                ns.Write((byte)0x0D);
                ns.Write((byte)0x0D);
                //}
                //pos в пакете нет, в коде 01
                //x 8
                ns.Write((long)0x0006FE959E500000);
                //y 8
                ns.Write((long)0x0021FC30B3480000);
                //z 4
                ns.Write((int)0x42CFA1CB);
                //{
                //ext 1
                ns.Write((byte)0x03);
                //type 4
                ns.Write((int)0x00);
                //type 4
                ns.Write((int)0x00);
                //defaultHairColor 4
                ns.Write((int)0x300907FF);
                //twoToneHair 4
                ns.Write((int)0xFF);
                //twoToneFirstWidth 4
                ns.Write((int)0x00);
                //twoToneSecondWidth 4
                ns.Write((int)0x00);
                //type 4
                ns.Write((int)0x5F);
                //type 4
                ns.Write((int)0x00);
                //type 4
                ns.Write((int)0x00);
                //weight 4
                ns.Write((int)0x3F800000);
                //{
                //type 4
                ns.Write((int)0x00);
                //weight 4
                ns.Write((int)0x3F800000);
                //scale 4
                ns.Write((int)0x3F800000);
                //rotate 4
                ns.Write((int)0x00);
                //moveX 2
                ns.Write((short)0x00);
                //moveY 2
                ns.Write((short)0x00);
                //}
                //{
                //pish 1
                ns.Write((byte)0x54);
                //pisc 5
                ns.Write((byte)0x00);
                ns.Write((byte)0x0B);
                ns.Write((byte)0x05);
                ns.Write((byte)0xFC);
                ns.Write((byte)0x04);
                //pish 1
                ns.Write((byte)0x34);
                //pisc 2
                ns.Write((byte)0x05);
                ns.Write((byte)0x05);
                //pish 1
                ns.Write((byte)0x0F);
                //pisc 3
                ns.Write((byte)0x06);
                ns.Write((byte)0x0F);
                ns.Write((byte)0x06);
                //????????????? еще раз ????????
                //pish 1
                ns.Write((byte)0x00);
                //pisc 3
                ns.Write((byte)0x00);
                ns.Write((byte)0x00);
                ns.Write((byte)0x00);
                //}
                //{6 раз
                //weight 4
                ns.Write((int)0x3F800000);
                ns.Write((int)0x3F800000);
                ns.Write((int)0x3F800000);
                ns.Write((int)0x3F35C28F);
                ns.Write((int)0x3F800000);
                ns.Write((int)0x3F800000);
                //}
                //weight 4
                ns.Write((int)0x3F800000);
                //lip 4
                ns.Write((uint)0xAB3E38FF); //не влезает в INTEGER
                //leftPupil 4
                ns.Write((uint)0x5F5B25FF); //не влезает в INTEGER
                //rightPupil 4
                ns.Write((uint)0x5F5B25FF); //не влезает в INTEGER
                //eyebrow 4
                ns.Write((uint)0x9A0E0EFF); //не влезает в INTEGER
                //deco 4
                ns.Write((int)0x00);
                //size.modifiers (128)
                msg =
                    "0000100000F10021DDCC403A0B0BFB0300F6F5CF0C00D10047000BCC9C00000000E800000000D9BF000000000000000CFA1E0012E10C003E21642323EE16000000E80018E8B9E800223700FC00000000CFCC00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                ns.WriteHex(msg, msg.Length);
                //}
                //laborPower 2
                ns.Write((short)0x1388); //очки работы = 5000
                //lastLaborPowerModified 8
                ns.Write((long)0x5B2D8572);
                //deadCount 2
                ns.Write((short)0x00);
                //deadTime 8
                ns.Write((long)0x5B29D9A2);
                //rezWaitDuration 4
                ns.Write((int)0x00);
                //rezTime 8
                ns.Write((long)0x5B29D9A2);
                //rezPenaltyDuration 4
                ns.Write((int)0x00);
                //lastWorldLeaveTime 8
                ns.Write((long)0x5B2D8205);
                //moneyAmount 8, Number of copper coins Automatic 1:100:10000 Convert gold coins
                ns.Write((long)0x1E); //серебро, золото и платина (начало)
                //moneyAmount 8
                ns.Write((long)0x00); //серебро, золото и платина (продолжение)
                //crimePoint 2
                ns.Write((short)0x00);
                //crimeRecord 4
                ns.Write((int)0x00);
                //crimeScore 2
                ns.Write((short)0x00);
                //deleteRequestedTime  8
                ns.Write((long)0x00);
                //transferRequestedTime 8
                ns.Write((long)0x00);
                //deleteDelay 8
                ns.Write((long)0x00);
                //consumedLp 4
                ns.Write((int)0x00);
                //bmPoint 8
                ns.Write((long)0x1E); //монеты дару = 30
                //moneyAmount 8
                ns.Write((long)0x00);
                //moneyAmount 8
                ns.Write((long)0x00);
                //autoUseAApoint 1
                ns.Write((byte)0x00);
                //prevPoint 4
                ns.Write((int)0x01);
                //point 4
                ns.Write((int)0x01);
                //gift 4
                ns.Write((int)0x00);
                //updated 8
                ns.Write((long)0x5B3F9014);
                //forceNameChange 1
                ns.Write((byte)0x00);
                //highAbilityRsc 4
                ns.Write((int)0x00);
                //}
            }
        }
    }

    /*
    type
      TItemSet = record
        Id: Integer;
        Name: string;
        Head, Chest, Legs, Gloves, Feet: Integer;
      end;

      TCharacterModel = record
        race, gender, face, body, id, model_id, hair_id, hair_color_id, skin_color_id, face_movable_decal_asset_id, face_movable_decal_scale, face_movable_decal_rotate, face_movable_decal_move_x, face_movable_decal_move_y, face_fixed_decal_asset_0_id, face_fixed_decal_asset_1_id, face_fixed_decal_asset_2_id, face_fixed_decal_asset_3_id, face_diffuse_map_id, face_normal_map_id, face_eyelash_map_id, lip_color, left_pupil_color, right_pupil_color, eyebrow_color, owner_type_id, face_movable_decal_weight, face_fixed_decal_asset_0_weight, face_fixed_decal_asset_2_weight, face_normal_map_weight, deco_color: Integer;
        name, npcOnly, modifier: string;
        face_fixed_decal_asset_3_weight, face_fixed_decal_asset_1_weight: Double;
      end;

    const
      ItemSets: array [0..69] of TItemSet = (
        (Id: 159; Name: 'Безжалостный кошмар'; Head: 27293; Chest: 27294; Legs: 27298; Gloves: 27296; Feet: 27299),
        (Id: 149; Name: 'Безумие сумасшедшего отшельника'; Head: 24503; Chest: 24504; Legs: 24505; Gloves: 24506; Feet: 24507),
        (Id: 166; Name: 'Воля забытого экипажа'; Head: 27996; Chest: 27995; Legs: 27998; Gloves: 27994; Feet: 27997),
        (Id: 111; Name: 'Гнев падшего бога разрушений'; Head: 19326; Chest: 19327; Legs: 19331; Gloves: 19329; Feet: 19332),
        (Id: 162; Name: 'Гнев падшего рыцаря'; Head: 26932; Chest: 26933; Legs: 26934; Gloves: 26935; Feet: 26936),
        (Id: 52; Name: 'Дельфийские кожаные доспехи'; Head: 20756; Chest: 20783; Legs: 20810; Gloves: 20837; Feet: 20864),
        (Id: 79; Name: 'Дельфийские латные доспехи'; Head: 20945; Chest: 20972; Legs: 20999; Gloves: 21026; Feet: 21053),
        (Id: 25; Name: 'Дельфийские матерчатые доспехи'; Head: 20569; Chest: 20596; Legs: 20623; Gloves: 20650; Feet: 20677),
        (Id: 143; Name: 'Доспехи Стального братства'; Head: 24480; Chest: 24481; Legs: 24482; Gloves: 24483; Feet: 24484),
        (Id: 135; Name: 'Доспехи безликого стража'; Head: 23780; Chest: 23781; Legs: 23782; Gloves: 0; Feet: 23784),
        (Id: 117; Name: 'Доспехи боевого инструктора'; Head: 0; Chest: 23752; Legs: 23753; Gloves: 0; Feet: 23755),
        (Id: 127; Name: 'Доспехи командира арсенала'; Head: 23813; Chest: 23814; Legs: 23815; Gloves: 0; Feet: 23817),
        (Id: 121; Name: 'Доспехи королевы разбойников'; Head: 0; Chest: 24439; Legs: 24440; Gloves: 0; Feet: 24441),
        (Id: 115; Name: 'Доспехи охотника на демонов огня'; Head: 0; Chest: 23747; Legs: 23748; Gloves: 0; Feet: 23750),
        (Id: 122; Name: 'Доспехи повелителя колоссов'; Head: 0; Chest: 24446; Legs: 24447; Gloves: 0; Feet: 24448),
        (Id: 120; Name: 'Доспехи пронизывающего ветра'; Head: 0; Chest: 24443; Legs: 24444; Gloves: 0; Feet: 24445),
        (Id: 113; Name: 'Доспехи рыцаря-узурпатора'; Head: 19333; Chest: 19334; Legs: 19338; Gloves: 19336; Feet: 19339),
        (Id: 61; Name: 'Закаленные латные доспехи'; Head: 20927; Chest: 20954; Legs: 20981; Gloves: 21008; Feet: 21035),
        (Id: 37; Name: 'Ирамийские кожаные доспехи'; Head: 20741; Chest: 20768; Legs: 20795; Gloves: 20822; Feet: 20849),
        (Id: 64; Name: 'Ирамийские латные доспехи'; Head: 20930; Chest: 20957; Legs: 20984; Gloves: 21011; Feet: 21038),
        (Id: 9; Name: 'Ирамийские матерчатые доспехи'; Head: 20553; Chest: 20580; Legs: 20607; Gloves: 20634; Feet: 20661),
        (Id: 45; Name: 'Иферийские кожаные доспехи'; Head: 20749; Chest: 20776; Legs: 20803; Gloves: 20830; Feet: 20857),
        (Id: 72; Name: 'Иферийские латные доспехи'; Head: 20938; Chest: 20965; Legs: 20992; Gloves: 21019; Feet: 21046),
        (Id: 18; Name: 'Иферийские матерчатые доспехи'; Head: 20562; Chest: 20589; Legs: 20616; Gloves: 20643; Feet: 20670),
        (Id: 98; Name: 'Комплект алхимика'; Head: 24846; Chest: 24847; Legs: 24848; Gloves: 24849; Feet: 24850),
        (Id: 100; Name: 'Комплект декоратора'; Head: 24856; Chest: 24857; Legs: 24858; Gloves: 24859; Feet: 24860),
        (Id: 107; Name: 'Комплект закройщика'; Head: 24891; Chest: 24892; Legs: 24893; Gloves: 24894; Feet: 24895),
        (Id: 102; Name: 'Комплект земледельца'; Head: 24866; Chest: 24867; Legs: 24868; Gloves: 24869; Feet: 24870),
        (Id: 108; Name: 'Комплект кожевника'; Head: 24896; Chest: 24897; Legs: 24898; Gloves: 24899; Feet: 24900),
        (Id: 105; Name: 'Комплект кузнеца'; Head: 24881; Chest: 24882; Legs: 24883; Gloves: 24884; Feet: 24885),
        (Id: 99; Name: 'Комплект кулинара'; Head: 24851; Chest: 24852; Legs: 24853; Gloves: 24854; Feet: 24855),
        (Id: 103; Name: 'Комплект лесоруба'; Head: 24871; Chest: 24872; Legs: 24873; Gloves: 24874; Feet: 24875),
        (Id: 109; Name: 'Комплект оружейника'; Head: 24901; Chest: 24902; Legs: 24903; Gloves: 24904; Feet: 24905),
        (Id: 110; Name: 'Комплект плотника'; Head: 24906; Chest: 24907; Legs: 24908; Gloves: 24909; Feet: 24910),
        (Id: 106; Name: 'Комплект рудокопа'; Head: 24886; Chest: 24887; Legs: 24888; Gloves: 24889; Feet: 24890),
        (Id: 101; Name: 'Комплект скотовода'; Head: 24861; Chest: 24862; Legs: 24863; Gloves: 24864; Feet: 24865),
        (Id: 104; Name: 'Комплект травника'; Head: 24876; Chest: 24877; Legs: 24878; Gloves: 24879; Feet: 24880),
        (Id: 160; Name: 'Месть запертого мученика'; Head: 26922; Chest: 26923; Legs: 26924; Gloves: 26925; Feet: 26926),
        (Id: 151; Name: 'Ненависть безжалостного убийцы'; Head: 24510; Chest: 24511; Legs: 24512; Gloves: 24513; Feet: 24514),
        (Id: 142; Name: 'Одеяние Черной Чешуи'; Head: 24470; Chest: 24471; Legs: 24472; Gloves: 24473; Feet: 24474),
        (Id: 134; Name: 'Одеяние раба крови'; Head: 23770; Chest: 23771; Legs: 23772; Gloves: 0; Feet: 23774),
        (Id: 116; Name: 'Одеяние сказителя Аль-Харбы'; Head: 0; Chest: 23741; Legs: 23742; Gloves: 0; Feet: 23744),
        (Id: 126; Name: 'Одеяние ученика пироманта'; Head: 23803; Chest: 23804; Legs: 23805; Gloves: 0; Feet: 23807),
        (Id: 168; Name: 'Оптика темного наблюдателя'; Head: 27725; Chest: 27726; Legs: 27727; Gloves: 27728; Feet: 27729),
        (Id: 62; Name: 'Отполированные латные доспехи'; Head: 20928; Chest: 20955; Legs: 20982; Gloves: 21009; Feet: 21036),
        (Id: 169; Name: 'Послание кровавого жнеца'; Head: 27717; Chest: 27716; Legs: 27715; Gloves: 27714; Feet: 27713),
        (Id: 35; Name: 'Проклепанные кожаные доспехи'; Head: 20739; Chest: 20766; Legs: 20793; Gloves: 20820; Feet: 20847),
        (Id: 6; Name: 'Проклепанные матерчатые доспехи'; Head: 20550; Chest: 20577; Legs: 20604; Gloves: 20631; Feet: 20658),
        (Id: 31; Name: 'Простые кожаные доспехи'; Head: 20735; Chest: 20762; Legs: 20789; Gloves: 20816; Feet: 20843),
        (Id: 58; Name: 'Простые латные доспехи'; Head: 20924; Chest: 20951; Legs: 20978; Gloves: 21005; Feet: 21032),
        (Id: 2; Name: 'Простые матерчатые доспехи'; Head: 20546; Chest: 20573; Legs: 20600; Gloves: 20627; Feet: 20654),
        (Id: 34; Name: 'Прошитые кожаные доспехи'; Head: 20738; Chest: 20765; Legs: 20792; Gloves: 20819; Feet: 20846),
        (Id: 5; Name: 'Прошитые матерчатые доспехи'; Head: 20549; Chest: 20576; Legs: 20603; Gloves: 20630; Feet: 20657),
        (Id: 161; Name: 'Пытки старшего инквизитора'; Head: 26927; Chest: 26928; Legs: 26929; Gloves: 26930; Feet: 26931),
        (Id: 112; Name: 'Решимость сильного привратника'; Head: 19348; Chest: 19349; Legs: 19353; Gloves: 19351; Feet: 19354),
        (Id: 150; Name: 'Сила изгнанного мага'; Head: 24493; Chest: 24494; Legs: 24495; Gloves: 24496; Feet: 24497),
        (Id: 125; Name: 'Снаряжение бандита Кровавой Руки'; Head: 23808; Chest: 23809; Legs: 23810; Gloves: 0; Feet: 23812),
        (Id: 133; Name: 'Снаряжение зловещего надзирателя'; Head: 23775; Chest: 23776; Legs: 23777; Gloves: 0; Feet: 23779),
        (Id: 141; Name: 'Снаряжение охотника на змей'; Head: 24475; Chest: 24476; Legs: 24477; Gloves: 24478; Feet: 24479),
        (Id: 164; Name: 'Страсть забытого пирата'; Head: 27802; Chest: 27801; Legs: 27804; Gloves: 27800; Feet: 27803),
        (Id: 167; Name: 'Тюремная камера песни теней'; Head: 27718; Chest: 27721; Legs: 27719; Gloves: 27720; Feet: 27722),
        (Id: 33; Name: 'Укрепленные кожаные доспехи'; Head: 20737; Chest: 20764; Legs: 20791; Gloves: 20818; Feet: 20845),
        (Id: 60; Name: 'Укрепленные латные доспехи'; Head: 20926; Chest: 20953; Legs: 20980; Gloves: 21007; Feet: 21034),
        (Id: 4; Name: 'Укрепленные матерчатые доспехи'; Head: 20548; Chest: 20575; Legs: 20602; Gloves: 20629; Feet: 20656),
        (Id: 32; Name: 'Улучшенные кожаные доспехи'; Head: 20736; Chest: 20763; Legs: 20790; Gloves: 20817; Feet: 20844),
        (Id: 59; Name: 'Улучшенные латные доспехи'; Head: 20925; Chest: 20952; Legs: 20979; Gloves: 21006; Feet: 21033),
        (Id: 3; Name: 'Улучшенные матерчатые доспехи'; Head: 20547; Chest: 20574; Legs: 20601; Gloves: 20628; Feet: 20655),
        (Id: 41; Name: 'Эрноанские кожаные доспехи'; Head: 20745; Chest: 20772; Legs: 20799; Gloves: 20826; Feet: 20853),
        (Id: 68; Name: 'Эрноанские латные доспехи'; Head: 20934; Chest: 20961; Legs: 20988; Gloves: 21015; Feet: 21042),
        (Id: 13; Name: 'Эрноанские матерчатые доспехи'; Head: 20557; Chest: 20584; Legs: 20611; Gloves: 20638; Feet: 20665)
      );

      CharacterModels: array [0..7] of TCharacterModel = (
        (race: 1; gender: 1; face: 19838; body: 536; id: 298; model_id: 10; hair_id: 24133; hair_color_id: 733; skin_color_id: 1; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 0; face_fixed_decal_asset_2_id: 560; face_fixed_decal_asset_3_id: 682; face_diffuse_map_id: 0; face_normal_map_id: 29; face_eyelash_map_id: 0; lip_color: 0; left_pupil_color: 4294489434; right_pupil_color: 4294489434; eyebrow_color: 4278199100; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 4282924640; name: 'nu_m_face0001'; npcOnly: 'f';
        modifier: '00F5000011DC000B00000000170000000000F323000000003D000000000000000' + '00000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 1),
        (race: 1; gender: 2; face: 19839; body: 539; id: 325; model_id: 11; hair_id: 25372; hair_color_id: 4299; skin_color_id: 4; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 444; face_fixed_decal_asset_2_id: 170; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 0; face_eyelash_map_id: 0; lip_color: 4287331299; left_pupil_color: 4293913775; right_pupil_color: 4293913775; eyebrow_color: 4281878616; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'nu_f_face0001'; npcOnly: 'f';
        modifier: '00EF00EF00EE000103000000000000110000000000FE00063BB900D800EE00D40' + '0281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 0.71; face_fixed_decal_asset_1_weight: 1),
        (race: 4; gender: 1; face: 23713; body: 1142; id: 307; model_id: 16; hair_id: 25361; hair_color_id: 765; skin_color_id: 5; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 738; face_fixed_decal_asset_2_id: 596; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 45; face_eyelash_map_id: 0; lip_color: 4287133659; left_pupil_color: 4294175843; right_pupil_color: 4294175843; eyebrow_color: 4278199857; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'el_m_face0001'; npcOnly: 'f';
        modifier: '00DA00E6F6F4002700000000000033F3000000320000FC0B40D400FB000000D30' + '0E2EF000000000000D41D000000000028000000E1000D00EA000000000007002C00000000D300000000000000100000CC0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 0.31; face_fixed_decal_asset_1_weight: 0.43),
        (race: 4; gender: 2; face: 23714; body: 1128; id: 124; model_id: 17; hair_id: 24044; hair_color_id: 782; skin_color_id: 8; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 0; face_fixed_decal_asset_2_id: 243; face_fixed_decal_asset_3_id: 358; face_diffuse_map_id: 0; face_normal_map_id: 24; face_eyelash_map_id: 0; lip_color: 4286806015; left_pupil_color: 4293519647; right_pupil_color: 4293519647; eyebrow_color: 4278199083; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'el_f_face0006'; npcOnly: 'f';
        modifier: '00F000EA0CFC00E900D1281F00E4DF3200004FCDF40001FC64FE0000000000000' + '011F7E1F80C070020002C00F5200027520B002ECFC5000029000000000003000000002100000000243812000000F400004212D10000070000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 0.72),
        (race: 5; gender: 1; face: 23715; body: 548; id: 316; model_id: 18; hair_id: 24140; hair_color_id: 3958; skin_color_id: 13; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 763; face_fixed_decal_asset_2_id: 637; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 49; face_eyelash_map_id: 0; lip_color: 4285094337; left_pupil_color: 4278861223; right_pupil_color: 4278861223; eyebrow_color: 4278257938; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 4282924640; name: 'ha_m_face0001'; npcOnly: 'f';
        modifier: '00EC00CA000000000000120000000EEB000000031C00000431E40000000000000' + '0F800000000000000EF00000031000000000000E51C0000DF000000EBE0E60A00002DA7EACE0000CE000000FA000000E00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 0.36),
        (race: 5; gender: 2; face: 23716; body: 553; id: 170; model_id: 19; hair_id: 25129; hair_color_id: 3853; skin_color_id: 15; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 397; face_fixed_decal_asset_2_id: 203; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 13; face_eyelash_map_id: 0; lip_color: 4282269373; left_pupil_color: 4281090181; right_pupil_color: 4281090181; eyebrow_color: 4278324512; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'ha_f_face0004'; npcOnly: 'f';
        modifier: '0000E8EA00FC0000390019000000A7000000002E0000001D5100EACF00E600EBE' + 'E0800000000000000221100000130E4000000FF14000700C400000000000917000000BA10D5000000649CFF0030F70000D800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 1),
        (race: 6; gender: 1; face: 20117; body: 559; id: 45; model_id: 20; hair_id: 8152; hair_color_id: 6328; skin_color_id: 9; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 0; face_fixed_decal_asset_2_id: 786; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 60; face_eyelash_map_id: 0; lip_color: 4287657983; left_pupil_color: 4282778368; right_pupil_color: 4282778368; eyebrow_color: 4280549888; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'fe_m_face0003'; npcOnly: 'f';
        modifier: '00D200DFDAD01700000000646400006400649C9C64F61111001600CAD2159C0BF' + '4F2AC1629640A9C0A070D000009469CD0BD49C464006400E1009C06081D00CB2E0000DB00CA6400B7646400DD642200009C06000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 1),
        (race: 6; gender: 2; face: 20118; body: 565; id: 9; model_id: 21; hair_id: 16245; hair_color_id: 2179; skin_color_id: 11; face_movable_decal_asset_id: 0; face_movable_decal_scale: 1; face_movable_decal_rotate: 0; face_movable_decal_move_x: 0; face_movable_decal_move_y: 0; face_fixed_decal_asset_0_id: 0; face_fixed_decal_asset_1_id: 869; face_fixed_decal_asset_2_id: 825; face_fixed_decal_asset_3_id: 0; face_diffuse_map_id: 0; face_normal_map_id: 0; face_eyelash_map_id: 0; lip_color: 4281008320; left_pupil_color: 4290534656; right_pupil_color: 4290534656; eyebrow_color: 4294436351; owner_type_id: 1; face_movable_decal_weight: 1; face_fixed_decal_asset_0_weight: 1; face_fixed_decal_asset_2_weight: 1; face_normal_map_weight: 1; deco_color: 0; name: 'fe_f_face0001'; npcOnly: 'f';
        modifier: '00FD6426531C41AD00210000AB00002D005A64FC0000000E64100000001963001' + '033AEED04000700E10011B713162600B61D28C6E7FA00D60A2B00FEDB14120600F49C9C6400649CD4E80001330000000000C5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'; face_fixed_decal_asset_3_weight: 1; face_fixed_decal_asset_1_weight: 1)
      );
        

    //------------------------------------------------------------------------------------------------------
        procedure TfrmMain.SendCharacterList(Stream: TGameStream; Char: TCharacter);
        var
          P: TGamePacket;
          S: RawByteString;
          I: Integer;
       
       procedure WriteItem(ItemId: Integer);
            var
              I: Integer;
          begin
            P.WriteD(ItemId);
            if ItemId > 0 then
              begin
                P.WriteD(1); // ObjectId
                for I := 1 to 6 do P.WriteC(0);
                P.WriteC(1);
                for I := 1 to 3 do P.WriteC(0);
                P.WriteC(1);
                for I := 1 to 4 do P.WriteC(0);
                P.WriteC(1);
                for I := 1 to 62 do P.WriteC(0);
                P.WriteC(3);
                for I := 1 to 16 do P.WriteC(0);
              end;
          end;
        
        begin
          P := TGamePacket.Create;
          try
            P.Id := $003A01DD;
            P.WriteC(1);
            P.WriteC(1);
            P.WriteD(Char.Id);
            P.WriteS(Char.Name);
       
         P.WriteC(Char.Model.race);
            P.WriteC(Char.Model.gender);
            P.WriteC(Char.Level);
            P.WriteC($64);
            P.WriteC(1);
            P.WriteC(0);
            P.WriteC(0);
            P.WriteD($178);
            P.WriteD(180); // zone_id?
            P.WriteC(101); // faction_id?
            for I := 1 to 21 do P.WriteC(0);
       
         WriteItem(Char.Chest);
            WriteItem(Char.Head);
            WriteItem(Char.Legs);
            WriteItem(Char.Gloves);
            WriteItem(Char.Feet);
       
         P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
       
         WriteItem(Char.Weapon);
            WriteItem(Char.WeaponExtra);
            WriteItem(Char.WeaponRanged);
            WriteItem(Char.Instrument);
       
         P.WriteD(Char.Model.face);
            P.WriteD(Char.Model.hair_id);
       
         P.WriteD(0);
            P.WriteD(0);
            P.WriteD(0);
       
         P.WriteD(Char.Model.body);
       
         S := RawByteString(#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$01 +
            #$0B#$0B#$00#$00#$00#$00#$28#$5F#$CD#$03#$00#$00#$00#$00#$40#$2F +
            #$C1#$03#$AC#$7D#$FC#$42#$03);
            P.WriteB(@S[1], Length(S));
       
         P.WriteD(Char.Model.hair_color_id);
            P.WriteD(Char.Model.skin_color_id);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteSingle(1);
            P.WriteSingle(1);
            P.WriteD(0);
            P.WriteD(0);
            P.WriteD(Char.Model.face_fixed_decal_asset_0_id);
            P.WriteSingle(Char.Model.face_fixed_decal_asset_0_weight);
            P.WriteD(Char.Model.face_fixed_decal_asset_1_id);
            P.WriteSingle(Char.Model.face_fixed_decal_asset_1_weight);
            P.WriteD(Char.Model.face_fixed_decal_asset_2_id);
            P.WriteSingle(Char.Model.face_fixed_decal_asset_2_weight);
            P.WriteD(Char.Model.face_fixed_decal_asset_3_id);
            P.WriteSingle(Char.Model.face_fixed_decal_asset_3_weight);
            P.WriteD(0);
            P.WriteD(Char.Model.face_normal_map_id);
            P.WriteD(0);
            P.WriteSingle(1);
            P.WriteD(0);
            P.WriteD(Char.Model.left_pupil_color);
            P.WriteD(Char.Model.right_pupil_color);
            P.WriteD(Char.Model.eyebrow_color);
            P.WriteD(Char.Model.deco_color);
       
         S := HexToStr(Char.Model.modifier);
            P.WriteH(Length(S));
            if S<> '' then
             begin
                P.WriteB(@S[1], Length(S));
              end;
       
         P.WriteD(500); // очки работы
       
         S := RawByteString(#$BF +
            #$F5#$F0#$52#$00#$00#$00#$00#$00#$00#$6B#$52#$EF#$52#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$6B#$52#$EF#$52#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$7A#$F5#$F0#$52#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$00#$00#$00#$00#$03#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00#$00 +
            #$00#$00#$5E#$D5#$F0#$52#$00#$00#$00#$00);
            P.WriteB(@S[1], Length(S));
       
         Stream.Send(P);
          finally
            FreeAndNil(P);
            end;
        end;
       */

    public sealed class NP_CharacterListPacket_0x0039 : NetPacket
    {
        /// <summary>
        /// пакет для входа в Лобби
        /// CharacterListPacket01_0x0039
        /// 
        /// author: NLObP
        /// </summary>
        /// <param name="net"></param>
        /// 


        public NP_CharacterListPacket_0x0039(ClientConnection net) : base(01, 0x0039)
        {
            var accid = net.CurrentAccount.AccountId;
            List<Character> charList = CharacterHolder.LoadCharacterData();
            var totalChars = charList.Count;

            //<packet id="0x003901" name="CharacterListPacket">
            //"last" type="c"
            ns.Write((byte)0x01);
            //<part id="0" name="count" type="c"
            ns.Write((byte)totalChars);
            //for (int i = 0; i < totalChars; i++)
            foreach (Character chr in charList)
            {
                //<for id="0">
                //  <!-- x times CreateCharacterResponsePacket -->
                //  "type" type="d"
                ns.Write((int)chr.Type[0]);
                //  "name" type="SS"
                ns.WriteUTF8Fixed(chr.CharName, chr.CharName.Length);
                //  "CharRace" type="c"
                ns.Write((byte)chr.CharRace);
                //  "CharGender" type="c"
                ns.Write((byte)chr.CharGender);
                //  "level" type="c"
                ns.Write((byte)chr.Level);
                //  "health" type="d"
                ns.Write((int)0x02D0);
                //  "mana" type="d"
                ns.Write((int)0x029E);
                //  "zid" type="d"
                //  "type" type="d"
                //  "factionName" type="SS"
                //  "type" type="d"
                //  "family" type="d"
                //  <for id="-1" size="19">
                //    <part id="2" name="type[1]" type="d"
                //    <switch id="2">
                //      <case id="0">
                //      </case>
                //      <case id="default">
                //         "id[1]" type="Q"
                //         "type[1]" type="c"
                //         "flags[1]" type="c"
                //         "stackSize[1]" type="d"
                //         <part id="3" name="detailType" type="c"
                //         <switch id="3">
                //           <case id="0">
                //           </case>
                //           <case id="1">
                //             "detail" size="51" type="b"
                //           </case>
                //           <case id="2">
                //             "detail" size="29" type="b"
                //           </case>
                //           <case id="3">
                //             "detail" size="6" type="b"
                //           </case>
                //           <case id="4">
                //           "detail" size="9" type="b"
                //           </case>
                //           <case id="5">
                //             "type" type="d"
                //             "x" type="Q"
                //             "y" type="Q"
                //             "z" type="f"
                //           </case>
                //           <case id="6">
                //             "detail" size="16" type="b"
                //           </case>
                //           <case id="7">
                //             "detail" size="16" type="b"
                //           </case>
                //           <case id="8">
                //              "detail" size="8" type="b"
                //           </case>
                //         </switch id="3>
                //         "creationTime[1]" type="Q"
                //         "lifespanMins[1]" type="d"
                //         "type[1]" type="d"
                //         "worldId" type="c"
                //         "unsecureDateTime" type="Q"
                //         "unpackDateTime" type="Q"
                //      </case default>
                //    </switch id=2>
                //  </for id =-1>
                //  <for id="-1" size="7">
                //"type[somehow_special]" type="d"
                //</for id=0>
                //<for id="-1" size="2">
                //  <part id="4" name="type[1]" type="d"
                //  <switch id="4">
                //    <case id="0">
                //    </case>
                //    <case id="default">
                //      "id[1]" type="Q"
                //      "type[1]" type="c"
                //      "flags[1]" type="c"
                //      "stackSize[1]" type="d"
                //      <part id="5" name="detailType" type="c"
                //      <switch id="5">
                //        <case id="0">
                //        </case>
                //        <case id="1">
                //          "detail" size="51" type="b"
                //        </case>
                //        <case id="2">
                //          "detail" size="29" type="b"
                //        </case>
                //        <case id="3">
                //          "detail" size="6" type="b"
                //        </case>
                //        <case id="4">
                //          "detail" size="9" type="b"
                //        </case>
                //        <case id="5">
                //          "type" type="d"
                //          "x" type="Q"
                //          "y" type="Q"
                //          "z" type="f"
                //        </case>
                //        <case id="6">
                //          "detail" size="16" type="b"
                //        </case>
                //        <case id="7">
                //          "detail" size="16" type="b"
                //        </case>
                //        <case id="8">
                //          "detail" size="8" type="b"
                //        </case>
                //      </switch  id="5">
                //      "creationTime[1]" type="Q"
                //      "lifespanMins[1]" type="d"
                //      "type[1]" type="d"
                //      "worldId" type="c"
                //      "unsecureDateTime" type="Q"
                //      "unpackDateTime" type="Q"
                //    </case id="default">
                //  </switch id="4">
                //</for id="-1" size="2">
                //"ability[0]" type="c"
                //"ability[1]" type="c"
                //"ability[2]" type="c"
                //"x" type="Q"
                //"y" type="Q"
                //"z" type="f"
                //<part id="6" name="ext" type="c"
                //<switch id="6">
                //  <case id="0">
                //  </case>
                //  <case id="1">
                //    "type" type="d"
                //  </case>
                //  <case id="2">
                //    "type" type="d"
                //    "type" type="d"
                //    "type" type="d"
                //  </case>
                //  <case id="default">
                //    "type" type="d"
                //    "type" type="d"
                //    "type" type="d"
                //    "type" type="d"
                //    "weight" type="f"
                //    "scale" type="f"
                //    "rotate" type="f"
                //    "moveX" type="h"
                //    "moveY" type="h"
                //    <for id="-1" size="4">
                //      "type" type="d"
                //      "weight" type="f"
                //    </for>
                //    "type" type="d"
                //    "type" type="d"
                //    "type" type="d"
                //    "weight" type="f"
                //    "lip" type="d"
                //    "leftPupil" type="d"
                //    "rightPupil" type="d"
                //    "eyebrow" type="d"
                //    "decor" type="d"
                //    <part id="7" name="modifiers_len" type="h"
                //    "modifiers" sizeid="7" type="b"
                //  </case>
                //</switch>
                //"laborPower" type="h"
                //"lastLaborPowerModified" type="Q"
                //"deadCount" type="h"
                //"deadTime" type="Q"
                //"rezWaitDuration" type="d"
                //"rezTime" type="Q"
                //"rezPenaltyDuration" type="d"
                //"lastWorldLeaveTime" type="Q"
                //"moneyAmount" type="Q"
                //"moneyAmount" type="Q"
                //"crimePoint" type="h"
                //"crimeRecord" type="d"
                //"crimeScore" type="h"
                //"deleteRequestedTime" type="Q"
                //"transferRequestedTime" type="Q"
                //"deleteDelay" type="Q"
                //"consumedLp" type="d"
                //"bmPoint" type="Q"
                //"moneyAmount" type="Q"
                //"moneyAmount" type="Q"
                //"autoUseAApoint" type="A"
                //"point" type="d"
                //"gift" type="d"
                //"updated" type="Q"
                //</for>
                //</packet>
            }
        }

    }
}

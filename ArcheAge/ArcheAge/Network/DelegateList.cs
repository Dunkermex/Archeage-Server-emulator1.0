﻿using System;
using ArcheAge.ArcheAge.Network.Connections;
using ArcheAge.ArcheAge.Structuring;
using ArcheAge.ArcheAge.Holders;
using LocalCommons.Logging;
using LocalCommons.Network;
using System.Collections.Generic;
using System.Linq;
using LocalCommons.Utilities;

namespace ArcheAge.ArcheAge.Network
{
    /// <summary>
    /// Delegate List That Contains Information About Received Packets.
    /// Contains a list of delegates that receive packet information.
    /// </summary>
    public class DelegateList
    {
        private static string clientVersion;
        private static int m_Maintained;
        private static PacketHandler<LoginConnection>[] m_LHandlers;
        //private static PacketHandler<ClientConnection>[] m_CHandlers;
        private static Dictionary<int, PacketHandler<ClientConnection>[]> levels;
        private static LoginConnection m_CurrentLoginServer;
        private static bool enter1;
        private static bool enter2;
        private static bool enter3;
        private static bool enter4;
        private static bool enter5;
        private static bool enter6;
        private static bool enter7;
        private static bool enter8;
        private static bool enter9;
        private static bool once2;
        private static bool once3;
        private static bool once4;
        private static bool once5;
        private static bool once6;

        public static LoginConnection CurrentLoginServer
        {
            get { return m_CurrentLoginServer; }
        }

        public static Dictionary<int, PacketHandler<ClientConnection>[]> ClientHandlers
        {
            get { return levels; }
        }

        public static PacketHandler<LoginConnection>[] LHandlers
        {
            get { return m_LHandlers; }
        }

        public static void Initialize(string clientVersion)
        {
            DelegateList.clientVersion = clientVersion;
            m_LHandlers = new PacketHandler<LoginConnection>[0x20];
            //m_LHandlers = new PacketHandler<ClientConnection>[0x30];
            levels = new Dictionary<int, PacketHandler<ClientConnection>[]>();

            once2 = true; // если false, то больше не повторять
            once3 = true; // если false, то больше не повторять
            once4 = true; // если false, то больше не повторять
            once5 = true; // если false, то больше не повторять
            once6 = true; // если false, то больше не повторять
            enter1 = false; // если true, то больше не повторять
            enter4 = false; // если true, то больше не повторять
            enter5 = false; // если true, то больше не повторять
            enter6 = false; // если true, то больше не повторять
            enter7 = false; // если true, то больше не повторять
            enter8 = false; // если true, то больше не повторять
            enter9 = false; // если true, то больше не повторять

            RegisterDelegates();
        }
        /// <summary>
        /// Registration service
        /// Using - Packet Level - Packet Opcode(short) - Receive Delegate
        /// </summary>
        private static void RegisterDelegates()
        {
            //-------------- Login - Game Communication Packets ------------
            //Game Communication Service
            Register(0x00, Handle_GameRegisterResult); //Taken Fully
            Register(0x01, (net, reader) => Handle_AccountInfoReceived(clientVersion, net, reader)); //Taken Fully
            switch (clientVersion)
            {
                case "1": //1.0.1406 Feb 11 2014
                    Register(0x01, 0x0000, (net, reader) => OnPacketReceive_X2EnterWorld(clientVersion, net, reader)); //+
                    Register(0x02, 0x0012, OnPacketReceive_Ping); //+
                    Register(0x02, 0x0001, (net, reader) => OnPacketReceive_FinishState0201(clientVersion, net, reader)); //+
                    Register(0x01, 0x001F, OnPacketReceive_CSListCharacter_0x001F);
                    Register(0x01, 0x001, OnPacketReceive_CSLeaveWorld_0x0001);
                    Register(0x01, 0x021, OnPacketReceive_CSCreateCharacter_0x0021);
                    Register(0x01, 0x023, OnPacketReceive_DeleteCharacter_0x0023);
                    //Entrance to the world
                    Register(0x01, 0x0020, CSRefreshInCharacterList);
                    Register(0x01, 0x0116, SCMailReceiverOpened);
                    Register(0x01, 0x0024, SelectCharacter);
                    Register(0x01, 0x0025, SpawnCharacter);
                    Register(0x01, 0x0027, NotifyInGame);
                    break;
                case "3": //3.0.3.0

                    //-------------- Client Communication Packets ------------------
                    //Client Communication Service
                    //-------------- Using - Packet Level - Packet Opcode(short) - Receive Delegate -----
                    Register(0x01, 0x0000, (net, reader) => OnPacketReceive_X2EnterWorld(clientVersion, net, reader)); //+
                    Register(0x02, 0x0012, OnPacketReceive_Ping); //+
                    Register(0x02, 0x0001, (net, reader) => OnPacketReceive_FinishState0201(clientVersion, net, reader)); //+
                    Register(0x01, 0xE4FB, OnPacketReceive_ClientE4FB);
                    Register(0x01, 0x0D7C, OnPacketReceive_Client0D7C);
                    Register(0x01, 0xE17B, OnPacketReceive_ClientE17B);
                    Register(0x05, 0x0438, OnPacketReceive_Client0438);
                    Register(0x05, 0x0088, OnPacketReceive_ReloginRequest_0x0088);
                    Register(0x05, 0x008A, OnPacketReceive_EnterWorld_0x008A); //вход в игру1
                    Register(0x05, 0x008B, OnPacketReceive_EnterWorld_0x008B); //вход в игру2
                    Register(0x05, 0x008C, OnPacketReceive_EnterWorld_0x008C); //вход в игру3
                    Register(0x05, 0x008D, OnPacketReceive_EnterWorld_0x008D); //вход в игру4
                    Register(0x05, 0x008E, OnPacketReceive_EnterWorld_0x008E); //вход в игру5
                    Register(0x05, 0x008F, OnPacketReceive_EnterWorld_0x008F); //вход в игру6
                    break;
                default:
                    break;
            }
        }

        #region Client Callbacks Implementation

        private static void OnPacketReceive_DeleteCharacter_0x0023(ClientConnection net, PacketReader reader)
        {
            //"Recv: 0800 0001 2300 FF091A00
            var accountID = reader.ReadLEInt32(); //accountID D
            net.SendAsync(new NP_SCDeleteCharacterResponse_0x0034(accountID)); //удаляем выбранного героя
            net.SendAsync(new NP_Packet_CharList01_0x0039(net)); //шлем список оставшихся героев
        }

        /// <summary>
        /// TODO: CSEditCharacterPacket 0x1C очень похож на этот пакет, на не на CSCreateCharacter_0x0021 0x1B
        /// </summary>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        private static void OnPacketReceive_CSCreateCharacter_0x0021(ClientConnection net, PacketReader reader)
        {
            Character newCharacter = new Character(); //куда записывать параметры нового героя

            newCharacter.AccountId = net.CurrentAccount.AccountId;
            //                     len  name      
            //Recv: 1801 0001 2100 0900 72656D6F746164696E 01 02 7F4D0000 1C 63 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 CB 10 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F BC 01 00 00 00 00 80 3F AA 00 00 00 00 00 80 3F 00 00 00 00 8F C2 35 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F E3 7B 8B FF AF EC EF FF AF EC EF FF 58 48 38 FF 00 00 00 00 80 00 00 EF 00 EF 00 EE 00 01 03 00 00 00 00 00 00 11 00 00 00 00 00 FE 00 06 3B B9 00 D8 00 EE 00 D4 00 28 1B EB E1 00 E7 00 F0 37 23 00 00 00 00 00 64 00 00 00 00 00 00 00 64 00 00 00 F0 00 00 00 00 00 00 00 2B D5 00 00 00 64 00 00 00 00 F9 00 00 00 E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 0B 0B 01
            //Recv: 1401 0001 2100 0500 6172747572         01 01 7E4D0000 455E0000 00000000 00000000 00000000 00000000 00000000 03 DD020000 01 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 80 3F 30 02 00 00 00 00 80 3F AA 02 00 00 00 00 80 3F 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 5A B5 F8 FF 5A B5 F8 FF 3C 23 00 FF 60 3E 48 FF 80 00 00 F5 00 00 11 DC 00 0B 00 00 00 00 17 00 00 00 00 00 F3 23 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 0B 0B 01
            //- <packet id="0x002101" name="CSCreateCharacterPacket">
            //  <part name="name" type="SS" /> 
            short len = reader.ReadLEInt16();
            newCharacter.CharName = reader.ReadString(len); //имя героя
            //  <part name="CharRace" type="c" /> 
            newCharacter.CharRace = reader.ReadByte(); //раса героя
            //  <part name="CharGender" type="c" /> 
            newCharacter.CharGender = reader.ReadByte(); //пол героя
            //цикл по характеристикам предметов на герое: 1.Head, 2.Chest, 3.Legs, 4.Gloves, 5.Feet, 6.unk(меч), 7.unk(лук)
            for (int i = 0; i < 7; i++)
            {
                //  - <for id="-1" size="7">
                //    <part name="type[somehow_special]" type="d" /> 
                newCharacter.Type[i] = reader.ReadLEInt32();
                //  </for>
            }
            //  <part id="6" name="ext" type="c" /> 
            byte ext = reader.ReadByte();
            //    - <switch id="6">
            switch (ext)
            {
                case 0:
                    //      <case id="0" /> 
                    break;
                case 1:
                    //      - <case id="1">
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[7] = reader.ReadLEInt32();
                    //      </case>
                    break;
                case 2:
                    //      - <case id="2">
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[7] = reader.ReadLEInt32();
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[8] = reader.ReadLEInt32();
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[9] = reader.ReadLEInt32();
                    //      </case>
                    break;
                default:
                    //      - <case id="default">
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[7] = reader.ReadLEInt32();
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[8] = reader.ReadLEInt32();
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[9] = reader.ReadLEInt32();
                    //        <part name="type" type="d" /> 
                    newCharacter.Type[10] = reader.ReadLEInt32();
                    //        <part name="weight" type="f" /> 
                    newCharacter.Weight[10] = reader.ReadLESingle(); //0-9 нет данных, 10
                    //        <part name="scale" type="f" /> 
                    newCharacter.Scale = reader.ReadLESingle();
                    //        <part name="rotate" type="f" /> 
                    //--newCharacter.Rotate = reader.ReadLESingle();   //почему то нехватило байтиков для Rotate?!
                    //        <part name="moveX" type="h" /> 
                    newCharacter.MoveX = reader.ReadLESingle();
                    //        <part name="moveY" type="h" /> 
                    newCharacter.MoveY = reader.ReadLESingle();
                    for (int i = 11; i < 15; i++)
                    {
                        //      - <for id="-1" size="4">
                        //        <part name="type" type="d" /> 
                        newCharacter.Type[i] = reader.ReadLEInt32();    //11, 12, 13, 14
                        //        <part name="weight" type="f" /> 
                        newCharacter.Weight[i] = reader.ReadLESingle(); //11, 12, 13, 14
                        //      </for>
                    }
                    //      <part name="type" type="d" /> 
                    newCharacter.Type[15] = reader.ReadLEInt32();
                    //      <part name="type" type="d" /> 
                    newCharacter.Type[16] = reader.ReadLEInt32();
                    //      <part name="type" type="d" /> 
                    newCharacter.Type[17] = reader.ReadLEInt32();
                    //      <part name="weight" type="f" /> 
                    newCharacter.Weight[17] = reader.ReadLESingle(); //17, 15-16 нет данных
                    //      <part name="lip" type="d" /> 
                    newCharacter.Lip= reader.ReadLEInt32();
                    //      <part name="leftPupil" type="d" /> 
                    newCharacter.LeftPupil = reader.ReadLEInt32();
                    //      <part name="rightPupil" type="d" /> 
                    newCharacter.RightPupil = reader.ReadLEInt32();
                    //      <part name="eyebrow" type="d" /> 
                    newCharacter.Eyebrow = reader.ReadLEInt32();
                    //      <part name="decor" type="d" /> 
                    newCharacter.Decor = reader.ReadLEInt32();
                    //      <part id="7" name="modifiers_len" type="h" /> 
                    short modifiersLen = reader.ReadLEInt16();
                    byte[] modfr = new byte[256];
                    //      <part name="modifiers" sizeid="7" type="b" /> 
                    modfr = reader.ReadByteArray(modifiersLen);
                    //newCharacter.Modifiers = reader.ReadHexString(modifiersLen);
                    newCharacter.Modifiers = Utility.ByteArrayToString(modfr);
                    //    </case>
                    //  </switch>
                    break;
            }
            //  <part name="a[0]" type="c" /> 
            newCharacter.A[0] = reader.ReadByte(); //0
            //  <part name="a[1]" type="c" /> 
            newCharacter.A[1] = reader.ReadByte(); //1
            //  <part name="a[2]" type="c" /> 
            newCharacter.A[2] = reader.ReadByte(); //2
            //  <part name="level" type="c" /> 
            newCharacter.Level = reader.ReadByte();
            //</packet>

            CharacterHolder.InsertOrUpdate(newCharacter); //записываем в базу character

        }

        private static void OnPacketReceive_CSLeaveWorld_0x0001(ClientConnection net, PacketReader reader)
        {

            //клиентский пакет на выход из игры Recv: 0500 0001 0100 00
            //клиентский пакет на релогин Recv: 0500 0001 0100 02

            int reason = reader.ReadByte();
            switch (reason)
            {
                case 0:
                    //quit from game
                    break;
                case 1:
                    //unk
                    break;
                case 2:
                    net.SendAsync(new NP_SCReconnectAuthPacket_0x0001(net)); //reconnect
                    break;
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008A(ClientConnection net, PacketReader reader)
        {
            if (!enter1) //регулируем последовательность входа
            {
                //клиентский пакет  Recv: 130000053829157BA816DB909183220859E934EFF6
                net.SendAsyncHex(new NP_EnterGame_008A());//вход в игру1, пакет A>s 0x038
                //net.SendAsyncHex(new NP_EnterGame_008A_2());//вход в игру1, пакет A>s 0x038
                enter1 = true;
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008B(ClientConnection net, PacketReader reader)
        {
            if (enter1) //регулируем последовательность входа
            {
                if (once2) //защитим от повтора посылки пакетов
                {
                    //вход в игру2
                    //13000005371947B88E92319E86B077729237FC244E
                    net.SendAsyncHex(new NP_EnterGame_008B());//вход в игру2, пакет A>s 0x037
                    //net.SendAsyncHex(new NP_EnterGame_008B_2());//вход в игру2, пакет A>s 0x037
                    enter2 = true;
                    once2 = false;
                }
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008C(ClientConnection net, PacketReader reader)
        {
            if (enter2) //регулируем последовательность входа
            {
                if (once3)
                {
                    //вход в игру3
                    //13000005390AEDA4C3949E6A5B4AC06820F2BC202A
                    //13000005370B469961E9F541A6AF4E8DB8BBB3EAFE
                    net.SendAsyncHex(new NP_EnterGame_008C());//вход в игру3, пакет A>s 0x039
                    //net.SendAsyncHex(new NP_EnterGame_008C_2());//вход в игру3, пакет A>s 0x039
                    enter3 = true;
                    enter2 = false;
                    once3 = false;
                }
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008D(ClientConnection net, PacketReader reader)
        {
            if (enter3)
            {
                if (once4)
                {
                    net.SendAsyncHex(new NP_EnterGame_008D());//вход в игру4, пакет A>s 0x03F
                    //net.SendAsyncHex(new NP_EnterGame_008D_2());//вход в игру4, пакет A>s 0x03F
                    enter4 = true;
                    enter3 = false;
                    once4 = false;
                }
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008E(ClientConnection net, PacketReader reader)
        {
            if (enter4)
            {
                if (once5)
                {
                    net.SendAsyncHex(new NP_EnterGame_008E());//вход в игру5, пакет A>s 0x033
                    //net.SendAsyncHex(new NP_EnterGame_008E_2());//вход в игру5, пакет A>s 0x033
                    once5 = false;
                    enter5 = true;
                    enter4 = false;
                }
            }
        }

        public static void OnPacketReceive_EnterWorld_0x008F(ClientConnection net, PacketReader reader)
        {
            if (enter5)
            {
                if (once6)
                {
                    //net.SendAsyncHex(new NP_EnterGame_008F());//вход в игру6, пакет A>s 0x036
                    once6 = false;
                    enter6 = true;
                    enter5 = false;
                }
            }
        }

        /// <summary>
        /// Verify user login permissions
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        public static void OnPacketReceive_X2EnterWorld(string clientVersion, ClientConnection net, PacketReader reader)
        {
            switch (clientVersion)
            {
                case "1":
                    //2100 0001 0000 B9040000BC0400001AC7000008C5B985FFFFFFFF0035CB020000000000
                    var pFrom01 = reader.ReadLEInt32(); //p_from D
                    var pTo01 = reader.ReadLEInt32(); //p_to D
                    var accountId01 = reader.ReadLEInt32(); //AccountId D
                    var cookie01 = reader.ReadLEInt32(); //cookie D
                    var zoneId01 = reader.ReadLEInt32(); //zoneId D
                    var tb01 = reader.ReadByte(); //tb A
                    var revision01 = reader.ReadLEInt64(); //revision Q, The resource version is the same as the brackets in the client header. (r.321543)

                    //пропускаем недо X2EnterWorld
                    //if (type == 0)
                    {
                        Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv => kv.Value.Session == cookie01 && kv.Value.AccountId == accountId01).Value;
                        if (m_Authorized == null)
                        {
                            net.Dispose();
                            Logger.Trace("Account ID: {0} not logged in, can not continue", net);
                        }
                        else
                        {
                            net.CurrentAccount = m_Authorized;
                            Logger.Trace("Account ID: {0} logged in, continue...", net);
                            net.SendAsync(new NP_X2EnterWorldResponsePacket01_0x0000());
                            //DD02 A>S
                            net.SendAsync(new NP_ChangeState_0x0000(-1)); //начальный пакет NP_ChangeState с параметром 0
                        }
                    }
                    break;
                case "3":
                    //v.3.0.3.0
                    /*
                    [1]             A>s             0ms.            19:48:01 .455      25.07.18
                    -------------------------------------------------------------------------------
                     TType: ArcheageServer: GS1     Parse: 6           EnCode: off         
                    ------- 0  1  2  3  4  5  6  7 -  8  9  A  B  A  D  E  F    -------------------
                    000000 28 00 00 01 00 00 00 00 | 6D 05 00 00 6D 05 00 00     (.......m...m...
                    000010 1A C7 00 00 00 00 00 00 | 28 10 B4 7A FF FF FF FF     .З......(.ґzяяяя
                    000020 00 F3 0C 05 00 00 00 00 | 00 00                       .у........
                    -------------------------------------------------------------------------------
                    Archeage: "X2EnterWorld"                     size: 42     prot: 2  $002
                    Addr:  Size:    Type:         Description:     Value:
                    0000     2   word          psize             40         | $0028
                    0002     2   word          type              256        | $0100
                    0004     2   word          ID                0          | $0000
                    0006     2   word          type              0          | $0000
                    0008     4   integer       p_from            1389       | $0000056D
                    000C     4   integer       p_to              1389       | $0000056D
                    0010     8   int64         accountId         50970      | $0000C71A
                    0018     4   integer       cookie            2058620968 | $7AB41028
                    001C     4   integer       zoneId            -1         | $FFFFFFFF
                    0020     2   word          tb                62208      | $F300
                    0022     4   integer       revision          1292       | $0000050C
                    0026     4   integer       index             0          | $00000000

                    Recv: 2800 0001 0000 0000 6D050000 6D050000 1AC7000000000000 2810B47A FFFFFFFF 00F3 0C050000 00000000
                     */
                    //type и ID нет в теле пакета (забрано ранее)
                    //reader.Offset += 2; //Undefined Random Byte
                    var type = reader.ReadLEInt16(); //type
                    var pFrom = reader.ReadLEInt32(); //p_from
                    var pTo = reader.ReadLEInt32(); //p_to
                    var accountId = reader.ReadLEInt64(); //Account Id
                    var cookie = reader.ReadLEInt32(); //cookie
                    var zoneId = reader.ReadLEInt32(); //zoneId
                    var tb = reader.ReadLEInt16(); //tb
                    var revision = reader.ReadLEInt32(); //revision, The resource version is the same as the brackets in the client header. (r.321543)
                    var index = reader.ReadLEInt32(); //index

                    //пропускаем недо X2EnterWorld
                    //if (type == 0)
                    {
                        Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv => kv.Value.Session == cookie && kv.Value.AccountId == accountId).Value;
                        if (m_Authorized == null)
                        {
                            net.Dispose();
                            Logger.Trace("Account ID: {0} not logged in, can not continue", net);
                        }
                        else
                        {
                            net.CurrentAccount = m_Authorized;
                            //нулевой пакет DD05 S>A
                            //0x01 0x0000_X2EnterWorldPacket
                            //net.SendAsyncHex(new NP_X2EnterWorldResponsePacket());
                            net.SendAsync(new NP_X2EnterWorldResponsePacket_0x0000(clientVersion));
                            //DD02 A>S
                            net.SendAsync(new NP_ChangeState_0x0000(-1)); //начальный пакет NP_ChangeState с параметром 0
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static void OnPacketReceive_Client001F(ClientConnection net, PacketReader reader)
        {
            //SC AiDebugPacket
            net.SendAsyncHex(new NP_Hex("1C00DD01B8010500000001000000000000000000000073CF565300000000"));
            //SC UnitVisualOptionsPacket
            net.SendAsyncHex(new NP_Hex("4000DD01C001FF091A000B004A757374746F636865636B10006368617261637465725F6F7074696F6E1300675F686964655F7475746F7269616C20310D0A14000000"));
            //SC UnitVisualOptionsPacket
            net.SendAsyncHex(new NP_Hex("3B0EDD01C001FF091A000B004A757374746F636865636B0B006B65795F62696E64696E67130E7072696D6172790D0A202020206275696C74696E0D0A2020202020202020657363617065206F70656E5F636F6E6669670D0A20202020202020204354524C2D53484946542D3120676F646D6F64650D0A20202020202020204354524C2D53484946542D3220666C796D6F64650D0A2020202020202020746162206379636C655F686F7374696C655F666F72776172640D0A20202020202020204354524C2D746162206379636C655F667269656E646C795F666F72776172640D0A202020202020202053484946542D746162206379636C655F686F7374696C655F6261636B776172640D0A20202020202020204354524C2D53484946542D746162206379636C655F667269656E646C795F6261636B776172640D0A202020202020202071206D6F76656C6566740D0A20202020202020204354524C2D7120746F67676C655F73686F775F67756964655F646563616C0D0A202020202020202077206D6F7665666F72776172640D0A202020202020202065206D6F766572696768740D0A20202020202020204354524C2D6520746F67676C655F706F73740D0A20202020202020204354524C2D7220746F67676C655F726169645F6672616D650D0A202020202020202053484946542D7220746F67676C655F726169645F7465616D5F6D616E616765720D0A20202020202020204354524C2D53484946542D72207265706C795F6C6173745F776869737065720D0A2020202020202020414C542D53484946542D72207265706C795F6C6173745F7768697370657265640D0A202020202020202053484946542D7420746F67676C655F65787065646974696F6E5F6D616E6167656D656E740D0A20202020202020206920746F67676C655F6261670D0A20202020202020204354524C2D6920746F67676C655F676D5F636F6E736F6C650D0A20202020202020206F20746F67676C655F63726166745F626F6F6B0D0A20202020202020207020746F67676C655F636F6D6D6F6E5F6661726D5F696E666F0D0A2020202020202020656E746572206F70656E5F636861740D0A202020202020202061207475726E6C6566740D0A202020202020202073206D6F76656261636B0D0A202020202020202064207475726E72696768740D0A20202020202020206620646F5F696E746572616374696F6E5F310D0A20202020202020204354524C2D6620746F67676C655F666F7263655F61747461636B0D0A20202020202020206720646F5F696E746572616374696F6E5F320D0A20202020202020206820646F5F696E746572616374696F6E5F330D0A20202020202020206A20646F5F696E746572616374696F6E5F340D0A20202020202020206B20746F67676C655F7370656C6C626F6F6B0D0A20202020202020206C20746F67676C655F71756573740D0A20202020202020207A2061637469766174655F776561706F6E0D0A20202020202020207820646F776E0D0A20202020202020206320746F67676C655F6368617261637465720D0A20202020202020204354524C2D7620746F67676C655F6E616D657461670D0A202020202020202053484946542D7620746F67676C655F667269656E640D0A202020202020202053484946542D6220746F67676C655F636F6D6D65726369616C5F6D61696C0D0A20202020202020206E20746F67676C655F696E67616D6573686F700D0A20202020202020206D20746F67676C655F776F726C646D61700D0A202020202020202053484946542D6D20746F67676C655F63726166745F72657365617263680D0A20202020202020202E20746F67676C655F77616C6B0D0A20202020202020207370616365206A756D700D0A2020202020202020663120726F756E645F7461726765740D0A2020202020202020414C542D663520646F665F626F6B65685F636972636C650D0A2020202020202020414C542D663620646F665F626F6B65685F68657861676F6E0D0A2020202020202020414C542D663720646F665F626F6B65685F68656172740D0A2020202020202020414C542D663820646F665F626F6B65685F737461720D0A20202020202020206E756D6C6F636B206175746F72756E0D0A20202020202020206631322073637265656E73686F746D6F64650D0A20202020202020204354524C2D6631322073637265656E73686F7463616D6572610D0A2020202020202020686F6D652072696768745F63616D6572610D0A2020202020202020414C542D686F6D6520646F665F626F6B65685F6164645F73697A650D0A20202020202020204354524C2D686F6D6520646F665F6164645F646973740D0A202020202020202053484946542D757020616374696F6E5F6261725F706167655F707265760D0A2020202020202020706167657570206379636C655F63616D6572615F636F756E7465725F636C6F636B776973650D0A2020202020202020414C542D70616765757020646F665F626F6B65685F6164645F696E74656E736974790D0A20202020202020204354524C2D70616765757020646F665F6164645F72616E67650D0A2020202020202020656E64206261636B5F63616D6572610D0A2020202020202020414C542D656E6420646F665F626F6B65685F7375625F73697A650D0A20202020202020204354524C2D656E6420646F665F7375625F646973740D0A202020202020202053484946542D646F776E20616374696F6E5F6261725F706167655F6E6578740D0A202020202020202070616765646F776E206379636C655F63616D6572615F636C6F636B776973650D0A2020202020202020414C542D70616765646F776E20646F665F626F6B65685F7375625F696E74656E736974790D0A20202020202020204354524C2D70616765646F776E20646F665F7375625F72616E67650D0A2020202020202020696E736572742066726F6E745F63616D6572610D0A2020202020202020414C542D696E7365727420646F665F626F6B65685F746F67676C650D0A20202020202020204354524C2D696E7365727420646F665F746F67676C650D0A202020202020202064656C657465206C6566745F63616D6572610D0A20202020202020204354524C2D64656C65746520646F665F6175746F5F666F6375730D0A2020202020202020776865656C75702073637265656E73686F745F7A6F6F6D5F696E0D0A20202020202020204354524C2D776865656C7570206275696C6465725F7A6F6F6D5F696E0D0A202020202020202053484946542D776865656C7570206275696C6465725F726F746174655F6C6566745F6C617267650D0A2020202020202020414C542D776865656C7570206275696C6465725F726F746174655F6C6566745F736D616C6C0D0A2020202020202020776865656C646F776E2073637265656E73686F745F7A6F6F6D5F6F75740D0A20202020202020204354524C2D776865656C646F776E206275696C6465725F7A6F6F6D5F6F75740D0A202020202020202053484946542D776865656C646F776E206275696C6465725F726F746174655F72696768745F6C617267650D0A2020202020202020414C542D776865656C646F776E206275696C6465725F726F746174655F72696768745F736D616C6C0D0A20202020202020206D6F757365647820726F746174657961770D0A20202020202020206D6F757365647920726F7461746570697463680D0A20202020202020206D6F757365647A207A6F6F6D696E670D0A202020206275696C74696E5F6D756C74690D0A2020202020202020616374696F6E5F6261725F627574746F6E0D0A2020202020202020202020203120310D0A20202020202020202020202053484946542D312031330D0A2020202020202020202020203220320D0A20202020202020202020202053484946542D322031340D0A2020202020202020202020203320330D0A20202020202020202020202053484946542D332031350D0A2020202020202020202020203420340D0A20202020202020202020202053484946542D342031360D0A2020202020202020202020203520350D0A20202020202020202020202053484946542D352031370D0A2020202020202020202020203620360D0A20202020202020202020202053484946542D362031380D0A2020202020202020202020203720370D0A20202020202020202020202053484946542D372031390D0A2020202020202020202020203820380D0A20202020202020202020202053484946542D382032300D0A2020202020202020202020203920390D0A20202020202020202020202053484946542D392032310D0A202020202020202020202020302031300D0A20202020202020202020202053484946542D302032320D0A2020202020202020202020206D696E75732031310D0A20202020202020202020202053484946542D6D696E75732032330D0A202020202020202020202020657175616C732031320D0A20202020202020202020202053484946542D657175616C732032340D0A20202020202020206D6F64655F616374696F6E5F6261725F627574746F6E2028207220312C207420322C207920332C2075203420290D0A20202020202020207465616D5F746172676574202820663220312C20663320322C20663420332C206635203420290D0A20202020202020206F7665725F686561645F6D61726B6572202820663620312C20663720322C206638203320290D0A7365636F6E640D0A202020206275696C74696E0D0A20202020202020204354524C2D414C542D663120676F646D6F64650D0A2020202020202020414C542D53484946542D663120666C796D6F64650D0A20202020202020207570206D6F7665666F72776172640D0A20202020202020206C656674207475726E6C6566740D0A20202020202020207269676874207475726E72696768740D0A2020202020202020646F776E206D6F76656261636B0D0A20202020202020206D6F75736534206175746F72756E0D0A72656D6F7665640D0A140E0000"));
            //SC SCRaceCongestionPacket
            net.SendAsyncHex(new NP_Hex("0D00DD013A00000000000000000000"));
            //SC CharacterListPacket
            net.SendAsyncHex(new NP_Hex("8104DD0139000101FF091A000B004A757374746F636865636B010203C4010000CE010000B3000000650000000000000000000000000000000000000000005B5B00004618C1000000000000000100000001000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005C5B00004718C1000000000000000100000001000000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005E5B00004818C1000000000000000100000001000000002300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D61700004918C1000000000000000100000001000000009100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B0000000000000000000000000000000000000000EF1700004A18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000003A1800004B18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000007F4D0000D85D00000000000000000000000000001B020000000000000000000000000000070B0B00000000A8B7CF03000000006090A603EFFC104303BE1000000400000000000000000000000000803F0000803F0000000000000000000000000000803FCF0100000000803FA60000000000803F000000008FC2353F0000000000000000000000000000803FE37B8BFFAFECEFFFAFECEFFF000000FF00000000800000EF00EF00EE0017D40000000000001000000000000000063BB900D800EE00D400281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000036007F422F530000000000000C302B5300000000000000000C302B530000000000000000B4412F5300000000C200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001E00000000000000000000000000000000000000000000000000000000000000000000000000000000B33F2F5300000000"));
        }
        public static void CSRefreshInCharacterList(ClientConnection net, PacketReader reader)
        {
            //SC ResultRestrictCheck
            net.SendAsyncHex(new NP_Hex("0D00DD01C301000000000000000000"));
        }

        public static void SCMailReceiverOpened(ClientConnection net, PacketReader reader)
        {
            //SC ICSMenuList
            net.SendAsyncHex(new NP_Hex("0A00DD01C401FF091A000100"));
        }

        public static void SelectCharacter(ClientConnection net, PacketReader reader)
        {
            //SC MileageChangedPacket
            //net.SendAsyncHex(new NP_Hex("08000001FB00FF091A00"));
            //SC CharacterStatePacket
            net.SendAsyncHex(new NP_Hex("4205DD013B000000000010000E4FC3755AE17949B1F626620F354A9300000000FF091A000B004A757374746F636865636B010203C4010000CE010000B3000000650000000000000000000000000000000000000000005B5B00004618C1000000000000000100000001000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005C5B00004718C1000000000000000100000001000000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005E5B00004818C1000000000000000100000001000000002300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D61700004918C1000000000000000100000001000000009100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B0000000000000000000000000000000000000000EF1700004A18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000003A1800004B18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000007F4D0000D85D00000000000000000000000000001B020000000000000000000000000000070B0B00000000A8B7CF03000000006090A603EFFC104303BE1000000400000000000000000000000000803F0000803F0000000000000000000000000000803FCF0100000000803FA60000000000803F000000008FC2353F0000000000000000000000000000803FE37B8BFFAFECEFFFAFECEFFF000000FF00000000800000EF00EF00EE0017D40000000000001000000000000000063BB900D800EE00D400281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000036007F422F530000000000000C302B5300000000000000000C302B530000000000000000B4412F5300000000C200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001E00000000000000000000000000000000000000000000000000000000000000000000000000000000B33F2F53000000000000000000000000DBFB17C092070000000000000000000056010000460000000000000000000000000000000000000000000000000000000000000092070000000000000000000000000000000000000000000000000000323200C200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C72C000022A21C530000000000"));
            //SC CharacterInvenInitPacket
            net.SendAsyncHex(new NP_Hex("0C00DD0146003200000032000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("1F02DD014700020500CD0F00004C18C1000000000000010100000000000000000000000000000000000000000B00000000000000000000000000000000674900004D18C1000000000000010100000000000000000000000000000000000000000B00000000000000000000000000000000684900004E18C1000000000000010300000000000000000000000000000000000000000B00000000000000000000000000000000DA0F0000628354010000000000010300000000B8282F530000000000000000000000000B00000000000000000000000000000000561F000086A155010000000000000600000000F13C2F530000000000000000000000000B00000000000000000000000000000000481F000087A155010000000000000400000000F13C2F530000000000000000000000000B00000000000000000000000000000000530D0000F5A155010000000000000300000000F83C2F530000000000000000000000000B0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("CF00DD0147000205050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("7F00DD01470002030A000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("CF00DD0147000305000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("CF00DD0147000305050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC CharacterInvenContentsPacket
            net.SendAsyncHex(new NP_Hex("7F00DD01470003030A000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
            //SC GamePointChangedPacket
            net.SendAsyncHex(new NP_Hex("2C00DD01800100000000100000000000000000000000000000000000000000000000000000000000000010000000"));
            //SC CharacterReturnDistrictsPacket
            net.SendAsyncHex(new NP_Hex("7900DD015700020000006B6008002900D09ED0BAD180D0B5D181D182D0BDD0BED181D182D0B820D0A5D0BED183D0BFD184D0BED180D0B4D0B0B3000000C40F9B44BD650145F47DFC428B6C3940317508001000D0A5D0BED183D0BFD184D0BED180D0B4B30000002D73A54478DBE54479E9F54251A00B406B600800"));
        }

        public static void SpawnCharacter(ClientConnection net, PacketReader reader)
        {
            //SC FinishState
            net.SendAsyncHex(new NP_Hex("08000002010006000000"));
            //SC ChangeState
            net.SendAsyncHex(new NP_Hex("0800DD02000007000000"));
            //SCCSQuestAcceptConditionalPacket
            net.SendAsyncHex(new NP_Hex("5700DD014201B6EA050100000000FB0000000303000000000000000000000000000000000000000000000000000000000000000000FFFFFFFF0000000000000000000000000000000000000000000000000000000000000000"));
            //SCCSQuestAcceptConditionalPacket
            net.SendAsyncHex(new NP_Hex("5700DD014201F6EA050100000000E40900000300000000000000000000000000000000000000000000000000000000000000000000FFFFFFFF0000000000000000000000000000000000000000000000000000000000000000"));
            //SCUnitStatePacket
            net.SendAsyncHex(new NP_Hex("B504DD016400F527000B004A757374746F636865636B00FF091A0000000000000000000000F5F6790CD27499BC030000803F030B00000000000000000000005B5B00004618C1000000000000000100000001000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005C5B00004718C1000000000000000100000001000000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005E5B00004818C1000000000000000100000001000000002300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D61700004918C1000000000000000100000001000000009100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B0000000000000000000000000000000000000000EF1700004A18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000003A1800004B18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000007F4D0000D85D00000000000000000000000000001B02000000000000000000000000000003BE1000000400000000000000000000000000803F0000803F0000000000000000000000000000803FCF0100000000803FA60000000000803F000000008FC2353F0000000000000000000000000000803FE37B8BFFAFECEFFFAFECEFFF000000FF00000000800000EF00EF00EE0017D40000000000001000000000000000063BB900D800EE00D400281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000090B0000078B40000FFFF00000002AB29000001002A000001000000000000D02100000000000065000000000000000000000000FF00000000FF00000000FF00000000FF00000000FF00000000FF920700000000000000FF00000000FF00000000FF01070000001E3C320028640101010000000001020000007709000000F5270000000000010100204E0000000000000000000000000000010000000000000000000000000101000000B119000000F527000000000001010000000000000000000000000000000000010000000000000000000000"));
            //SCCooldownsPacket
            net.SendAsyncHex(new NP_Hex("0C00DD0145000000000000000000"));
            //SC DetailedTimeOfDayPacket
            net.SendAsyncHex(new NP_Hex("1400DD01EA004871B241D171DA3A000000000000C041"));
        }
        public static void NotifyInGame(ClientConnection net, PacketReader reader)
        {
            //SCChatMessagePacket
            net.SendAsyncHex(new NP_Hex("2D00DD01C600FEFF000000000000000000000000000000000000000000"));
            //SC ChangeState
            net.SendAsyncHex(new NP_Hex("080057656C636F6D65210000000000000000"));
        }

        public static void OnPacketReceive_CSListCharacter_0x001F(ClientConnection net, PacketReader reader)
        {
            //SC AiDebugPacket
            net.SendAsyncHex(new NP_Hex("1C00DD01B8010500000001000000000000000000000073CF565300000000"));
            //SC UnitVisualOptionsPacket
            net.SendAsyncHex(new NP_Hex("4000DD01C001FF091A000B004A757374746F636865636B10006368617261637465725F6F7074696F6E1300675F686964655F7475746F7269616C20310D0A14000000"));
            //SC UnitVisualOptionsPacket
            net.SendAsyncHex(new NP_Hex("3B0EDD01C001FF091A000B004A757374746F636865636B0B006B65795F62696E64696E67130E7072696D6172790D0A202020206275696C74696E0D0A2020202020202020657363617065206F70656E5F636F6E6669670D0A20202020202020204354524C2D53484946542D3120676F646D6F64650D0A20202020202020204354524C2D53484946542D3220666C796D6F64650D0A2020202020202020746162206379636C655F686F7374696C655F666F72776172640D0A20202020202020204354524C2D746162206379636C655F667269656E646C795F666F72776172640D0A202020202020202053484946542D746162206379636C655F686F7374696C655F6261636B776172640D0A20202020202020204354524C2D53484946542D746162206379636C655F667269656E646C795F6261636B776172640D0A202020202020202071206D6F76656C6566740D0A20202020202020204354524C2D7120746F67676C655F73686F775F67756964655F646563616C0D0A202020202020202077206D6F7665666F72776172640D0A202020202020202065206D6F766572696768740D0A20202020202020204354524C2D6520746F67676C655F706F73740D0A20202020202020204354524C2D7220746F67676C655F726169645F6672616D650D0A202020202020202053484946542D7220746F67676C655F726169645F7465616D5F6D616E616765720D0A20202020202020204354524C2D53484946542D72207265706C795F6C6173745F776869737065720D0A2020202020202020414C542D53484946542D72207265706C795F6C6173745F7768697370657265640D0A202020202020202053484946542D7420746F67676C655F65787065646974696F6E5F6D616E6167656D656E740D0A20202020202020206920746F67676C655F6261670D0A20202020202020204354524C2D6920746F67676C655F676D5F636F6E736F6C650D0A20202020202020206F20746F67676C655F63726166745F626F6F6B0D0A20202020202020207020746F67676C655F636F6D6D6F6E5F6661726D5F696E666F0D0A2020202020202020656E746572206F70656E5F636861740D0A202020202020202061207475726E6C6566740D0A202020202020202073206D6F76656261636B0D0A202020202020202064207475726E72696768740D0A20202020202020206620646F5F696E746572616374696F6E5F310D0A20202020202020204354524C2D6620746F67676C655F666F7263655F61747461636B0D0A20202020202020206720646F5F696E746572616374696F6E5F320D0A20202020202020206820646F5F696E746572616374696F6E5F330D0A20202020202020206A20646F5F696E746572616374696F6E5F340D0A20202020202020206B20746F67676C655F7370656C6C626F6F6B0D0A20202020202020206C20746F67676C655F71756573740D0A20202020202020207A2061637469766174655F776561706F6E0D0A20202020202020207820646F776E0D0A20202020202020206320746F67676C655F6368617261637465720D0A20202020202020204354524C2D7620746F67676C655F6E616D657461670D0A202020202020202053484946542D7620746F67676C655F667269656E640D0A202020202020202053484946542D6220746F67676C655F636F6D6D65726369616C5F6D61696C0D0A20202020202020206E20746F67676C655F696E67616D6573686F700D0A20202020202020206D20746F67676C655F776F726C646D61700D0A202020202020202053484946542D6D20746F67676C655F63726166745F72657365617263680D0A20202020202020202E20746F67676C655F77616C6B0D0A20202020202020207370616365206A756D700D0A2020202020202020663120726F756E645F7461726765740D0A2020202020202020414C542D663520646F665F626F6B65685F636972636C650D0A2020202020202020414C542D663620646F665F626F6B65685F68657861676F6E0D0A2020202020202020414C542D663720646F665F626F6B65685F68656172740D0A2020202020202020414C542D663820646F665F626F6B65685F737461720D0A20202020202020206E756D6C6F636B206175746F72756E0D0A20202020202020206631322073637265656E73686F746D6F64650D0A20202020202020204354524C2D6631322073637265656E73686F7463616D6572610D0A2020202020202020686F6D652072696768745F63616D6572610D0A2020202020202020414C542D686F6D6520646F665F626F6B65685F6164645F73697A650D0A20202020202020204354524C2D686F6D6520646F665F6164645F646973740D0A202020202020202053484946542D757020616374696F6E5F6261725F706167655F707265760D0A2020202020202020706167657570206379636C655F63616D6572615F636F756E7465725F636C6F636B776973650D0A2020202020202020414C542D70616765757020646F665F626F6B65685F6164645F696E74656E736974790D0A20202020202020204354524C2D70616765757020646F665F6164645F72616E67650D0A2020202020202020656E64206261636B5F63616D6572610D0A2020202020202020414C542D656E6420646F665F626F6B65685F7375625F73697A650D0A20202020202020204354524C2D656E6420646F665F7375625F646973740D0A202020202020202053484946542D646F776E20616374696F6E5F6261725F706167655F6E6578740D0A202020202020202070616765646F776E206379636C655F63616D6572615F636C6F636B776973650D0A2020202020202020414C542D70616765646F776E20646F665F626F6B65685F7375625F696E74656E736974790D0A20202020202020204354524C2D70616765646F776E20646F665F7375625F72616E67650D0A2020202020202020696E736572742066726F6E745F63616D6572610D0A2020202020202020414C542D696E7365727420646F665F626F6B65685F746F67676C650D0A20202020202020204354524C2D696E7365727420646F665F746F67676C650D0A202020202020202064656C657465206C6566745F63616D6572610D0A20202020202020204354524C2D64656C65746520646F665F6175746F5F666F6375730D0A2020202020202020776865656C75702073637265656E73686F745F7A6F6F6D5F696E0D0A20202020202020204354524C2D776865656C7570206275696C6465725F7A6F6F6D5F696E0D0A202020202020202053484946542D776865656C7570206275696C6465725F726F746174655F6C6566745F6C617267650D0A2020202020202020414C542D776865656C7570206275696C6465725F726F746174655F6C6566745F736D616C6C0D0A2020202020202020776865656C646F776E2073637265656E73686F745F7A6F6F6D5F6F75740D0A20202020202020204354524C2D776865656C646F776E206275696C6465725F7A6F6F6D5F6F75740D0A202020202020202053484946542D776865656C646F776E206275696C6465725F726F746174655F72696768745F6C617267650D0A2020202020202020414C542D776865656C646F776E206275696C6465725F726F746174655F72696768745F736D616C6C0D0A20202020202020206D6F757365647820726F746174657961770D0A20202020202020206D6F757365647920726F7461746570697463680D0A20202020202020206D6F757365647A207A6F6F6D696E670D0A202020206275696C74696E5F6D756C74690D0A2020202020202020616374696F6E5F6261725F627574746F6E0D0A2020202020202020202020203120310D0A20202020202020202020202053484946542D312031330D0A2020202020202020202020203220320D0A20202020202020202020202053484946542D322031340D0A2020202020202020202020203320330D0A20202020202020202020202053484946542D332031350D0A2020202020202020202020203420340D0A20202020202020202020202053484946542D342031360D0A2020202020202020202020203520350D0A20202020202020202020202053484946542D352031370D0A2020202020202020202020203620360D0A20202020202020202020202053484946542D362031380D0A2020202020202020202020203720370D0A20202020202020202020202053484946542D372031390D0A2020202020202020202020203820380D0A20202020202020202020202053484946542D382032300D0A2020202020202020202020203920390D0A20202020202020202020202053484946542D392032310D0A202020202020202020202020302031300D0A20202020202020202020202053484946542D302032320D0A2020202020202020202020206D696E75732031310D0A20202020202020202020202053484946542D6D696E75732032330D0A202020202020202020202020657175616C732031320D0A20202020202020202020202053484946542D657175616C732032340D0A20202020202020206D6F64655F616374696F6E5F6261725F627574746F6E2028207220312C207420322C207920332C2075203420290D0A20202020202020207465616D5F746172676574202820663220312C20663320322C20663420332C206635203420290D0A20202020202020206F7665725F686561645F6D61726B6572202820663620312C20663720322C206638203320290D0A7365636F6E640D0A202020206275696C74696E0D0A20202020202020204354524C2D414C542D663120676F646D6F64650D0A2020202020202020414C542D53484946542D663120666C796D6F64650D0A20202020202020207570206D6F7665666F72776172640D0A20202020202020206C656674207475726E6C6566740D0A20202020202020207269676874207475726E72696768740D0A2020202020202020646F776E206D6F76656261636B0D0A20202020202020206D6F75736534206175746F72756E0D0A72656D6F7665640D0A140E0000"));
            //SC SCRaceCongestionPacket
            net.SendAsyncHex(new NP_Hex("0D00DD013A00000000000000000000"));
            //SC CharacterListPacket
            net.SendAsyncHex(new NP_Hex("8104DD0139000101FF091A000B004A757374746F636865636B010203C4010000CE010000B3000000650000000000000000000000000000000000000000005B5B00004618C1000000000000000100000001000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005C5B00004718C1000000000000000100000001000000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B00000000000000000000000000000000000000005E5B00004818C1000000000000000100000001000000002300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D61700004918C1000000000000000100000001000000009100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B0000000000000000000000000000000000000000EF1700004A18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000003A1800004B18C1000000000000000100000001000000008200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B000000000000000000000000000000007F4D0000D85D00000000000000000000000000001B020000000000000000000000000000070B0B00000000A8B7CF03000000006090A603EFFC104303BE1000000400000000000000000000000000803F0000803F0000000000000000000000000000803FCF0100000000803FA60000000000803F000000008FC2353F0000000000000000000000000000803FE37B8BFFAFECEFFFAFECEFFF000000FF00000000800000EF00EF00EE0017D40000000000001000000000000000063BB900D800EE00D400281BEBE100E700F037230000000000640000000000000064000000F0000000000000002BD50000006400000000F9000000E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000036007F422F530000000000000C302B5300000000000000000C302B530000000000000000B4412F5300000000C200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001E00000000000000000000000000000000000000000000000000000000000000000000000000000000B33F2F5300000000"));
            //net.SendAsync(new NP_CharacterListPacket_0x0039(net)); //шлем список героев
        }


        public static void OnPacketReceive_FinishState0201(string clientVersion, ClientConnection net, PacketReader reader)
        {
            int state = reader.ReadLEInt32(); //считываем state
            //--------------------------------------------------------------------------
            //NP_ChangeState
            net.SendAsync(new NP_ChangeState_0x0000(state));
            //--------------------------------------------------------------------------
            switch (clientVersion)
            {
                case "1":
                    if (state == 0)
                    {
                        //--------------------------------------------------------------------------
                        //SetGameType
                        net.SendAsync(new NP_SetGameType_0x000F());
                        //--------------------------------------------------------------------------
                        //SC InitialConfigPacket
                        net.SendAsyncHex(new NP_Hex("2B00DD0105000A0061612E6D61696C2E727507003E320F0F79003300000000000A003200000000000101010101"));
                        //SC AiDebugPacket
                        //net.SendAsyncHex(new NP_Hex("1C00DD01B8010100000001000000000000000000000073CF565300000000"));
                        //SC ChatSpamDelayPacket
                        net.SendAsyncHex(new NP_Hex("1400DD01CB0000000000000000000000000000000000"));
                        //SC HackGuardRetAddrsRequestPacket на этом пакете ошибка "PACKET ERROR"
                        //net.SendAsyncHex(new NP_Hex("9401DD01BB018C018C01981A261C1A1F56488A1BA345EC6A44241BD974F1E6251D843BC6CBFB859F2184FCFE21DE86AF147BA3231D89C80A1E4CCAAF812BE3E7218E9C78AF971FECF1836D7020442A9E77A239A73F36BF047C69872E858CEAC5A553230E54CEE715C7A4C35F6FC38AB3118BB8B933E9176B568D5184D03076A96FE7F4E9216B0A7247B2D2E344A8CF14FA84464E798ABB5D493EBBF1E02E59D5F1B505FB03E6D486151A9B138FC33CE4F3BA0F9391984E8F6FAC91C651FB36033B742EB6E30D990A36B0EAF44195F3E1A06FEE852B15A8F612ED119F0B034EAF17164E79AB7A13DC42F5343717B3FCE54ED73FCBAF87384E2ADD53F6E32E2AE55C5653DB7DB0E4C779AC64505D69D8A329CFFE41135E6042FDE221CA71705C5AE29BDAEE41D297FF681499EEB29A2B3870DF90454E6CBBE4210EA98C8E7382DCDE79F8C43255D58BC536AB1DAA3001C4C2D295BA258B44C518BE58054FE6AF1E45D200C36ADE77796D39FAC97F049473D24B6E0097AED630C68B5184D03076A96FE7F4E9216B0A7247B28E97C79D70263F4B0F1E2D3C"));
                        //SC CountUnreadMailPacket
                        //net.SendAsyncHex(new NP_Hex("9601000110018E018E01C3AA681672D83AE1C4137A0D6095E77DA725C55AC652482905DFF22172EDA9D00EADED6863D8D8E2AC33D5FDB12F6F695812D966777753BF54EEE65201CBA5316F2A1E7DF69FA7E7BFAF39B3F51E4A910C549CE8C9D76DC644334FFE7DFF5AFBD207FDF4C920678D64289FCD0A13BE0355FECC02D55B856331A9B838A643F420E894908975D29935CDFD2FC082848A56A8BD078BB75A130029656C19B183F4AF1BDBD9B8FE992B0BA9B9D815117341974073D0D9D398E4C948F209CCE56E1B98537C8BF7BAC9A431B40F92C10155E78BCE012386D3AC6CA097EFEB01ED09918CECED6CDE785D71BB29D41220A75E7C3FA485A30CFF790B97EFEDB4ABE9658AEEF979423B8889E1551A59D5C929172BDA3DDB653A68DE153FAE156DBCDC2C947035ED3990C25743D7DC8191239C403CB1CE54654B9F86C89181B03D242E043599166F46034B3458B55FEEEBB216BDF3ECCA44AA5B897686E12AE89645A25A1E1F3B57F9084D56BA4916C536F739E79BF706C1153FDF87F754A814AE3FDD4B2F4FF4DD118E69D7ACB9342ED7984269"));
                    }
                    break;
                case "3":
                    if (state == 0)
                    {
                        //выводим один раз
                        //пакет №1 DD05 S>A
                        net.SendAsync(new NP_Packet_0x0094()); //1400DD05C2E7E3865627F6CF97087265899FE9242175
                        //--------------------------------------------------------------------------
                        //SetGameType
                        net.SendAsync(new NP_SetGameType_0x000F());
                        //--------------------------------------------------------------------------
                        //пакеты для входа в Лобби
                        //пакет №2 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x0034()); //5000DD057F20F4C282625271B0CB11257381D3E43840DBA6F90B2C06A99043486EEFCD4B6745F31F35E70901D0441D85AB78EE825322F4C494643405D5A5754517E7B7875627F7C797704010E0B0115081B0
                        //пакет №3 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x02C3()); //A900DD0574936530FFE0A119356592C1B87D0D80A6E0105A6BBA8A10367483C1AB3C4882A3F1145673BEC8196E6492D9EE3F4690ACF7111C72A0CA052A138BC0F02457CEEABA044766BEC3172173C9D3F536418AFEC91E347486C3E0254B9DACBC16416ABCCD13257981C7AB25579CB3B905596BBBC2052472C8C0F5224398A0E2041E62A0C7162B66909CF3265197ACF5175128B6D710217F92C5AB314B98B0B911236489DFEF51E71747
                        //пакет №4 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x00EC()); //2A00DD05B9656B03D2A2724212E3B3835323F4C494643405B5F1754516E6B6865727F7C797704010E0B08151
                        //пакет №5 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x0281()); //7700DD05F997B53000EEA3724212E2B4845524F4C595643505D7A6764616E7B7875767BED0A0704011E1B1815122F2C292623303D3A3744414E4B4855525F5C596663606D6A7774717E7B0805020F0C191613101D2A2724212E3B3835323F4C49465340AC6A5754566A4B3865727F7C781348DDCAC8F8B99F2
                        //пакет №6 DD05 S>A
                        net.SendAsync(new NP_Packet_0x00BA()); //0900DD05BFB67E50104170
                        //пакет №7 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x018A()); //1C00DD054863A207D5AF754516E6B9895827F7C897704010E0B0815EC4F4
                        //пакет №8 DD05 S>A
                        net.SendAsync(new NP_Packet_0x01CC()); //0E00DD05842F7EC797704010E0B08151
                        //пакет №9 DD05 S>A
                        net.SendAsync(new NP_Packet_0x0030()); //0900DD052CB90D53114270
                        //пакет №10 DD05 S>A
                        net.SendAsync(new NP_Packet_0x01AF()); //0E00DD054E2DB8C597704010E0B08151
                        //пакет №11 DD05 S>A
                        net.SendAsync(
                            new NP_Packet_0x02CF()); //2300DD0500E82E825223F4C494643405D5A5754516E6B6865727F7C797704010E0B0815142
                        //пакет №12 DD05 S>A
                        net.SendAsync(new NP_Packet_0x029C()); //0A00DD05817C5812E0B08151
                    }
                    break;
                default:
                    break;
            }
        }

        public static void OnPacketReceive_ClientE4FB(ClientConnection net, PacketReader reader)
        {
            var number1 = reader.ReadLEInt32();
            //net.SendAsync(new NP_ChangeState(0));
        }

        public static void OnPacketReceive_Client0D7C(ClientConnection net, PacketReader reader)
        {
            var number1 = reader.ReadLEInt32();
            //var number2 = reader.ReadLEInt32();
            //var number3 = reader.ReadLEInt32();
            //var number4 = reader.ReadLEInt32();
            //var number5 = reader.ReadLEInt32();
            //net.SendAsync(new NP_ChangeState(0));
        }
        public static void OnPacketReceive_ClientE17B(ClientConnection net, PacketReader reader)
        {
            ////пакет для входа в Лобби - продолжение
            //пакет №13 DD05 S>A
            net.SendAsync(new NP_Packet_0x0272());   //0700DD050CBD7B5010
            //пакет №14 DD05 S>A
            net.SendAsync(new NP_Packet_0x00EC_2()); //2A00DD05EA6F6B03D3A2724213E3B3835323F4C4946434053B833B2816E6B6865727F7C797704010E0B08151
            //пакет №15 DD05 S>A
            net.SendAsync(new NP_Packet_0x008C());   //FE00DD0595F92296663707D7A7775020F0C090613101D1A1724212E2B2835323F3C494643404D5A5754515E6B6865626F7C7976737FED0A0704011E1B1815122F2C292623303D3A3734414E4B4845525F5C596663606D6A7774717E7C0906030FED1A1714111E2B2825222F3C393633304D4A4744415E5B5855526F6C696663707D7A7774010E0B0815121F1C192623202D2A3734313E3B4845424F4C595653505D6A6764616E7B7875727FED0A0704011E1B1815122F2C292623303D3A3744414E4B4855525F5C596663606D6A7774717E7B0805020F0C191613101D2A2724212E3B3835323F4C494643405D5A5754516E6B6865727F7C797704010E0B08151
            //пакет №16 DD05 S>A
            net.SendAsync(new NP_Packet_0x014D());   //0F00DD050637C9C697704010E0B0815186
            //var ii = GameServerController.AuthorizedAccounts.FirstOrDefault(n => n.Value.AccountId == net.CurrentAccount.AccountId);
            //список чаров
            if (net.CurrentAccount.Characters == 2)
            {
                //пакет №17 DD05 S>A
                net.SendAsync(new NP_CharacterListPacket_0x0079()); //0209DD051E05ACB68556F261C495603654B3CB183376E4B591B032F
                                                                //эти пакеты нужны когда есть чары в лобби
                                                                //пакет №18 DD05 S>A
                net.SendAsync(new NP_Packet_0x014F()); //2400DD0564F11F825223F4C495643405D55A754516E634B7D47DF7C797704010E0B081514272
                //пакет №19 DD05 S>A
                net.SendAsync(new NP_Packet_0x0145());   //1D00DD052777B6070231744517E6BD86214285B4FE1F2E30D1BD8B5DC4F423
                //пакет №20 DD05 S>A
                net.SendAsync(new NP_Packet_0x0145_2()); //1D00DD051C70B6070231744514E6BD86214285B4FE1F2E30D2BD8B5DC4F423
                //пакет №21 DD05 S>A
                net.SendAsync(new NP_Packet_0x0145_3()); //1D00DD050D71B6074342744517E6BD86214285B4FE1F2E30D1BD8B5DC4F423
                //пакет №22 DD05 S>A
                net.SendAsync(new NP_Packet_0x0145_4()); //1D00DD05FA72B6074342744514E6BD86214285B4FE1F2E30D2BD8B5DC4F423
            }
            else
            {
                //не забыть установить кол-во чаров в ArcheAgeLoginServer :: ArcheAgePackets.cs :: AcWorldList_0X08
                //пакет №17 DD05 S>A
                net.SendAsync(new NP_Packet_CharList_empty_0x0079()); //0800DD05FEA1C9531140
                //пакет №18 DD05 S>A
                net.SendAsync(new NP_Packet_0x014F()); //2400DD0564F11F825223F4C495643405D55A754516E634B7D47DF7C797704010E0B081514272
            }
            ///идет клиентский пакет 13000005393DB7A29CAA4C2365F02DB94C5B18BB50
            ///идет клиентский пакет 1300000539297EE205DC192D2A33B7071BC23B38BC
            ///идет клиентский пакет 1300000539B1D74AE4C48857E02BAB7E33AF496A8C
            ///идет клиентский пакет 1300000539211BA0D0AC0DE28974E1158F1BE5BB86

            //net.SendAsync(new NP_Packet_quit_0x00A5());
        }
        public static void OnPacketReceive_Client0438(ClientConnection net, PacketReader reader)
        {
            ///клиентский пакет на вход в мир 13000005 3804 2E8CFF98F0282A5A79DE98E9BE80B6
            ///зашифрован - не ловится
        }
        public static void OnPacketReceive_ReloginRequest_0x0088(ClientConnection net, PacketReader reader)
        {
            //клиентский пакет на релогин Recv: 13 00 00 05 34 0E 6F 39 8E 0A E3 5C E5 B9 85 25 D3 3E B3 8A 74
            net.SendAsync(new NP_Packet_quit_0x01F1()); //Good-Bye
            net.SendAsync(new NP_SCReconnectAuthPacket__0x01E5(net)); //
        }

        /// <summary>
        /// Получили клиентский пакет Ping, отвечаем серверным пакетом Pong
        /// </summary>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        public static void OnPacketReceive_Ping(ClientConnection net, PacketReader reader)
        {
            //Ping
            long tm = reader.ReadLEInt64(); //tm
            long when = reader.ReadLEInt64(); //when
            int local = reader.ReadLEInt32(); //local
            net.SendAsync(new NP_Pong_0x0013(tm, when, local));
        }

        /// <summary>
        /// Authenticate user login permissions I do not know how to use, discarded
        /// </summary>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        public static void OnPacketReceive_ClientAuthorized(ClientConnection net, PacketReader reader)
        {
            //B3 04 00 00 B3 04 00 00 8C 28 22 00 E7 F0 0C C6 FF FF FF FF 00 
            reader.Offset += 2;
            long protocol = reader.ReadLEInt64(); //Protocols?

            long accountId = reader.ReadLEInt64(); //Account Id
            reader.Offset += 4;
            int sessionId = reader.ReadLEInt32(); //User Session Id
            Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv => kv.Value.Session == sessionId && kv.Value.AccountId == accountId).Value;
            if (m_Authorized == null)
            {
                net.Dispose();
                Logger.Trace("Account ID: {0} is not logged in, unable to continue.", net);
            }
            else
            {
                net.CurrentAccount = m_Authorized;
                net.SendAsync(new NP_ClientConnected2());
                net.SendAsync(new NP_Client02());
                //net.SendAsync(new NP_ClientConnected());
            }
        }

        /// <summary>
        /// Connect game server first package
        /// </summary>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        public static void OnPacketReceive_Client01(ClientConnection net, PacketReader reader)
        {
            net.SendAsyncHex(new NP_Hex("0700dd05f2bdb150102a00dd056f6fcc01d3a2724213e3b3e05321512c00dd0205d012452606e6b6865727f7c797704010e0b081512c00dd021300157f26060000000060bee1d96c0100000000058ef05d96663707d219375020f0b62d01007dd3e50ffe00dd058ef95d96663707d7a7775020f0c090613101d1a1724212e2b2835323f3c494643404d5a5754515e6b6865626f7c7976737fed0a0704011e1b1815122f2c292623303d3a3734414e4b4845525f5c596663606d6a7774717e7c0906030fed1a1714111e2b2825222f3c393633304d4a4744415e5b5855526f6c696663707d7a7774010e0b0815121f1c192623202d2a3734313e3b4845424f4c595653505d6a6764616e7b7875727fed0a0704011e1b1815122f2c292623303d3a3744414e4b4855525f5c596663606d6a7774717e7b0805020f0c191613101d2a2724212e3b3835323f4c494643405d5a5754516e6b680b08151860800dd0520b181510f00dd0552379ac797704010e0b08151860800dd0520a188501140"));
        }
        public static void Onpacket0201(ClientConnection net, PacketReader reader)
        {
            byte b3 = reader.ReadByte();
            if (b3 == 0x0)
            {
                net.SendAsync(new NP_Client0200());//Also returns an error
                net.SendAsyncHex(new NP_Hex("1400dd05fee767865627f6cf97087265899fe9242175"));
                net.SendAsyncHex(new NP_Hex("1e00dd020f000f00735f7069726174655f69736c616e640000000000000000014d00dd05e1606b03c3a31536778cd1e4324092a4fb031865b9ca6f4768d0bf8f29288d0aa62032df76266a421005dc04e238f2c494643405d5a5754516e6b7875626f6c797704010e0b0815152f322a900dd05e1936731ffe0a119356592c1b87d0d80a6e0105a6bba8a10367483c1ab3c4882a3f1145673bec8196e6492d9ee3f4690acf7111c72a0ca052a138bc0f02457ceeaba044766bec3172173c9d3f536418afec91e347486c3e0254b9dacbc16416abccd13257981c7ab25579cb3b905596bbbc2052472c8c0f5224398a0e2041e62a0c7162b66909cf3265197acf5175128b6d710217f92c5ab314b98b0b911236489dfef51e717472a00dd050e65cc01d2a2724212e3b3835323f4c4946434053561754516e6b6865727f7c797704010e0b081517700dd057997103300eea3724212e2b4845524f4c595643505d7a6764616e7b7875767bed0a0704011e1b1815122f2c292623303d3a3744414e4b4855525f5c596663606d6a7774717e7b0805020f0c191613101d2a2724212e3b3835323f4c49465340ac6a5754566a4b3865727f7c781348ddcac8f8b99f20900dd05a9b6e5511041701c00dd05d0635d04d5af754516e6b9895827f7c897704010e0b0815ec4f40e00dd057a2fb0c797704010e0b081510900dd059db9ac531142700e00dd05282df0c797704010e0b081512300dd0523e85c835223f4c494643405d5a5754516e6b6865727f7c797704010e0b08151420a00dd058b7cf511e0b08151"));
            }
            else if (b3 == 0x01)
            {
                net.SendAsync(new NP_Client02002());
            }
            //net.SendAsync(new NP_Clientdd05bae9());
        }

        public static void Onpacket0212(ClientConnection net, PacketReader reader)
        {
            //reader.Offset += 8; //00 00 00 00 00 00 00 00  Undefined Data
            int number1 = reader.ReadLEInt32();
            int number2 = reader.ReadLEInt32();
            int number3 = reader.ReadLEInt32();
            int number4 = reader.ReadLEInt32();
            int number5 = reader.ReadLEInt32();
            net.SendAsync(new NP_Client0212(number1, number2, number3, number4, number5));
        }
        #endregion

        #region LOGIN<->GAME server Callbacks Implementation

        /// <summary>
        /// Логин сервер передал Гейм серверу пакет с информацией об подключаемом аккаунте
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        private static void Handle_AccountInfoReceived(string clientVersion, LoginConnection net, PacketReader reader)
        {
            switch (clientVersion)
            {
                case "1":
                    //Set Account Info
                    Account account = new Account
                    {
                        AccountId = reader.ReadLEInt32(),
                        Name = reader.ReadDynamicString(),
                        //Password = reader.ReadDynamicString(),
                        Token = reader.ReadDynamicString(),
                        AccessLevel = reader.ReadByte(),
                        Membership = reader.ReadByte(),
                        LastIp = reader.ReadDynamicString(),
                        LastEnteredTime = reader.ReadLEInt64(),
                        Characters = reader.ReadByte(),
                        Session = reader.ReadLEInt32()
                    };
                    Logger.Trace("Prepare login account ID: {0}, Session(cookie): {1}", account.AccountId, account.Session);
                    //Check if the account is online and force it to disconnect online
                    Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv =>
                        kv.Value.Session == account.Session && kv.Value.AccountId == account.AccountId).Value;
                    if (m_Authorized != null)
                    {
                        //Already
                        Account acc = ClientConnection.CurrentAccounts[m_Authorized.Session];
                        if (acc.Connection != null)
                        {
                            acc.Connection.Dispose(); //Disconenct  
                            Logger.Trace("Account Name: {0} log in twice, old connection is forcibly disconnected", acc.Name);
                        }
                        else
                        {
                            //Исправление входа второго пользователя, вторичный логин
                            ClientConnection.CurrentAccounts.Remove(m_Authorized.Session);
                            Logger.Trace("Account Name: {0} double connection is forcibly disconnected", acc.Name);
                            ClientConnection.CurrentAccounts.Add(account.Session, account);
                            Logger.Trace("Account Name: {0}, Session(cookie): {1} authorized", account.Name, account.Session);
                        }
                    }
                    else
                    {
                        ClientConnection.CurrentAccounts.Add(account.Session, account);
                        Logger.Trace("Account Name: {0}, Session(cookie): {1} authorized", account.Name, account.Session);
                    }
                    break;
                case "3":
                    /*
                        5400 0100
                        1AC7000000000000 61617465737400 616174657374616100 333165333466326237326439336262323564356632376265386139346334373800 01 01 3132372E302E302E3100 4329871565010000 02 2810B47A
                    */
                    //Set Account Info
                    account = new Account
                    {
                        AccountId = reader.ReadLEInt64(),
                        Name = reader.ReadDynamicString(),
                        //Password = reader.ReadDynamicString(),
                        Token = reader.ReadDynamicString(),
                        AccessLevel = reader.ReadByte(),
                        Membership = reader.ReadByte(),
                        LastIp = reader.ReadDynamicString(),
                        LastEnteredTime = reader.ReadLEInt64(),
                        Characters = reader.ReadByte(),
                        Session = reader.ReadLEInt32()
                    };
                    Logger.Trace("Prepare login account ID: {0}, Session(cookie): {1}", account.AccountId, account.Session);
                    //Check if the account is online and force it to disconnect online
                    m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv =>
                        kv.Value.Session == account.Session && kv.Value.AccountId == account.AccountId).Value;
                    if (m_Authorized != null)
                    {
                        //Already
                        Account acc = ClientConnection.CurrentAccounts[m_Authorized.Session];
                        if (acc.Connection != null)
                        {
                            acc.Connection.Dispose(); //Disconenct  
                            Logger.Trace("Account Name: {0} log in twice, old connection is forcibly disconnected",
                                acc.Name);
                        }
                        else
                        {
                            //Исправление входа второго пользователя, вторичный логин
                            ClientConnection.CurrentAccounts.Remove(m_Authorized.Session);
                            Logger.Trace("Account Name: {0} double connection is forcibly disconnected", acc.Name);
                            ClientConnection.CurrentAccounts.Add(account.Session, account);
                            Logger.Trace("Account Name: {0}, Session(cookie): {1} authorized", account.Name,
                                account.Session);
                        }
                    }
                    else
                    {
                        ClientConnection.CurrentAccounts.Add(account.Session, account);
                        Logger.Trace("Account Name: {0}, Session(cookie): {1} authorized", account.Name,
                            account.Session);
                    }
                    break;
                default:
                    break;
            }
        }

        private static void Handle_GameRegisterResult(LoginConnection con, PacketReader reader)
        {
            bool result = reader.ReadBoolean();
            if (result)
            {
                Logger.Trace("LoginServer successfully installed");
            }
            else
            {
                Logger.Trace("Some problems are appear while installing LoginServer");
            }

            if (result)
            {
                m_CurrentLoginServer = con;
            }
        }
        #endregion

        private static void Register(short opcode, OnPacketReceive<LoginConnection> e)
        {
            m_LHandlers[opcode] = new PacketHandler<LoginConnection>(opcode, e);
            m_Maintained++;
        }

        private static void Register(byte level, ushort opcode, OnPacketReceive<ClientConnection> e)
        {
            if (!levels.ContainsKey(level))
            {
                PacketHandler<ClientConnection>[] handlers = new PacketHandler<ClientConnection>[0xFFFF];
                handlers[opcode] = new PacketHandler<ClientConnection>(opcode, e);
                levels.Add(level, handlers);
            }
            else
            {
                levels[level][opcode] = new PacketHandler<ClientConnection>(opcode, e);
            }
        }
    }
}

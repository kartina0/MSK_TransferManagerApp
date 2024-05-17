// ----------------------------------------------
// Copyright © 2021 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using DL_CommonLibrary;
using ErrorCodeDefine;

namespace DL_PlcInterfce
{
    /// <summary>
    /// メモリ構造体
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct MemoryAssigne
    {
        [FieldOffset(0)]
        public int word;

        [FieldOffset(0)]
        public byte byte0;

        [FieldOffset(1)]
        public byte byte1;

        [FieldOffset(0)]
        public BitField bits;
    }

    /// <summary>
    /// Bit フィールド
    /// </summary>
    internal struct BitField
    {
        int word;
        public int this[int indexer]
        {
            get
            {
                int mask = (int)Math.Pow(2, indexer);
                return ((word & mask) > 0) ? 1 : 0;
            }
            set
            {
                int mask = (int)Math.Pow(2, indexer);
                if (value == 1)
                    word |= mask;
                else
                    word &= ~mask;
            }
        }
    }

    /// <summary>
    /// ダミーPLC
    /// </summary>
    public class Dummy_PLC : IPlc
    {
        /// <summary>
        /// デバイス数
        /// </summary>
        private const int PlcDeviceCount = 50000;

        #region Private Variable

        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PLC_IF";

        /// <summary>
        /// デバイス値保存ファイルパス
        /// </summary>
        private string _deviceFileName = "DeviceValue.csv";

        /// <summary>
        /// 接続パラメータ
        /// </summary>
        private string _connectionParam = "";

        /// <summary>
        /// 接続状態
        /// </summary>
        private bool _connected = false;

        /// <summary>
        /// 共有メモリ マップ
        /// </summary>
        private MemoryMappedFile[] _map = null;
        /// <summary>
        /// 共有メモリ ビュー
        /// </summary>
        private MemoryMappedViewAccessor[] _map_view = null;

        #endregion
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Dummy_PLC() { }

        /// <summary>
        /// 共有メモリ マップが作成されているのか確認する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsExistSharedMemory(string name)
        {
            bool exist = false;
            try
            {
                MemoryMappedFile map = MemoryMappedFile.OpenExisting(name);
                map.Dispose();
                exist = true;
            }
            catch { }
            return exist;
        }

        /// <summary>
        /// ダミーモードか確認
        /// </summary>
        /// <returns></returns>
        public bool IsDummy()
        {
            return true;
        }
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionParam">接続パラメータ</param>
        /// <returns></returns>
        public UInt32 Open(string connectionParam)
        {
            UInt32 rc = 0;

            _connectionParam = connectionParam;
            string[] param = connectionParam.Split(';');

            if (param.Length >= 2)
            {
                _deviceFileName = param[1];
            }
            // 共有メモリ作成
            MemoryAssigne mem = new MemoryAssigne();
            int size = (int)Marshal.SizeOf(mem) * PlcDeviceCount;

            // 既に共有メモリが作成されているのか確認する
            bool existMap = IsExistSharedMemory(_deviceFileName + "_SHARED_PLC_MEMORY0");
            if (_map == null)
            {
                _map = new MemoryMappedFile[(int)DeviceType.MaxCount];
                _map_view = new MemoryMappedViewAccessor[(int)DeviceType.MaxCount];
                for (int i = 0; i < (int)DeviceType.MaxCount; i++)
                {
                    _map[i] = MemoryMappedFile.CreateOrOpen(_deviceFileName + $"_SHARED_PLC_MEMORY{i}", size);
                    _map_view[i] = _map[i].CreateViewAccessor();
                }
            }

            // 初回共有マップ作成した場合、デバイス値を復元
            if (!existMap) ReadDeviceFile(_deviceFileName);
     
            _connected = STATUS_SUCCESS(rc);

            return rc;
        }
        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;

            _connected = false;
            // ファイルにデバイス値を保存
            WriteDeviceFile(_deviceFileName);

            // 共有メモリ破棄
            if (_map_view != null)
            {
                for (int i = 0; i < _map_view.Length; i++)
                    _map_view[i].Dispose();
            }
            if (_map != null)
            {
                for (int i = 0; i < _map.Length; i++)
                    _map[i].Dispose();
            }
           
            return rc;
        }

        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() 
        {
            return _connected;
        }

        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, ref int value)
        {
            UInt32 rc = 0;
            value = 0;

            if (_map_view != null)
            {
                MemoryAssigne mem = new MemoryAssigne();
                int offset = (int)Marshal.SizeOf(mem) * (int)addr;
                _map_view[(int)deviceType].Read<MemoryAssigne>(offset, out mem);
                value = (Int16)mem.word;
            }

            return rc;
        }

        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read32(DeviceType deviceType, UInt32 addr, ref Int32 value)
        {
            UInt32 rc = 0;
            value = 0;

            if (_map_view != null)
            {
                MemoryAssigne mem = new MemoryAssigne();
                int offset = (int)Marshal.SizeOf(mem) * (int)addr;
                _map_view[(int)deviceType].Read<MemoryAssigne>(offset, out mem);
                value = mem.word;
            }
            
            return rc;
        }

        /// <summary>
        /// 文字読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="len">最大文字数</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 len, ref string buf)
        {
            UInt32 rc = 0;

            buf = "";
          
            if (_map_view != null)
            {
                for (int i = 0; i < len; i++)
                {
                    MemoryAssigne mem = new MemoryAssigne();
                    int offset = (int)Marshal.SizeOf(mem) * (int)(addr + i);
                    _map_view[(int)deviceType].Read<MemoryAssigne>(offset, out mem);
                    //string asc1 = ASCIIEncoding.ASCII.GetString(new Byte[1] { mem.byte0 });
                    //string asc2 = ASCIIEncoding.ASCII.GetString(new Byte[1] { mem.byte1 });
                    string asc1 = Encoding.GetEncoding("Shift_JIS").GetString(new Byte[1] { mem.byte0 });
                    string asc2 = Encoding.GetEncoding("Shift_JIS").GetString(new Byte[1] { mem.byte1 });

                    if (mem.byte0 == 0) break;
                    buf += asc1;
                    if (mem.byte1 == 0) break;
                    buf += asc2;
                }
            }

            return rc;
        }

        /// <summary>
        /// 連続デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 count, ref Int32[] value)
        {
            UInt32 rc = 0;

            value = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (_map_view != null)
                {
                    MemoryAssigne mem = new MemoryAssigne();
                    int offset = (int)Marshal.SizeOf(mem) * (int)(addr + i);
                    _map_view[(int)deviceType].Read<MemoryAssigne>(offset, out mem);
                    value[i] = mem.word;
                }
            }

            return rc;
        }

        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, int value)
        {
            UInt32 rc = 0;

            if (_map_view != null)
            {
                MemoryAssigne mem = new MemoryAssigne();
                int offset = (int)Marshal.SizeOf(mem) * (int)addr;
                mem.word = value;
                _map_view[(int)deviceType].Write<MemoryAssigne>(offset, ref mem);
            }

            return rc;
        }
        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write32(DeviceType deviceType, UInt32 addr, Int32 value)
        {
            UInt32 rc = 0;

            if (_map_view != null)
            {
                MemoryAssigne mem = new MemoryAssigne();
                int offset = (int)Marshal.SizeOf(mem) * (int)addr;
                mem.word = value;
                _map_view[(int)deviceType].Write<MemoryAssigne>(offset, ref mem);
            }

            return rc;
        }

        /// <summary>
        /// 連続デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, Int32 count, Int32[] value)
        {
            UInt32 rc = 0;
     
            for (int i = 0; i < count; i++)
            {
                if (_map_view != null)
                {
                    MemoryAssigne mem = new MemoryAssigne();
                    mem.word = value[i];
                    int offset = (int)Marshal.SizeOf(mem) * (int)(addr + i);
                    _map_view[(int)deviceType].Write<MemoryAssigne>(offset, ref mem);
                }
            }

            return rc;
        }

        /// <summary>
        /// 文字書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, string buf)
        {
            UInt32 rc = 0;

            if (_map_view != null)
            {
                // @@20220809-2
                if (deviceType == DeviceType.MaxCount)
                {   // デバッグ用に デバイスタイプ MaxCountを指定したらデバイスファイル保存する
                    WriteDeviceFile();
                    return 0;
                }


                //Byte[] b = ASCIIEncoding.ASCII.GetBytes(buf);
                Byte[] b = Encoding.GetEncoding("Shift-JIS").GetBytes(buf);
                int wordCount = (int)Math.Ceiling((double)b.Length / 2);
                bool addZero = b.Length == wordCount * 2;
                if (addZero) wordCount++;   // 最後に1ワード分0を書き込む
                Array.Resize(ref b, wordCount * 2);

                for (int i = 0; i < wordCount; i++)
                {
                    MemoryAssigne mem = new MemoryAssigne();
                    int offset = (int)Marshal.SizeOf(mem) * (int)(addr + i);
                    mem.byte0 = b[i*2];
                    mem.byte1 = b[i*2+1];
                    _map_view[(int)deviceType].Write<MemoryAssigne>(offset, ref mem);
                }

            }

            return rc;
        }

        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 rc)
        {
            return rc == 0;
        }

        /// <summary>
        /// デバイス値をファイルから読み出す
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadDeviceFile(string filePath)
        {
            try
            {
                // ファイル有無確認
                if (!FileIo.ExistFile(filePath)) return;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string buf = sr.ReadLine();
                        string[] item = buf.Split(',');
                        if (item.Length == 3 && item[0] != "" && item[1] != "" && item[2] != "")
                        {
                            DeviceType device = (DeviceType)Enum.Parse(typeof(DeviceType), item[0]);
                            int addr = int.Parse(item[1]);
                            int v = int.Parse(item[2]);

                            if (addr < PlcDeviceCount)
                            {
                                MemoryAssigne mem = new MemoryAssigne();
                                int offset = (int)Marshal.SizeOf(mem) * (int)(addr);
                                mem.word = v;
                                _map_view[(int)device].Write<MemoryAssigne>(offset, ref mem);
                            }
                        }
                    }
                }

            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// デバイス値をファイルに書き込む
        /// </summary>
        /// <param name="filePath"></param>
        private void WriteDeviceFile(string filePath)
        {
            try
            {
                if (_map_view == null) return;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath))
                {
                    for (int i = 0; i < (int)DeviceType.MaxCount; i++)
                    {
                        for (int addr = 0; addr < PlcDeviceCount; addr++)
                        {
                            MemoryAssigne mem = new MemoryAssigne();
                            int offset = (int)Marshal.SizeOf(mem) * (int)(addr);
                            _map_view[i].Read<MemoryAssigne>(offset, out mem);
 
                            string buf = string.Format("{0},{1},{2}", ((DeviceType)i).ToString(), addr, mem.word);
                            sw.WriteLine(buf);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// @@20220808-2
        /// </summary>
        private void WriteDeviceFile()
        {
            // ファイルにデバイス値を保存
            WriteDeviceFile(_deviceFileName);
        }

    }
}

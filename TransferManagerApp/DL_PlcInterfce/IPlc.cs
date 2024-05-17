using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_PlcInterfce
{
    /// <summary>
    /// PLCインターフェース
    /// </summary>
    interface IPlc
    {
        /// <summary>
        /// ダミーモードか確認
        /// </summary>
        /// <returns></returns>
        bool IsDummy();

        /// <summary>
        /// 接続
        /// </summary>
        /// <returns></returns>
        UInt32 Open(string connectionParam);
        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        UInt32 Close();

        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Read(DeviceType devType, UInt32 addr, ref int value);
        /// <summary>
        /// 単一デバイス読み出し(32bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Read32(DeviceType devType, UInt32 addr, ref Int32 value);

        /// <summary>
        /// 文字読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="len">最大文字数</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        UInt32 Read(DeviceType devType, UInt32 addr, Int32 len, ref string buf);

        /// <summary>
        /// 連続デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Read(DeviceType devType, UInt32 addr, Int32 count, ref Int32[] value);

        /// <summary>
        /// 単一デバイス書込み(16bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Write(DeviceType devType, UInt32 addr, int value);

        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Write32(DeviceType devType, UInt32 addr, Int32 value);

        /// <summary>
        /// 連続デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        UInt32 Write(DeviceType devType, UInt32 addr, Int32 count, Int32[] value);

        /// <summary>
        /// 文字書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        UInt32 Write(DeviceType devType, UInt32 addr, string buf);

    }
}

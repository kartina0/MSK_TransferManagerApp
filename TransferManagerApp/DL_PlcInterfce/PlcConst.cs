using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_PlcInterfce
{
    /// <summary>
    /// デバイス種別
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// リレー(R)
        /// </summary>
        Relay = 0,

        /// <summary>
        /// 内部リレー(MR/M)
        /// </summary>
        InternalRelay,

        /// <summary>
        /// データメモリ(DM/D)
        /// </summary>
        DataMemory,

        /// <summary>
        /// タイマ(T)
        /// </summary>
        Timer,

        /// <summary>
        /// 拡張メモリ(EM/E)
        /// </summary>
        ExtendDataMemory,

        /// <summary>
        /// ファイルレジスタ(ZF/ZR)
        /// </summary>
        FileRegister,

        /// <summary>
        /// リンクリレー(B)
        /// </summary>
        LinkRelay,

        /// <summary>
        /// リンクレジスタ(W)
        /// </summary>
        LinkRegister,

        MaxCount,

    }

}

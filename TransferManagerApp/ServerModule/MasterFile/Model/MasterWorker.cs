//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using SystemConfig;


namespace ServerModule
{
    /// <summary>
    /// 作業者マスタファイル データ
    /// </summary>
    public class MasterWorker
    {
        /// <summary>
        /// 作業者No(代表)
        /// </summary>
        public int workerChiefNo = 0;
        /// <summary>
        /// 作業者名(代表)
        /// </summary>
        public string workerChiefName = "";
        /// <summary>
        /// 作業者名 1~当日3便
        /// </summary>
        public int[] workerNo = new int[Const.MaxPostCount + 1];
    }
}

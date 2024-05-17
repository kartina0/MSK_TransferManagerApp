//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 引数無し
    /// </summary>
    public delegate void delegate_void();

    /// <summary>
    /// メッセージボックス表示
    /// </summary>
    /// <param name="msg"></param>
    public delegate void delegate_display_messageBox(string msg);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate void delegate_void_string(string p1);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate void delegate_void_string_string(string p1, string p2);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate string delegate_string_string(string p1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate void delegate_void_bool(bool p1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate void delegate_void_bool_bool(bool p1, bool p2);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate bool delegate_bool_string(string p1);

    /// <summary>
    /// ダイアログ表示
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="btn"></param>
    /// <returns></returns>
    public delegate DialogResult delegate_show_dialog(string msg, MessageBoxButtons btn);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    public delegate void delegate_void_int_string(int p1, string p2);


}

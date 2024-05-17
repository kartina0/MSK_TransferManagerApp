using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;


using DL_Logger;
using ErrorCodeDefine;
using ShareResource;
using SystemConfig;
using System.Globalization;
using ServerModule;
using BatchModule;
using System.Linq;

namespace TransferManagerApp
{
    /// <summary>
    /// windowOutputOrderReport.xaml の相互作用ロジック
    /// </summary>
    public partial class windowOutputOrderReport : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowOutputOrderReport";

        /// <summary>
        /// 便インデックス
        /// </summary>
        public int _postIndex = 0;

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;

        /// <summary>
        /// プレビュー ウィンドウオブジェクト
        /// </summary>
        private windowPreview _windowPreview = null;

        /// <summary>
        /// ステーションごとの仕分数リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindStationOrderCount> _stationOrderCount = new ObservableCollection<BindStationOrderCount>();
        /// <summary>
        /// 商品ごとの仕分数リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkOrderCount> _workOrderCount = new ObservableCollection<BindWorkOrderCount>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowOutputOrderReport()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ウィンドウロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                // ウィンドウ表示中
                isShowing = true;

                // 便Noコンボボックス
                //comboPostNo.Items.Add("1");
                //comboPostNo.Items.Add("2");
                //comboPostNo.Items.Add("3");
                comboPostNo.SelectedIndex = Resource.SystemStatus.CurrentPostIndex;

                // ListView初期化
                rc = InitListView();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ウィンドウ表示終了
                isShowing = false;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == btnPreview)
                {// プレビュー

                    rc = PreviewPdf(out FixedDocument document);

                    if (STATUS_SUCCESS(rc)) 
                    {
                        // 重複表示防止
                        if (_windowPreview == null || !_windowPreview.isShowing)
                        {
                            _windowPreview = new windowPreview(document);
                            _windowPreview.Show();
                        }
                    }

                }
                else if (ctrl == btnOutput)
                {// PDFファイル出力

                    DateTime dtNow = DateTime.Now;
                    string fileName = dtNow.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    string filePath = System.IO.Path.Combine(IniFile.OutputOrderReportDir, fileName);

                    rc = OutputPdf(filePath);
                }
                else if (ctrl == btnExit)
                {// 閉じる
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// 選択変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UInt32 rc = 0;
            //if (!_initialized)
            //    return;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() " +
            //    $"日付:{dpOrderDate.SelectedDate.ToString()} 便:{_comboPostNoList[comboPostNo.SelectedIndex].DisplayValue} アイル:{_comboAisleNoList[comboAisleNo.SelectedIndex].DisplayValue}");
            try
            {
                _postIndex = comboPostNo.SelectedIndex;
                rc = UpdateList(_postIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ListView初期化
        /// </summary>
        /// <returns></returns>
        private UInt32 InitListView()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 表の各カラムのWidthを調整
                double w = listviewOrderRemainInfo.ActualWidth;
                columnJanCode.Width = Math.Floor(w * 0.09);
                columnWorkCode.Width = Math.Floor(w * 0.06);
                columnWorkName.Width = Math.Floor(w * 0.25);
                columnSupplierCode.Width = Math.Floor(w * 0.07);
                columnSupplierName.Width = Math.Floor(w * 0.20);
                columnOrderCount.Width = Math.Floor(w * 0.06);
                columnOrderCompCount.Width = Math.Floor(w * 0.06);
                columnWorkDamagedCount.Width = Math.Floor(w * 0.06);
                columnOrderRemainCount.Width = Math.Floor(w * 0.06);
                columnReason.Width = Math.Floor(w * 0.08);

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 画面のテーブルを更新
        /// </summary>
        /// <returns></returns>
        private UInt32 UpdateList(int postIndex)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // -------------------------------------
                // ステーションごとのテーブル
                _stationOrderCount = null;
                _stationOrderCount = new ObservableCollection<BindStationOrderCount>();

                int orderCount_St1 = 0;
                int inputCount_St1 = 0;
                int orderCompCount_St1 = 0;
                int damagedCount_St1 = 0;

                int orderCount_St2 = 0;
                int inputCount_St2 = 0;
                int orderCompCount_St2 = 0;
                int damagedCount_St2 = 0;

                int orderCount_St3 = 0;
                int inputCount_St3 = 0;
                int orderCompCount_St3 = 0;
                int damagedCount_St3 = 0;

                foreach (ExecuteData work in Resource.Server.OrderInfo.ExecuteDataList[postIndex]) 
                {
                    foreach (ExecuteStoreData store in work.storeDataList) 
                    {
                        if (store.aisleNo_MH >= 1 && store.aisleNo_MH <= 3)
                        {
                            orderCount_St1 += (int)store.orderCount;
                            inputCount_St1 += (int)store.orderCount;
                            orderCompCount_St1 += (int)store.orderCompCount;
                            damagedCount_St1 = 0;
                        }
                        else if (store.aisleNo_MH >= 4 && store.aisleNo_MH <= 6)
                        {
                            orderCount_St2 += (int)store.orderCount;
                            inputCount_St2 += (int)store.orderCount;
                            orderCompCount_St2 += (int)store.orderCompCount;
                            damagedCount_St2 = 0;
                        }
                        else if (store.aisleNo_MH >= 7)
                        {
                            orderCount_St3 += (int)store.orderCount;
                            inputCount_St3 += (int)store.orderCount;
                            orderCompCount_St3 += (int)store.orderCompCount;
                            damagedCount_St3 = 0;
                        }
                    }
                }

                _stationOrderCount.Add(new BindStationOrderCount
                {
                    St1 = orderCount_St1.ToString(),
                    St2 = orderCount_St2.ToString(),
                    St3 = orderCount_St3.ToString()
                });
                _stationOrderCount.Add(new BindStationOrderCount
                {
                    St1 = inputCount_St1.ToString(),
                    St2 = inputCount_St2.ToString(),
                    St3 = inputCount_St3.ToString()
                });
                _stationOrderCount.Add(new BindStationOrderCount
                {
                    St1 = orderCompCount_St1.ToString(),
                    St2 = orderCompCount_St2.ToString(),
                    St3 = orderCompCount_St3.ToString()
                });
                _stationOrderCount.Add(new BindStationOrderCount
                {
                    St1 = damagedCount_St1.ToString(),
                    St2 = damagedCount_St2.ToString(),
                    St3 = damagedCount_St3.ToString()
                });

                listviewStationOrderCount.ItemsSource = _stationOrderCount;



                // -------------------------------------
                // 商品ごとのテーブル
                _workOrderCount = null;
                _workOrderCount = new ObservableCollection<BindWorkOrderCount>();

                foreach (ExecuteData work in Resource.Server.OrderInfo.ExecuteDataList[postIndex])
                {
                    string janCode = work.JANCode;
                    string workCode = work.workCode;
                    rc = Resource.Server.MasterFile.GetWorkNameFromJanCode(postIndex, janCode, out string workName);
                    workName = workName.Trim();
                    rc = Resource.Server.MasterFile.GetSuplierFromJanCode(postIndex, janCode, out string supplierCode, out string supplierName);
                    supplierName = supplierName.Trim();
                    string orderCount = $"{(int)work.orderCountTotal}";
                    string orderCompCount = $"{(int)work.orderCompCountTotal}";
                    string damagedCount = "0";
                    string remainCount = ((int)work.orderCountTotal - (int)work.orderCompCountTotal).ToString();
                    string reason = "";

                    BindWorkOrderCount b = new BindWorkOrderCount
                    {
                        JanCode = janCode,
                        WorkCode = workCode,
                        WorkName = workName,
                        SupplierCode = supplierCode,
                        SupplierName = supplierName,
                        OrderCount = orderCount,
                        OrderCompCount = orderCompCount,
                        DamagedCount = damagedCount,
                        RemainCount = remainCount,
                        Reason = reason
                    };

                    _workOrderCount.Add(b);
                }

                listviewOrderRemainInfo.ItemsSource = _workOrderCount;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// PDFファイル出力
        /// </summary>
        private UInt32 OutputPdf(string outputFilePath)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // サイズ
                int width = 1169;
                int height = 827;
                // 現在日時
                DateTime currentDt = DateTime.Now;
                // 未処理商品だけのリストを作成
                List<BindWorkOrderCount> list = _workOrderCount.Where(x => int.Parse(x.RemainCount) > 0).ToList();
                // ページ数算出
                int pageCount = 0;
                if (list.Count <= 8)
                {
                    pageCount = 1;
                }
                else
                {
                    pageCount = 1 + (list.Count - 8) / 20;
                    if ((list.Count - 8) % 20 > 0)
                        pageCount += 1;
                }


                System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.DefaultPageSettings.Landscape = true;

                int pageIndex = 0;
                printDoc.PrintPage += (sender, e) =>
                {
                    // ここで印刷内容を描画
                    System.Drawing.Graphics graphics = e.Graphics;
                    DrawPdf(graphics, width, height, pageIndex, pageCount, currentDt, list);

                    // ページ数処理
                    pageIndex++;
                    if (pageIndex >= pageCount)
                        e.HasMorePages = false;
                    else
                        e.HasMorePages = true;
                };

                // 出力先をPDFファイルに設定
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = outputFilePath;

                // 印刷を開始
                printDoc.Print();

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// PDFをプレビュー表示
        /// ※描画
        /// </summary>
        private UInt32 PreviewPdf(out FixedDocument document)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            document = new FixedDocument();
            try
            {
                // サイズ
                int width = 1169;
                int height = 827;
                // 現在日時
                DateTime currentDt = DateTime.Now;
                // 未処理商品だけのリストを作成
                List<BindWorkOrderCount> list = _workOrderCount.Where(x => int.Parse(x.RemainCount) > 0).ToList();
                // ページ数算出
                int pageCount = 0;
                if (list.Count <= 8)
                {
                    pageCount = 1;
                }
                else 
                {
                    pageCount = 1 + (list.Count - 8) / 20;
                    if ((list.Count - 8) % 20 > 0)
                        pageCount += 1;
                }


                int pageIndex = 0;
                while (true)
                {
                    // 描画したい内容を作成
                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        DrawPdf(context, width, height, pageIndex, pageCount, currentDt, list);
                    }
                    // 描画内容をビットマップにレンダリング
                    RenderTargetBitmap renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    renderTarget.Render(visual);

                    // FixedPageにビットマップを追加
                    //document = new FixedDocument();
                    PageContent pageContent = new PageContent();
                    FixedPage fixedPage = new FixedPage();

                    // 横向きのサイズ（8.5 x 11インチをポイント単位で表現）
                    fixedPage.Width = width;
                    fixedPage.Height = height;

                    Image image = new Image();
                    image.Source = renderTarget;
                    fixedPage.Children.Add(image);
                    pageContent.Child = fixedPage;

                    // ページを追加
                    document.Pages.Add(pageContent);
                    pageIndex++;
                    if (pageIndex >= pageCount)
                        break;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        ///// <summary>
        ///// PDFをプレビュー表示
        ///// ※外部ファイル読み込み
        ///// </summary>
        //private async void PreviewPdf(string pdffilePath)
        //{
        //    Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
        //    FixedDocument document = new FixedDocument();
        //    try
        //    {
        //        // ファイル情報取得
        //        var file = await Windows.Storage.StorageFile.GetFileFromPathAsync("D:\\work\\practice\\C#\\Native\\WPF\\test\\Test\\PDF\\PdfTest01\\Compiled\\PDF\\aaaaa.pdf");
        //        // PDFオブジェクト取得
        //        Windows.Data.Pdf.PdfDocument pdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);

        //        if (pdfDocument != null)
        //        {
        //            //var document = new FixedDocument();

        //            // 全ページループ
        //            for (uint pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
        //            {
        //                // 1ページずつ読み出して、documentオブジェクトに追加する
        //                using (Windows.Data.Pdf.PdfPage page = pdfDocument.GetPage(pageIndex))
        //                {
        //                    BitmapImage image = new BitmapImage();

        //                    using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
        //                    {
        //                        await page.RenderToStreamAsync(stream);

        //                        image.BeginInit();
        //                        image.CacheOption = BitmapCacheOption.OnLoad;
        //                        image.StreamSource = stream.AsStream();
        //                        image.EndInit();


        //                        var pageContent = new Image();
        //                        pageContent.Source = image;

        //                        // FixedPageにページコンテンツを追加
        //                        var fixedPage = new FixedPage();
        //                        fixedPage.Children.Add(pageContent);

        //                        // FixedPageをFixedDocumentに追加
        //                        var pageContentContainer = new PageContent();
        //                        ((IAddChild)pageContentContainer).AddChild(fixedPage);
        //                        document.Pages.Add(pageContentContainer);
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Resource.ErrorHandler(ex);
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return;
        //}




        /// <summary>
        /// PDF描画
        /// </summary>
        private UInt32 DrawPdf(System.Drawing.Graphics graphics, int width, int height, int pageIndex, int pageCount, DateTime currentDt, List<BindWorkOrderCount> list)
        {
            UInt32 rc = 0;
            try
            {
                System.Drawing.Pen pen = null;
                pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1.0f);
                System.Drawing.Font font;
                string text = "";
                int fontsize = 0;
                //string fontType = "MS UI Gothic";
                string fontType = "Meiryo";
                System.Drawing.PointF start;
                System.Drawing.PointF end;

                int start_x = 0;
                int start_y = 0;
                int text_x = 3;
                int text_y = 7;

                if (pageIndex == 0)
                {
                    int rowCount = 8;

                    // ----------------------------------------------
                    // タイトル
                    // ----------------------------------------------
                    fontsize = 18;
                    text = "仕分け作業完了報告書";
                    font = new System.Drawing.Font(fontType, fontsize, System.Drawing.FontStyle.Bold);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(455, 15));

                    // ----------------------------------------------
                    // 日付/ページ
                    // ----------------------------------------------
                    fontsize = 11;
                    #region 日付/ページ
                    text = currentDt.ToString("yyyy/MM/dd HH:mm");
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF((width - 20) - 240, 15));

                    //text = "作成";
                    //font = new System.Drawing.Font(fontType, fontsize);
                    //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF((width - 20) - 80, 15));

                    text = $"{pageIndex + 1} / {pageCount}";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF((width - 30) - 30, 15));
                    #endregion


                    // ----------------------------------------------
                    // 左上
                    // ----------------------------------------------
                    #region 左上
                    fontsize = 11;
                    start_x = 40;
                    start_y = 70;

                    text = "センター";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x, start_y));

                    text = "納品日";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x, start_y + 25));

                    text = "便No";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x, start_y + 50));

                    text = "作業日";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x, start_y + 75));

                    text = "大田CDC";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + 70, start_y));

                    text = $"{currentDt.ToString("yyyy/MM/dd")}";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + 70, start_y + 25));

                    text = $"{_postIndex + 1}便";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + 70, start_y + 50));

                    text = $"{currentDt.ToString("yyyy/MM/dd")}";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + 70, start_y + 75));
                    #endregion


                    // ----------------------------------------------
                    // 右上
                    // ----------------------------------------------
                    #region 右上
                    start = new System.Drawing.PointF((width - 20) - 250, 65);
                    end = new System.Drawing.PointF((width - 20) - 10, 65);
                    graphics.DrawLine(pen, start, end);

                    start = new System.Drawing.PointF((width - 20) - 250, 135);
                    end = new System.Drawing.PointF((width - 20) - 10, 135);
                    graphics.DrawLine(pen, start, end);

                    start = new System.Drawing.PointF((width - 20) - 250, 65);
                    end = new System.Drawing.PointF((width - 20) - 250, 135);
                    graphics.DrawLine(pen, start, end);

                    start = new System.Drawing.PointF((width - 20) - 170, 65);
                    end = new System.Drawing.PointF((width - 20) - 170, 135);
                    graphics.DrawLine(pen, start, end);

                    start = new System.Drawing.PointF((width - 20) - 90, 65);
                    end = new System.Drawing.PointF((width - 20) - 90, 135);
                    graphics.DrawLine(pen, start, end);

                    start = new System.Drawing.PointF((width - 20) - 10, 65);
                    end = new System.Drawing.PointF((width - 20) - 10, 135);
                    graphics.DrawLine(pen, start, end);

                    text = "出力担当";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(900, 140));

                    //text = "-";
                    //font = new System.Drawing.Font(fontType, fontsize);
                    //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(990, 140));

                    //text = "-";
                    //font = new System.Drawing.Font(fontType, fontsize);
                    //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(1040, 140));
                    #endregion


                    // ----------------------------------------------
                    // テーブル(上)
                    // ----------------------------------------------
                    fontsize = 10;
                    #region 枠組
                    // 横線
                    start_x = 20;
                    start_y = 180;
                    int count_y = 0;
                    int span_y = 32;

                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "ステーション";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "作業時間";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "作業予定 (SKU/ピース)";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "入荷完了 (SKU/ピース)";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "仕分完了 (SKU/ピース)";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + count_y * span_y);
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "庫内破損 (SKU/ピース)";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + count_y * span_y);
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);


                    // 縦線
                    float w_total = (width - start_x) - start_x;
                    float station = 0.15f;
                    float st1 = (1 - station) / 8.0f;
                    float st2 = (1 - station) / 8.0f;
                    float st3 = (1 - station) / 8.0f;
                    float total = (1 - station) / 8.0f;
                    float sub1 = (1 - station) / 8.0f;
                    float sub2 = (1 - station) / 8.0f;
                    float sub3 = (1 - station) / 8.0f;
                    float sub4 = (1 - station) / 8.0f;
                    float sum = start_x;

                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    sum += w_total * station;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "1ST";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * st1;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "2ST";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * st2;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "3ST";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * st3;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "全体";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * total;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    sum += w_total * sub1;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    sum += w_total * sub2;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    sum += w_total * sub3;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    sum += w_total * sub4;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    count_y = 1;
                    sum = start_x;

                    sum += w_total * station;
                    DateTime startDt = DateTime.MinValue;
                    string startTime = "";
                    string endTime = "";
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                    {
                        if (PreStatus.PostStartDt[postIndex] != DateTime.MinValue)
                        {
                            if (startDt == DateTime.MinValue)
                                startDt = PreStatus.PostStartDt[postIndex];
                            else if (PreStatus.PostStartDt[postIndex] < startDt)
                                startDt = PreStatus.PostStartDt[postIndex];
                        }
                    }
                    startTime = $"{(startDt.Hour).ToString("X2")}:{(startDt.Minute).ToString("X2")}";
                    text = startTime;
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 48, start_y + (count_y * span_y) + text_y));
                    DateTime endDt = DateTime.MinValue;
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                    {
                        if (PreStatus.PostEndDt[postIndex] != DateTime.MinValue)
                        {
                            if (endDt == DateTime.MinValue)
                                endDt = PreStatus.PostEndDt[postIndex];
                            else if (PreStatus.PostEndDt[postIndex] > endDt)
                                endDt = PreStatus.PostEndDt[postIndex];
                        }
                    }
                    endTime = $"{(endDt.Hour).ToString("X2")}:{(endDt.Minute).ToString("X2")}";
                    text = endTime;
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    TimeSpan duration = endDt.Subtract(startDt);
                    text = $"{(int)duration.TotalHours}時間{duration.Minutes}分";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st1;
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st2;
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st3;
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    count_y++;
                    for (int i = 0; i < 4; i++)
                    {
                        count_y++;
                        sum = start_x;

                        if (i < 3)
                        {
                            sum += w_total * station;
                            text = $"{_stationOrderCount[i].St1}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St1}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st1;
                            text = $"{_stationOrderCount[i].St2}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St2}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st2;
                            text = $"{_stationOrderCount[i].St3}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St3}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st3;
                            text = $"{int.Parse(_stationOrderCount[i].St1) + int.Parse(_stationOrderCount[i].St2) + int.Parse(_stationOrderCount[i].St3)}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{int.Parse(_stationOrderCount[i].St1) + int.Parse(_stationOrderCount[i].St2) + int.Parse(_stationOrderCount[i].St3)}";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 62, start_y + (count_y * span_y) + text_y));
                        }
                        else 
                        {
                            sum += w_total * station;
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st1;
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st2;
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st3;
                            text = "/";
                            font = new System.Drawing.Font(fontType, fontsize);
                            graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + 54, start_y + (count_y * span_y) + text_y));

                        }

                    }
                    #endregion


                    // ----------------------------------------------
                    // テーブル(下)
                    // ----------------------------------------------
                    fontsize = 10;

                    text = "未処理商品";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(30, 440));

                    text = "合計SKU数";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(1010, 440));

                    //text = "-";
                    //font = new System.Drawing.Font(fontType, fontsize);
                    //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(1090, 440));

                    #region 枠組
                    start_x = 20;
                    start_y = 460;

                    // 横線
                    count_y = 0;
                    span_y = 32;

                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    count_y++;
                    start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                    end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);


                    // 縦線
                    w_total = (width - start_x) - start_x;
                    float janCode = 0.1f;
                    float workCode = 0.05f;
                    float workName = 0.28f;
                    float supplierCode = 0.05f;
                    float supplierName = 0.18f;
                    float orderCount = 0.06f;
                    float inputCount = 0.06f;
                    float damagedCount = 0.06f;
                    float remainCount = 0.06f;
                    float reason = 0.10f;

                    sum = start_x;
                    fontsize = 9;

                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "JANコード";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * janCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "商品";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * workCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "商品名/規格";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * workName;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "取引先";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * supplierCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "取引先名";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * supplierName;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "仕分予定";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * orderCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "入荷完了";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * inputCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "庫内破損";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * damagedCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "未仕分数";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * remainCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "理由";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * reason;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    count_y = 0;
                    for (int i = 0; i < rowCount; i++)
                    {
                        sum = start_x;
                        count_y++;

                        text = list[i].JanCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * janCode;
                        text = list[i].WorkCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workCode;
                        text = list[i].WorkName;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workName;
                        text = list[i].SupplierCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierCode;
                        text = list[i].SupplierName;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierName;
                        text = list[i].OrderCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * orderCount;
                        text = list[i].OrderCompCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * inputCount;
                        //text = list[i].DamagedCount;
                        //font = new System.Drawing.Font(fontType, fontsize);
                        //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * damagedCount;
                        text = list[i].RemainCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * remainCount;
                        text = list[i].Reason;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));
                    }
                    #endregion


                }
                else
                {
                    int rowCount = 20;


                    // ----------------------------------------------
                    // 日付/ページ
                    // ----------------------------------------------
                    fontsize = 11;
                    #region 日付/ページ
                    text = currentDt.ToString("yyyy/MM/dd HH:mm");
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF((width - 20) - 240, 15));

                    text = $"{pageIndex + 1} / {pageCount}";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF((width - 30) - 30, 15));
                    #endregion


                    // ----------------------------------------------
                    // テーブル(下)
                    // ----------------------------------------------
                    fontsize = 10;

                    text = "未処理商品";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(30, 45));


                    #region 枠組
                    start_x = 20;
                    start_y = 70;

                    // 横線
                    int count_y = 0;
                    int span_y = 32;

                    for (int i = 0; i < rowCount + 2; i++)
                    {
                        start = new System.Drawing.PointF(start_x, start_y + (count_y * span_y));
                        end = new System.Drawing.PointF(width - start_x, start_y + (count_y * span_y));
                        graphics.DrawLine(pen, start, end);
                        count_y++;
                    }


                    // 縦線
                    float w_total = (width - start_x) - start_x;
                    float janCode = 0.1f;
                    float workCode = 0.05f;
                    float workName = 0.28f;
                    float supplierCode = 0.05f;
                    float supplierName = 0.18f;
                    float orderCount = 0.06f;
                    float inputCount = 0.06f;
                    float damagedCount = 0.06f;
                    float remainCount = 0.06f;
                    float reason = 0.10f;
                    float sum = start_x;

                    count_y--;
                    fontsize = 9;

                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "JANコード";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * janCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "商品";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * workCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "商品名/規格";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * workName;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "取引先";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * supplierCode;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "取引先名";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * supplierName;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "仕分予定";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * orderCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "入荷完了";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * inputCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "庫内破損";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * damagedCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "未仕分数";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * remainCount;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);

                    text = "理由";
                    font = new System.Drawing.Font(fontType, fontsize);
                    graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + text_y));

                    sum += w_total * reason;
                    start = new System.Drawing.PointF(sum, start_y);
                    end = new System.Drawing.PointF(sum, start_y + (count_y * span_y));
                    graphics.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    count_y = 0;
                    for (int i = 8 + (pageIndex - 1) * rowCount; i < 8 + pageIndex * rowCount; i++)
                    {
                        sum = start_x;
                        count_y++;

                        text = list[i].JanCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * janCode;
                        text = list[i].WorkCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workCode;
                        text = list[i].WorkName;
                        if (text.Length > 25)
                            text = text.Substring(0, 25);   // テキストが枠からはみ出るのを防止
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workName;
                        text = list[i].SupplierCode;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierCode;
                        text = list[i].SupplierName;
                        if (text.Length > 15)
                            text = text.Substring(0, 15);   // テキストが枠からはみ出るのを防止
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierName;
                        text = list[i].OrderCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * orderCount;
                        text = list[i].OrderCompCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * inputCount;
                        //text = list[i].DamagedCount;
                        //font = new System.Drawing.Font(fontType, fontsize);
                        //graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * damagedCount;
                        text = list[i].RemainCount;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * remainCount;
                        text = list[i].Reason;
                        font = new System.Drawing.Font(fontType, fontsize);
                        graphics.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.PointF(sum + text_x, start_y + (count_y * span_y) + text_y));
                    }
                    #endregion

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rc;
        }
        /// <summary>
        /// PDF描画
        /// </summary>
        private UInt32 DrawPdf(DrawingContext context, int width, int height, int pageIndex, int pageCount, DateTime currentDt, List<BindWorkOrderCount> list)
        {
            UInt32 rc = 0;
            try
            {
                Pen pen = null;
                pen = new Pen(Brushes.Black, 1.0);
                FormattedText format = null;
                string text = "";
                int fontsize = 0;
                //string fontType = "MS UI Gothic";
                string fontType = "Meiryo";
                Point start;
                Point end;


                int start_x = 0;
                int start_y = 0;
                int text_x = 3;
                int text_y = 7;

                if (pageIndex == 0)
                {// 1ページ目
                    int rowCount = 8;

                    // ----------------------------------------------
                    // タイトル(上)
                    // ----------------------------------------------
                    fontsize = 28;
                    text = "仕分け作業完了報告書";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Bold);
                    context.DrawText(format, new Point(455, 15));


                    // ----------------------------------------------
                    // 日付/ページ
                    // ----------------------------------------------
                    fontsize = 16;
                    #region 日付/ページ
                    text = currentDt.ToString("yyyy/MM/dd HH:mm");
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point((width - 20) - 240, 15));

                    text = $"{pageIndex + 1} / {pageCount}";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point((width - 30) - 30, 15));
                    #endregion


                    // ----------------------------------------------
                    // 左上
                    // ----------------------------------------------
                    #region 左上
                    fontsize = 14;
                    start_x = 40;
                    start_y = 70;

                    text = "センター";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x, start_y));

                    text = "納品日";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x, start_y + 25));

                    text = "便No";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x, start_y + 50));

                    text = "作業日";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x, start_y + 75));

                    text = "大田CDC";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + 70, start_y));

                    text = $"{currentDt.ToString("yyyy/MM/dd")}";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + 70, start_y + 25));

                    text = $"{_postIndex + 1}便";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + 70, start_y + 50));

                    text = $"{currentDt.ToString("yyyy/MM/dd")}";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + 70, start_y + 75));
                    #endregion


                    // ----------------------------------------------
                    // 右上
                    // ----------------------------------------------
                    #region 右上
                    fontsize = 14;

                    start = new Point((width - 20) - 250, 65);
                    end = new Point((width - 20) - 10, 65);
                    context.DrawLine(pen, start, end);

                    start = new Point((width - 20) - 250, 135);
                    end = new Point((width - 20) - 10, 135);
                    context.DrawLine(pen, start, end);

                    start = new Point((width - 20) - 250, 65);
                    end = new Point((width - 20) - 250, 135);
                    context.DrawLine(pen, start, end);

                    start = new Point((width - 20) - 170, 65);
                    end = new Point((width - 20) - 170, 135);
                    context.DrawLine(pen, start, end);

                    start = new Point((width - 20) - 90, 65);
                    end = new Point((width - 20) - 90, 135);
                    context.DrawLine(pen, start, end);

                    start = new Point((width - 20) - 10, 65);
                    end = new Point((width - 20) - 10, 135);
                    context.DrawLine(pen, start, end);

                    text = "出力担当";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(900, 140));

                    //text = "-";
                    //format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);
                    //format.SetFontSize(fontsize);
                    //format.SetFontWeight(FontWeights.Thin);
                    //context.DrawText(format, new Point(990, 140));

                    //text = "-";
                    //format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Black);
                    //format.SetFontSize(fontsize);
                    //format.SetFontWeight(FontWeights.Thin);
                    //context.DrawText(format, new Point(1040, 140));
                    #endregion


                    // ----------------------------------------------
                    // テーブル(上)
                    // ----------------------------------------------
                    fontsize = 14;
                    #region 枠組
                    // 横線
                    start_x = 20;
                    start_y = 180;
                    int count_y = 0;
                    int span_y = 32;

                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "ステーション";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "作業時間";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "作業予定 (SKU/ピース)";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "入荷完了 (SKU/ピース)";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "仕分完了 (SKU/ピース)";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new Point(start_x, start_y + count_y * span_y);
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "庫内破損 (SKU/ピース)";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(start_x + text_x, start_y + (count_y * span_y) + text_y));

                    count_y++;
                    start = new Point(start_x, start_y + count_y * span_y);
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);


                    // 縦線
                    double w_total = (width - start_x) - start_x;
                    double station = 0.15;
                    double st1 = (1 - station) / 8.0;
                    double st2 = (1 - station) / 8.0;
                    double st3 = (1 - station) / 8.0;
                    double total = (1 - station) / 8.0;
                    double sub1 = (1 - station) / 8.0;
                    double sub2 = (1 - station) / 8.0;
                    double sub3 = (1 - station) / 8.0;
                    double sub4 = (1 - station) / 8.0;
                    double sum = start_x;

                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    sum += w_total * station;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "1ST";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * st1;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "2ST";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);

                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));
                    sum += w_total * st2;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "3ST";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);

                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));
                    sum += w_total * st3;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "全体";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * total;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    sum += w_total * sub1;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    sum += w_total * sub2;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    sum += w_total * sub3;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    sum += w_total * sub4;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    count_y = 1;
                    sum = start_x;

                    sum += w_total * station;
                    DateTime startDt = DateTime.MinValue;
                    string startTime = "";
                    string endTime = "";
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++) 
                    {
                        if (PreStatus.PostStartDt[postIndex] != DateTime.MinValue)
                        {
                            if (startDt == DateTime.MinValue)
                                startDt = PreStatus.PostStartDt[postIndex];
                            else if(PreStatus.PostStartDt[postIndex] < startDt)
                                startDt = PreStatus.PostStartDt[postIndex];
                        }
                    }
                    startTime = $"{(startDt.Hour).ToString("X2")}:{(startDt.Minute).ToString("X2")}";
                    text = startTime;
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 48, start_y + (count_y * span_y) + text_y));
                    DateTime endDt = DateTime.MinValue;
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                    {
                        if (PreStatus.PostEndDt[postIndex] != DateTime.MinValue)
                        {
                            if (endDt == DateTime.MinValue)
                                endDt = PreStatus.PostEndDt[postIndex];
                            else if (PreStatus.PostEndDt[postIndex] > endDt)
                                endDt = PreStatus.PostEndDt[postIndex];
                        }
                    }
                    endTime = $"{(endDt.Hour).ToString("X2")}:{(endDt.Minute).ToString("X2")}";
                    text = endTime;
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    TimeSpan duration = endDt.Subtract(startDt);
                    text = $"{(int)duration.TotalHours}時間{duration.Minutes}分";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st1;
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st2;
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    sum += w_total * st3;
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 6, start_y + (count_y * span_y) + text_y));
                    text = "～";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 48, start_y + (count_y * span_y) + text_y));
                    text = "00:00";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 64, start_y + (count_y * span_y) + text_y));
                    text = "[";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y + 16));
                    text = "00時間00分";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 16, start_y + (count_y * span_y) + text_y + 16));
                    text = "]";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + 92, start_y + (count_y * span_y) + text_y + 16));

                    count_y++;
                    for (int i = 0; i < 4; i++)
                    {
                        count_y++;
                        sum = start_x;

                        if (i < 3) 
                        {
                            sum += w_total * station;
                            text = $"{_stationOrderCount[i].St1}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St1}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st1;
                            text = $"{_stationOrderCount[i].St2}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St2}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st2;
                            text = $"{_stationOrderCount[i].St3}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{_stationOrderCount[i].St3}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 62, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st3;
                            text = $"{int.Parse(_stationOrderCount[i].St1) + int.Parse(_stationOrderCount[i].St2) + int.Parse(_stationOrderCount[i].St3)}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 10, start_y + (count_y * span_y) + text_y));
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));
                            text = $"{int.Parse(_stationOrderCount[i].St1) + int.Parse(_stationOrderCount[i].St2) + int.Parse(_stationOrderCount[i].St3)}";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 62, start_y + (count_y * span_y) + text_y));
                        }
                        else 
                        {
                            sum += w_total * station;
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st1;
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st2;
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));

                            sum += w_total * st3;
                            text = "/";
                            format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                            format.SetFontSize(fontsize);
                            format.SetFontWeight(FontWeights.Thin);
                            context.DrawText(format, new Point(sum + 54, start_y + (count_y * span_y) + text_y));
                        }
                    }
                    #endregion


                    // ----------------------------------------------
                    // テーブル(下)
                    // ----------------------------------------------
                    text = "未処理商品";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(30, 440));

                    text = "合計SKU数";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(1010, 440));

                    //text = "-";
                    //format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    //format.SetFontSize(fontsize);
                    //format.SetFontWeight(FontWeights.Thin);
                    //context.DrawText(format, new Point(1090, 440));


                    #region 枠組
                    start_x = 20;
                    start_y = 460;

                    // 横線
                    count_y = 0;
                    span_y = 32;

                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    count_y++;
                    start = new Point(start_x, start_y + (count_y * span_y));
                    end = new Point(width - start_x, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);


                    // 縦線
                    w_total = (width - start_x) - start_x;
                    double janCode = 0.1;
                    double workCode = 0.05;
                    double workName = 0.28;
                    double supplierCode = 0.05;
                    double supplierName = 0.18;
                    double orderCount = 0.06;
                    double inputCount = 0.06;
                    double damagedCount = 0.06;
                    double remainCount = 0.06;
                    double reason = 0.10;
                    sum = start_x;

                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "JANコード";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * janCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "商品";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * workCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "商品名/規格";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * workName;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "取引先";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * supplierCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "取引先名";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * supplierName;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "仕分予定";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * orderCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "入荷完了";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * inputCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "庫内破損";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * damagedCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "未仕分数";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * remainCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "理由";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);

                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));
                    sum += w_total * reason;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    fontsize = 13;
                    count_y = 0;

                    for (int i = 0; i < rowCount; i++)
                    {
                        sum = start_x;
                        count_y++;

                        text = list[i].JanCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * janCode;
                        text = list[i].WorkCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workCode;
                        text = list[i].WorkName;
                        if (text.Length > 28)
                            text = text.Substring(0, 28);   // テキストが枠からはみ出るのを防止
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workName;
                        text = list[i].SupplierCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierCode;
                        text = list[i].SupplierName;
                        if (text.Length > 17)
                            text = text.Substring(0, 17);   // テキストが枠からはみ出るのを防止
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierName;
                        text = list[i].OrderCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * orderCount;
                        text = list[i].OrderCompCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * inputCount;
                        //text = list[i].DamagedCount;
                        //format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        //format.SetFontSize(fontsize);
                        //format.SetFontWeight(FontWeights.Thin);
                        //context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * damagedCount;
                        text = list[i].RemainCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * remainCount;
                        text = list[i].Reason;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));
                    }
                    #endregion


                }
                else
                {// 2ページ目以降

                    int rowCount = 20;


                    // ----------------------------------------------
                    // 日付/ページ
                    // ----------------------------------------------
                    fontsize = 16;

                    text = currentDt.ToString("yyyy/MM/dd HH:mm");
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point((width - 20) - 240, 15));

                    text = $"{pageIndex + 1} / {pageCount}";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point((width - 30) - 30, 15));


                    // ----------------------------------------------
                    // テーブル(下)
                    // ----------------------------------------------
                    fontsize = 14;

                    text = "未処理商品";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(30, 45));


                    #region 枠組
                    start_x = 20;
                    start_y = 70;

                    // 横線
                    int count_y = 0;
                    int span_y = 32;

                    for (int i = 0; i < rowCount + 2; i++)
                    {
                        start = new Point(start_x, start_y + (count_y * span_y));
                        end = new Point(width - start_x, start_y + (count_y * span_y));
                        context.DrawLine(pen, start, end);
                        count_y++;
                    }


                    // 縦線
                    double w_total = (width - start_x) - start_x;
                    double janCode = 0.1;
                    double workCode = 0.05;
                    double workName = 0.28;
                    double supplierCode = 0.05;
                    double supplierName = 0.18;
                    double orderCount = 0.06;
                    double inputCount = 0.06;
                    double damagedCount = 0.06;
                    double remainCount = 0.06;
                    double reason = 0.10;
                    double sum = start_x;

                    count_y--;
                    fontsize = 13;

                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "JANコード";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * janCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "商品";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * workCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "商品名/規格";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * workName;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "取引先";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * supplierCode;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "取引先名";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * supplierName;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "仕分予定";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * orderCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "入荷完了";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * inputCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "庫内破損";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * damagedCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "未仕分数";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);
                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));

                    sum += w_total * remainCount;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);

                    text = "理由";
                    format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                    format.SetFontSize(fontsize);
                    format.SetFontWeight(FontWeights.Thin);

                    context.DrawText(format, new Point(sum + text_x, start_y + text_y));
                    sum += w_total * reason;
                    start = new Point(sum, start_y);
                    end = new Point(sum, start_y + (count_y * span_y));
                    context.DrawLine(pen, start, end);
                    #endregion

                    #region データ入力
                    count_y = 0;
                    for (int i = 8 + (pageIndex - 1) * rowCount; i < 8 + pageIndex * rowCount; i++)
                    {
                        sum = start_x;
                        count_y++;

                        text = list[i].JanCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * janCode;
                        text = list[i].WorkCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workCode;
                        text = list[i].WorkName;
                        if (text.Length > 25)
                            text = text.Substring(0, 25);   // テキストが枠からはみ出るのを防止
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * workName;
                        text = list[i].SupplierCode;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierCode;
                        text = list[i].SupplierName;
                        if (text.Length > 15)
                            text = text.Substring(0, 15);   // テキストが枠からはみ出るのを防止
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * supplierName;
                        text = list[i].OrderCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * orderCount;
                        text = list[i].OrderCompCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * inputCount;
                        //text = list[i].DamagedCount;
                        //format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        //format.SetFontSize(fontsize);
                        //format.SetFontWeight(FontWeights.Thin);
                        //context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * damagedCount;
                        text = list[i].RemainCount;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));

                        sum += w_total * remainCount;
                        text = list[i].Reason;
                        format = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontType), 32, Brushes.Black);
                        format.SetFontSize(fontsize);
                        format.SetFontWeight(FontWeights.Thin);
                        context.DrawText(format, new Point(sum + text_x, start_y + (count_y * span_y) + text_y));
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rc;
        }




        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }


    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindStationOrderCount : INotifyPropertyChanged
    {

        private string _st1;
        /// <summary>
        /// ステーション01
        /// </summary>
        public string St1
        {
            get { return _st1; }
            set
            {
                if (_st1 != value)
                {
                    _st1 = value;
                    OnPropertyChanged(nameof(St1));
                }
            }
        }

        private string _st2;
        /// <summary>
        /// ステーション02
        /// </summary>
        public string St2
        {
            get { return _st2; }
            set
            {
                if (_st2 != value)
                {
                    _st2 = value;
                    OnPropertyChanged(nameof(St2));
                }
            }
        }

        private string _st3;
        /// <summary>
        /// ステーション03
        /// </summary>
        public string St3
        {
            get { return _st3; }
            set
            {
                if (_st3 != value)
                {
                    _st3 = value;
                    OnPropertyChanged(nameof(St3));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindWorkOrderCount : INotifyPropertyChanged
    {

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JanCode
        {
            get { return _janCode; }
            set
            {
                if (_janCode != value)
                {
                    _janCode = value;
                    OnPropertyChanged(nameof(JanCode));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _workName;
        /// <summary>
        /// 商品名
        /// </summary>
        public string WorkName
        {
            get { return _workName; }
            set
            {
                if (_workName != value)
                {
                    _workName = value;
                    OnPropertyChanged(nameof(WorkName));
                }
            }
        }

        private string _supplierCode;
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string SupplierCode
        {
            get { return _supplierCode; }
            set
            {
                if (_supplierCode != value)
                {
                    _supplierCode = value;
                    OnPropertyChanged(nameof(SupplierCode));
                }
            }
        }

        private string _supplierName;
        /// <summary>
        /// 取引先名
        /// </summary>
        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                if (_supplierName != value)
                {
                    _supplierName = value;
                    OnPropertyChanged(nameof(SupplierName));
                }
            }
        }

        private string _orderCount;
        /// <summary>
        /// 仕分予定
        /// </summary>
        public string OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    OnPropertyChanged(nameof(OrderCount));
                }
            }
        }

        private string _orderCompCount;
        /// <summary>
        /// 仕分完了
        /// </summary>
        public string OrderCompCount
        {
            get { return _orderCompCount; }
            set
            {
                if (_orderCompCount != value)
                {
                    _orderCompCount = value;
                    OnPropertyChanged(nameof(OrderCompCount));
                }
            }
        }

        private string _damagedCount;
        /// <summary>
        /// 庫内破損
        /// </summary>
        public string DamagedCount
        {
            get { return _damagedCount; }
            set
            {
                if (_damagedCount != value)
                {
                    _damagedCount = value;
                    OnPropertyChanged(nameof(DamagedCount));
                }
            }
        }

        private string _remainCount;
        /// <summary>
        /// 未仕分数
        /// </summary>
        public string RemainCount
        {
            get { return _remainCount; }
            set
            {
                if (_remainCount != value)
                {
                    _remainCount = value;
                    OnPropertyChanged(nameof(RemainCount));
                }
            }
        }

        private string _reason;
        /// <summary>
        /// 理由
        /// </summary>
        public string Reason
        {
            get { return _reason; }
            set
            {
                if (_reason != value)
                {
                    _reason = value;
                    OnPropertyChanged(nameof(Reason));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5      )    #TransferManagerApp.windowMasterFile   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      2local clr-namespace:TransferManagerApp     5      -   5          � 
ResizeMode$  NoResize=�5
       � WindowStyle$
 None=�5       � WindowStartupLocation$ CenterScreen=�5       � Title$$ マスターファイル一覧��5      $	��940��6   $
��1400��6%      PageColorOrder#��[  5      
���p�$��2q�5      .��5      �  � RowDefinitions 5   
   �� 5      $��5* �6   �� 5      $	��46* �6   �� 5      $��4* �6   .+�5   
   ]� $��0��5      ��.  .+�5      �� $��0��5       �� Orientation$ 
Horizontal=�5       .+�5!      �� -   5         ��Name$ btnWorkMaster��5"      $��商品マスター��5#      $��5q�5$      $	��165��5%      $��20(�5&      5'      �� 5(      -   5        $ btnStoreMaster��5)      $��店マスター��5*      $��5q�5+      $	��165��5,      $��20(�5-      5.      �� 5/      -   5        $ btnWorkerMaster��50      $��作業者マスター��51      $��5q�52      $	��165��53      $��20(�54      55      5O      �� 5P      -   5         ��Name$ lblTotalCount��5Q      $��0,0,30,0q�5R      $��Right=�5S      $��16(�5T      $��Bottom=�5U      5V      &   System.Windows.Controls.DataGrid  5Y   
   -   5          Name$ dataGridWorkMaster��5Z      $��1��5[      $��5q�5\      $��17(�5]      	   GridLinesVisibility$	 
Horizontal=�5^      
   HorizontalGridLinesBrush

 �����   
IsReadOnly .    CanUserResizeColumns .     AutoGenerateColumns .  $y�Visible=�5c         Columns 5e      0  *System.Windows.Controls.DataGridTextColumn  5f      ,  &System.Windows.Controls.DataGridColumn   Header$  ��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6/   �� *No+  5g      $ 	納品日��6%    62   �� *	OrderDate+  5h      $ 取引先コード��6%    65   �� *SupplierCode+  5i      $ VDRコード��6%    65   �� *	VDRCode+  5j      $ 取引先名(漢字)��6%    67   �� *SupplierName+  5k      $ 商品コード��6%    64   �� *
WorkCode+  5l      $ 便No��6%    62   �� *PostNo+  5m      $ JANコード��6%    65   �� *	JANCode+  5n      $ 	JAN区分��6%    64   �� *
JANClass+  5o      $ 商品名(漢字)��6%    66   �� *
WorkName+  5p      $ 
DEPT-CLASS��6%    69   �� *	DeptClass+  5q      $ センター入数��6%    65   �� *CenterCount+  5r      $ パック入数��6%    64   �� *	PackCount+  5s      $ JANコード(下4桁)��6%    6:   �� *JANCode4digits+  5t      $  仕分け商品名(カナ)��6%    69   �� *WorkNameKana+   ColumnHeaderStyle 5w      �� 5x       �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5y      ��  �� Property � �� �� Value$ 60��6/   5z         RowStyle 5}      �� 5~      )  #System.Windows.Controls.DataGridRow"  6    �� Triggers 5      P� 5�       P� Property �@J�IsMouseOver P� Value . . �5�      ��  � ��
 �����6;   5�      5�         	CellStyle 5�      �� 5�      *  $System.Windows.Controls.DataGridCell"  6   .�5�      ��  � ��$ 16(�61    5�      P� 5�       �@ 
IsSelected . . �5�      ��  � �� �#C0C0F06;   �� 5�       � ��$ 0q�6@   �� 5�       � ��
 �   �6;   5�      5�      5�        5�   
   -   5        $ dataGridStoreMaster��5�      $��1��5�      $��5q�5�      $��17(�5�      $	 
Horizontal=�5�      

 ����� .  .   .  $y�Hidden=�5�       5�        5�      $  ��6%    6/   �� *No+  5�      $ 	納品日��6%    62   �� *	OrderDate+  5�      $ 	社区分��6%    62   �� *CompanyType+  5�      $ 店コード��6%    63   �� *	StoreCode+  5�      $ 店名(漢字)��6%    65   �� *	StoreName+  5�      $ 電話番号��6%    63   �� *PhoneNumber+  5�      $ コース01��6%    64   �� *
Course01+  5�      $ 順01��6%    62   �� *	Process01+  5�      $
 ST01��6%    63   �� *	Station01+  5�      $ アイル01��6%    64   �� *	Aisle01+  5�      $ スロット01��6%    65   �� *Slot01+  5�      $ ドッグNo01��6%    66   �� *	DogNo01+  5�      $ 搬入条件01��6%    65   �� *Condition01+  5�      $ 到着01��6%    63   �� *TimeArrive01+  5�      $ 入場01��6%    63   �� *TimeEntry01+  5�      $ 出発01��6%    63   �� *TimeDepart01+  5�      $ 終了01��6%    63   �� *TimeFinish01+  5�      $ 運送会社01��6%    65   �� *	Company01+  5�      $ 運送会社名01��6%    66   �� *CompanyName01+  5�      $ 車種01��6%    63   �� *	CarType01+  5�      $ コース02��6%    64   �� *
Course02+  5�      $ 順02��6%    62   �� *	Process02+  5�      $
 ST02��6%    63   �� *	Station02+  5�      $ アイル02��6%    64   �� *	Aisle02+  5�      $ スロット02��6%    65   �� *Slot02+  5�      $ ドッグNo02��6%    66   �� *	DogNo02+  5�      $ 搬入条件02��6%    65   �� *Condition02+  5�      $ 到着02��6%    63   �� *TimeArrive02+  5�      $ 入場02��6%    63   �� *TimeEntry02+  5�      $ 出発02��6%    63   �� *TimeDepart02+  5�      $ 終了02��6%    63   �� *TimeFinish02+  5�      $ 運送会社02��6%    65   �� *	Company02+  5�      $ 運送会社名02��6%    66   �� *CompanyName02+  5�      $ 車種02��6%    63   �� *	CarType02+  5�      $ コース03��6%    64   �� *
Course01+  5�      $ 順03��6%    62   �� *	Process01+  5�      $
 ST03��6%    63   �� *	Station01+  5�      $ アイル03��6%    64   �� *	Aisle01+  5�      $ スロット03��6%    65   �� *Slot01+  5�      $ ドッグNo03��6%    66   �� *	DogNo01+  5�      $ 搬入条件03��6%    65   �� *Condition01+  5�      $ 到着03��6%    63   �� *TimeArrive01+  5�      $ 入場03��6%    63   �� *TimeEntry01+  5�      $ 出発03��6%    63   �� *TimeDepart01+  5�      $ 終了03��6%    63   �� *TimeFinish01+  5�      $ 運送会社03��6%    65   �� *	Company01+  5�      $ 運送会社名03��6%    66   �� *CompanyName01+  5�      $ 車種03��6%    63   �� *	CarType01+ 5�      �� 5�      "  6   .�5�      ��  � ��$ 60��6/   5�       5�      �� 5�      "  6    5�      P� 5�       �@J�IsMouseOver . . �5�      ��  � ��
 �����6;   5�      5�       5�      �� 5�      "  6   .�5�      ��  � ��$ 16(�61    5�      P� 5�       �@ 
IsSelected . . �5�      ��  � �� �#C0C0F06;   �� 5�       � ��$ 0q�6@   �� 5�       � ��
 �   �6;   5�      5�      5�        5�   
   -   5        $ dataGridWorkerMaster��5�      $��1��5�      $��5q�5�      $��17(�5�      $	 
Horizontal=�5�      

 ����� .  .   .  $y�Hidden=�5      5       5     $  ��6%    6/   �� *No+  5     $ 作業者No(代表)��6%    68   �� *WorkerChiefNo+  5     $ 作業者名(代表)��6%    67   �� *WorkerChiefName+  5     $ 作業者名 1便��6%    66   �� *
WorkerNo01+  5     $ 作業者名 2便��6%    66   �� *
WorkerNo02+  5	     $ 作業者名 3便��6%    66   �� *
WorkerNo03+  5
     $ 作業者名 当日3便��6%    68   �� *WorkerNo03_today+ 5     �� 5     "  6   .�5     ��  � ��$ 60��6/   5      5     �� 5     "  6    5     P� 5      �@J�IsMouseOver . . �5     ��  � ��
 �����6;   5     5      5     �� 5     "  6   .�5     ��  � ��$ 16(�61    5      P� 5!      �@ 
IsSelected . . �5"     ��  � �� �#C0C0F06;   �� 5#      � ��$ 0q�6@   �� 5$      � ��
 �   �6;   5%     5'     5)     ]� 5,  
   $��2��5-     ��.  .+�5/     �� $��Right=�50     $ 
Horizontal=�51     .+�52     �� -	   5        $ 	btnUpdate��53     $��更新��54     $��5q�55     $	��120��56     $��20(�57       ButtonColorOrder#��[ 58     
���   �5:     �� 5;     -
   5        $ btnClose��5<     $��	閉じる��5=     $��5q�5>     $	��120��5?     $��20(�5@     #��[ 5A     
���   �5C     5D     5E     5G     5H     
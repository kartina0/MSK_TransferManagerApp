   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5   	   ,    &TransferManagerApp.windowSystemSetting   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:TransferManagerApp     5   	   -   5          � Title$  windowSystemSetting��5	   	   $	��900��6%   $
��1000��62    � 
ResizeMode$ NoResize=�5
   	    � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � ���#000070 � RowDefinitions 5   
   �� 5      $	��12* �6   �� 5      $��1* �6   .+�5   
   � $��0��6   $��2��6    � ColumnDefinitions 5      �� 5      $��2* �6#   �� 5      $��5* �6#   .+�5      �� $��0��6   $��5q�5      
�������
�������$��1q�5        �� CornerRadius$ 1��5!       J� Effect 5"      3  -System.Windows.Media.Effects.DropShadowEffect  5#         
BlurRadius$ 5Q�5$      	   RenderingBias$	 Quality=�5%      
   ShadowDepth$
 1Q�5&         Color$
 Gray��5'      ._�5)      �� $��5q�5*      
�������.+�5,      R� $��24(�5-      .@�5.      Q� -   5         Q�Name$ treeViewItem_Basic��5/      $��基本��50      51      Q� 52      -   5        $ treeViewItem_Equipment��53      $��設備��54      55      Q� 56      -   5        $ treeViewItem_Server��57      $��サーバー��58      59      5C      5D      5E      �� 5I      -   5         ��Name$ borderBasic��5K      $��1��5J      $��5q�5L      
�������
�������$��1q�5O      $ 1��5P      $y�Hidden=�5Q       5R        5S      $ 5Q�5T      $	 Quality=�5U      $
 1Q�5V      $
 Gray��5W      5\      �� 5`      -   5        $ borderEquipment��5b      $��1��5a      $��5q�5c      
�������
�������$��1q�5f      $ 1��5g      $y�Hidden=�5h       5i        5j      $ 5Q�5k      $	 Quality=�5l      $
 1Q�5m      $
 Gray��5n      ._�5q      ,   &TransferManagerApp.ucSetting_Equipment  -   5          Name$ ucSetting_Equipment��5r      $��0q�5s      5t      �� 5x      -   5        $ borderServer��5z      $��1��5y      $��5q�5{      
�������
�������$��1q�5~      $ 1��5      $y�Hidden=�5�       5�        5�      $ 5Q�5�      $	 Quality=�5�      $
 1Q�5�      $
 Gray��5�      ._�5�      )   #TransferManagerApp.ucSetting_Server  -	   5          Name$ ucSetting_Server��5�      $��0q�5�      5�      5�      �� 5�   
   $��1��6   $��0,5,5,5q�5�       �� Orientation$ 
Horizontal=�5�      $��Right=�5�      $��1��61   .+�5�      �� -
   5         ��Name$ btnSave��5�      $��保存��5�      $��	10,0,10,0q�5�      $	��120��5�      $��20(�5�      
����� �
����� �5�      �� 5�      -   5        $ btnExit��5�      $��終了��5�      $��	10,0,10,0q�5�      $	��120��5�      $��20(�5�      
����� �
����� �5�      5�      5�      5�      
   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5   	   '    !TransferManagerApp.windowRegistry   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:TransferManagerApp     5   	   -   5          � Title$  windowRegistry��5	   	   $
��1000��6    $
��1900��6.    � 
ResizeMode$ NoResize=�5
   	    � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterScreen=�5   	      BaseColorRegistry#��[  5   	   $��1q�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $
��0.8* �6   �� 5      $	��16* �6   �� 5      $��1* �6   �� 5      $
��1.7* �6   .+�5   
   !   TransferManagerApp.ucHeader  -   5          Name$ ucHeader��5      $��0��5      $��0q�5       �� 5"   
   $��1��5#      $��0q�5$        BorderColor#��[ 5%      $��1q�5&      $��Stretch=�5'      ._�5(      o� $��24(�5)        SecondColor#��[ 5*      $��Stretch=�5+      5,      5-      � 5/   
   $��2��50       51      �� 52      $��1* �6    �� 53      $	��12* �6    .+�56      ]� $��0��57      $��0q�58      ��.  .+�5:      ��  �� Orientation$ 
Horizontal=�5;      $
��Left=�5<      .+�5=      �� -   5         ��Name$ btnShelfMaster��5>      $��棚マスタ��5?      $��5q�5@      $	��150��5A      $��22(�5B      5C      5D      �� 5O      -   5        $ btnClose��5P      $��	閉じる��5Q      $��Right=�5R      $��5q�5S      $	��120��5T      $��24(�5U      ��5W      �� 5X       �� 
TargetType" ��6    .�5Y      �� 	 �� Property	 � ��
 �� Value
 5Z   "   �� 5[   &    �� 
TargetType" ��66   .R�5\   *   � .+�5]   .   �� $��border��5^   1   ���#F0B0B0
���iii�$��2q�5a   1    �� CornerRadius$ 5��5b   1   ._�5c   2   �� $��Center=�6C   $��Center=�6`   5d   /   5e   +    �� Triggers 5f   *   P� 5g   .    P� Property �@J�IsMouseOver P� Value . . �5h   2   ��  �� 
TargetName$ border��69   	 � ��
 �#FFD0D06c   5i   /   5k   '   5m      5n      5p      5q      *   $TransferManagerApp.ucTab_ShelfMaster  5t      -   5          Name$ ucTab_ShelfMaster��5u      $��1��5v      $��3q�5w      #��[ 5x      $��2q�5y      $y�Visible=�5z      5|      !   TransferManagerApp.ucFooter  5�   
   -   5          Name$ ucFooter��5�      $��3��5�      $��0q�5�      %   TransferManagerApp.ucKeyControl  5�   
   -   5          Name$ ucKeyControl��5�      $��4��5�      $��0q�5�      5�      5�      
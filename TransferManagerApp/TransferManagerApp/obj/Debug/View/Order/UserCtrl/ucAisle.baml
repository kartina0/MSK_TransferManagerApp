   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5           TransferManagerApp.ucAisle   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2local clr-namespace:TransferManagerApp     5      -   5        $��1q�5        � 	Resources  5          BooleanToVisibilityConverter��%   &	        �� 5   
   6'   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��7* �6   �� 5      $��9* �6   .+�5   
   �� -   5         ��Name$ bdWhole��5      $��0��5      $��3��5       
���   �$��2q�5"      
�������5#      ]� 5&   
   $��0��5'      ��.  .+�5+      �� $��6q�5,      .+�5-      �� -   5         ��Name$ 
lblAisleNo��5.      $��アイル 01��5/      $��20(�50      �� 51      -   5        $ lblAutoStatus��52      $��	停止中��53      $��20(�54      $��1q�55      
�������$��Center=�57      58      5|      '   !TransferManagerApp.ucAisleTopView  5�   
   -   5          Name$ ucAisleTopView��5�      $��1��5�      �� 5�   
   -   5         ��Name$ listviewEntryWorkInfo��5�      $��2��5�      $��	10,5,10,5q�5�      $��0q�5�      
�������$��Hidden=�5�      $��18(�5�        5�       �� 
TargetType �� Property �� Value��%�   '��       �� 5�      " ��6   .�5�      ��  � ��
 �����63   �� 5�       � ��
 �   �63   �� 5�       � ��$ 17(�61   �� 5�       �@J�IsHitTestVisible .  69   5�      5�      	 �� View	 5�      �� 5�      .P�5�      �� -   5        columnWorkName��$��	商品名��5�      
 �� Width$	
 100��5�       �� CellTemplate 5�      �� 5�      .R�5�   "   �� 
���   ���5�   %   �� *
WorkName+y�5�   %   �� *	IsVisible+ �� 	Converter# [  5�      5�      �� 5�      -   5        columnJanCode��$��JANコード��5�      $	
 100��5�       5�      �� 5�      .R�5�   "   �� 
���   ���5�   %   �� *	JANCode+y�5�   %   �� *	IsVisible+# [  5�      5�      �� 5�      -	   5        columnOrderCount��$��仕分予定��5     $	
 100��5      5     �� 5     .R�5  "   �� 
���   ���5  %   �� *
OrderCount+y�5  %   �� *	IsVisible+# [  5	     5     �� 5     -
   5        columnOrderCompCount��$��仕分完了��5     $	
 100��5      5     �� 5     .R�5  "   �� 
���   ���5  %   �� *OrderCompCount+y�5  %   �� *	IsVisible+# [  5     5     �� 5     -   5        columnOrderRemainCount��$��残り��5     $	
 100��5      5     �� 5     .R�5  "   �� 
���   ���5   %   �� *OrderRemainCount+y�5"  %   �� *	IsVisible+# [  5#     5%     �� 5&     -   5        columnOrderProgress��$��進捗��5(     $	
 100��5)      5*     �� 5+     .R�5,  "   ]� $��0��5-  %   $��2,0,2,0q�5.  %   $	��200��5/  %   ��.  .+�51  &   �� $ 
lblPercent��52  )   $
��Left=�53  )   $��40��55  )   
���   �$��8,0,1,0q�57  )   $��0,0,0,0q�58  )   $��Right=�59  )   ��54  )   �� *OrderProgress+y�5:  )   �� *	IsVisible+# [  �� 5;  &   $ lblPercentMark��5<  )   $	��％��5=  )   $
��Left=�5>  )   
���   �$��5,0,0,0q�5@  )   $��0,0,10,0q�5A  )   y�5B  )   �� *	IsVisible+# [  � 5C  &    �Name$ progressPercent��5D  )   $��2q�5E  )   $	��100��5F  )   $
��Left=�5G  )    � Minimum$ 0Q�5H  )    � Maximum$	 100Q�5I  )   
�������
����� �
���iii� � Value 5M  )   �� *OrderProgress+y�5N  )   �� *	IsVisible+# [  5O  '   5P  #   5Q     5S     �� 5T     -   5        columnStatus��$��ステータス��5V     $	
 100��5W      5X     �� 5Y     .R�5Z  "   �� 
���   ���5[  %   �� *Status+y�5]  %   �� *	IsVisible+# [  5^     5`     5a     ��5f     �� 5g     " ��6   .�5h     ��  � ��$ 26��6/   �� 5i      � ��$ Center=�6C   �� 5j      � ��
 �����63   �� 5k      � ��$ 0 1 0 0q�68   �� 5l      � ��
 �   �64   �� 5m      � ��$ 0q�6/   �� 5n      � ��$ 20(�61   �� 5o      � ��$ 0q�60   �� 5s      � �� 5t     �� 5v      �� 
TargetType" ��6.   .R�5x  "   �� $ Border��6)   #��z��5y  %   #��z��5z  %   #��z��5{  %    J� SnapsToDevicePixels . ._�5~  &   �� #��z��5  )    �� Content# z��5�  )    �� ColumnCollection �� Columns# z 5�  )   # z 5�  )   5�  #   5�     5�     5�     5�     5�     5�     
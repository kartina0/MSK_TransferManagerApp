   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5   	   #    TransferManagerApp.windowMain   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:TransferManagerApp     5   	   -   5          � Title$  
windowMain��5	   	   $	��900��6   $
��1300��6)    � 
ResizeMode$ NoResize=�5
   	    � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   
�������$��1q�5   	    � 	Resources 5      ��%    5      .��5      �  � RowDefinitions 5   
   �� 5      $��50 �6   �� 5      $	��16* �6   �� 5      $��60 �6   �� 5      $��75 �6   .+�5   
   !   TransferManagerApp.ucHeader  -   5          Name$ ucHeader��5      $��0��5       ]� 5"   
   $��1��5#      ��.  .+�5%      �� -   5         ��Name$ btnOrder��5&      $��	仕分け��5'      $	��Top=�5(      $��30q�5)      $	��600��5*      $	��140��5+      $��40(�5,      
����İ�
���OO/�$��3q�5/       51       �� 
TargetType	 �� Property
 �� Value��%^   '��       �� 52      " ��6   .�53      �� 	 �@��CornerRadius$
 6��69   54      55      56      �� 57      -   5        $ btnRegistry��58      $��照会・登録��59      $	��Top=�5:      $��30q�5;      $	��600��5<      $	��140��5=      $��40(�5>      
����İ�
���OO/�$��3q�5A       5C      ��%^   '��       �� 5D      " ��6   .�5E      �� 	 �@��CornerRadius$
 6��69   5F      5G      5H      �� 5I      -   5        $ btnExit��5J      $��アプリ終了��5K      $��Bottom=�5L      $��30q�5M      $	��600��5N      $	��140��5O      $��40(�5P      
����İ�
���OO/�$��3q�5S       5U      ��%^   '��       �� 5V      " ��6   .�5W      �� 	 �@��CornerRadius$
 6��69   5X      5Y      5Z      5[      !   TransferManagerApp.ucFooter  5]   
   -   5          Name$ ucFooter��5^      $��2��5_      %   TransferManagerApp.ucKeyControl  5a   
   -   5          Name$ ucKeyControl��5b      $��3��5c      5e      5g      
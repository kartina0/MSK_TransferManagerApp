   M S B A M L   `   `   `  ����W  STransferManagerApp_Simulator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:TransferManagerApp_SimulatorTransferManagerApp_Simulator  5   	   5    /TransferManagerApp_Simulator.windowPlcSimulator   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:TransferManagerApp_Simulator     5   	   -   5          � Title$  windowPlcSimulator��5   	   $
��1000��6$   $
��1700��62    � 
ResizeMode$ NoResize=�5	   	    � WindowStyle$
 None=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � ���#303020 � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $	��25* �6   .+�5   
   ]� $��0��5      $��0q�5      ��.  .+�5      �� -   5         ��Name$ lblTitle��5      $��PLCシミュレーター��5      $
��Left=�5      $��18(�5      ���#DDD78B$��0��5      �� 5       -   5         ��Name$ btnClose��5!      $��	閉じる��5"      $��Right=�5#      $��20,2,2,2q�5$      $��80��5%      
�������$��14(�5'      $��0q�5(      5)      �� 5+      $��20,2,2,2q�5,      $��Right=�5-       �� Orientation$ 
Horizontal=�5.      .+�5/      �� -   5        $ btnAuto��50      $��運転��51      $��5,0,5,0q�52      $��80��53      
�������$��14(�55      $��0q�56      57      �� 58      -   5        $ btnStop��59      $��停止��5:      $��5,0,5,0q�5;      $��80��5<      
��� ���$��14(�5>      $��0q�5?      5@      5A      �� 5C      $��20,2,2,2q�5D      $��Right=�5E      $ 
Horizontal=�5F      .+�5G      �� -   5        $ btnReadWork��5H      $��商品読み込み��5I      $��Right=�5J      $��5,0,5,0q�5K      $	��100��5L      
�������$��14(�5N      $��0q�5O      5P      �� 5Q      -   5        $ btnClear��5R      $��	クリア��5S      $��Right=�5T      $��5,0,5,0q�5U      $	��100��5V      
�������$��14(�5X      $��0q�5Y      5Z      5[      5]      � 5a   
   -   5         �Name$ gridWork��5b      $��1��5c      	 � ColumnDefinitions	 5d      �� 5e      $��* �6#   �� 5f      $��* �6#   �� 5g      $��* �6#   �� 5h      $��* �6#   �� 5i      $��* �6#    5k      �� 5l      $��* �6    �� 5m      $��* �6    5�      5�      5�      
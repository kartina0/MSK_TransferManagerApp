   M S B A M L   `   `   `  ����M  ITransferManagerApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TransferManagerAppTransferManagerApp  5   	   &     TransferManagerApp.windowOpening   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:TransferManagerApp     5   	   -   5        $	��250��5	   	   $	��500��6     � WindowStartupLocation$  CenterScreen=�5
   	    � WindowStyle$
 None=�5   	    � 
ResizeMode$ NoResize=�5   	    � AllowsTransparency . 
������  J� Opacity$	 1.0Q�5   	   .��5      �  J� RenderTransformOrigin$ 0.482,0.664&�6   ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W   .��5      � 	 � Color$	 	#FF353544��6   
 � Offset$
 1Q�61   � 5      $	 	#FF0A0A2B��6   5       � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��2* �6   �� 5      $��* �6   .+�5"   
   � $��0��6   $��5,1,0,1q�5#       � ColumnDefinitions 5$      �� 5%      $��* �6#   �� 5&      $��7* �6#   .+�5)      �� $��0��5*      $��0,2,0,2q�5+      $'��!/Resources/TransferManagerApp.ico��5,      ]� 5-      $��1��6   ��.  .+�5/      �� -   5         ��Name$ lblAppTitle��50      $
��Left=�51      $��Center=�52      $��30(�53      
�������54      55      56      � 59   
   $��1��6    5:      �� 5;      $��* �6#   �� 5<      $��7* �6#   .+�5?      ]� $��1��5@      $��10,0,0,0q�5A      ��.  .+�5C      �� -   5        $ 
lblVersion��5D      $
��Left=�5E      $	��Top=�5F      $��15(�5G      
�������5H      5I      5J      � 5M   
   $��3��6   $��5q�5N       5O      �� 5P      $��6* �6#   �� 5Q      $��* �6#   �� 5R      $��2* �6#   .+�5]      �� $��1��5^      $��4q�5_      ._�5`      (   "TransferManagerApp.ucWaitingCircle  -   5          Name$ ucWaitingCircle��5a      5b      �� 5c      -   5        $ 
lblMessage��5d      $��2��5e      $��Center=�5f      $
��Left=�5g      $��18(�5h      
�������5i      5k      5l      5m      
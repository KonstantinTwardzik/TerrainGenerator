   M S B A M L   `   `   `  ����H  DTopographer3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Topographer3D.ViewsTopographer3D.Views  5   	   C&clr-namespace:Topographer3D.ViewModelsTopographer3D.ViewModels  5   	   $    Topographer3D.Views.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3views!clr-namespace:Topographer3D.Views     5   	   =
ViewModels&clr-namespace:Topographer3D.ViewModels     5   	     � WindowStyle$
  None=�5	   	    � 
ResizeMode$ CanResizeWithGrip=�5
   	    � AllowsTransparency .    PrimaryColor#��[  5   	    � Title$ Topographer 3D��5   	   $
��1280��6|   $	��600��6�   $	��800��6�   $��1q�5   	     SecondaryColor#��[ 5   	   ��5       �� *LayerManager.ViewHeight+ �� Mode$ OneWayToSource=� �� NotifyOnTargetUpdated .  � 	Resources 5       �� 
TargetType �� Property	 �� Value  TertiaryColor��%�   '��       7  7  �� 5   
   " ��6   .�5      ��  � ��$	 1q�60   �� 5       � ��8	   6,   �� 5       � ��8	  6+   5      5      
 � DataContext
 5      ,  &Topographer3D.ViewModels.MainViewModel  5   
    J� InputBindings 5      �� 5   
    �� 	Modifiers$
 Ctrlf�6   
 �� Key$ Q��6&    �� Command 6.   �� *QuitCommand+�� 5   
   $
 Ctrlf�6   $ N��6&    6.   �� *
NewCommand+�� 5    
   $
 Ctrlf�6   $ E��6&    6.   �� *ExportCommand+�� 5!   
   $
 Ctrlf�6   $ D1��6&    �� CommandParameter$ 16��6[    6/   �� *DetailResolutionCommand+6[   �� 5"   
   $
 Ctrlf�6   $ D2��6&   $ 32��6[    6/   �� *DetailResolutionCommand+6[   �� 5#   
   $
 Ctrlf�6   $ D3��6&   $ 64��6[    6/   �� *DetailResolutionCommand+6[   �� 5$   
   $
 Ctrlf�6   $ D4��6&   $	 128��6[    6/   �� *DetailResolutionCommand+6[   �� 5%   
   $
 Ctrlf�6   $ D5��6&   $	 512��6[    6/   �� *DetailResolutionCommand+6[   �� 5&   
   $
 Ctrlf�6   $ D6��6&   $
 1024��6[    6/   �� *DetailResolutionCommand+6[   �� 5'   
   $
 Ctrlf�6   $ D7��6&   $
 2048��6[    6/   �� *DetailResolutionCommand+6[   �� 5(   
   $
 Ctrlf�6   $ D8��6&   $
 4096��6[    6/   �� *DetailResolutionCommand+6[   .��5+      � #��[  6    � ColumnDefinitions 5,   
   �� 5-      $��* �6   �� 5.      $	��350��6   $��0.25* �6.    � RowDefinitions 50   
   �� 51      $��35 �6   �� 52      �� 53      $	��218 �6   �� 54      $��25 �6   .+�57   
   �� $��0��6   $��0��6!   $��2��6.   $��0q�6B   $ ��Topographer3D - Terrain_01��6V    58      d� 59       d� Gesture$ LeftDoubleClicka�6    69   �� *MaximizeCommand+d� 5:      $ 	LeftClicka�6    63   �� *DragCommand+��5<      �� 5=      " ��6   .�5>      ��  � ��
	 ���� 63   �� 5?       � ��	 5@      �� 5A       �� 
TargetType" ��6.   .R�5B   "   �� #��z��6)   ._�5C   &   �� $��Center=�67   $��Center=�6T   5D   #   5E      5G       �� Triggers 5H      P� 5I       P� Property �@J�IsMouseOver P� Value . . �5J      ��  � ��
	 ���� 6;   5K      5M      5O      �� 5S   
   $��0��6   $��0��6%   $
��Left=�62    �� Orientation$ 
Horizontal=�6M   $��5,5,0,2q�6f   .+�5T      �� $D��>pack://application:,,,/Topographer3D;component/Assets/TG32.png��6   $��40��6\   !   Topographer3D.Views.MenuBar  5U      5V      �� 5Z   
   $��1��6   $��0��6%   $��Right=�62   $ 
Horizontal=�6N   .+�5[      �� $��40��6   $��0q�6    ��64   �� *MinimizeCommand+.��5\      �� $"��../Assets/Icons/Minimize.png��6   ��5]      �� 5^      " ��6   .�5_      ��  � ��
	 ���� 67   �� 5`       � ��	 5a      �� 5b   "   " ��62   .R�5c   &   �� #��z��6-   ._�5d   *   �� $��Center=�6;   $��Center=�6X   5e   '   5f   #   5h       5i      P� 5j       �@J�IsMouseOver . . �5k   "   ��  � ��  LighterBlue#	 [ 6?   5l      5n      5p      �� 5q      $��40��6   $��0q�6    ��64   �� *MaximizeCommand+.��5r      �� ��6   �� *MaxImagePath+��5s      �� 5t      " ��6   .�5u      ��  � ��
	 ���� 67   �� 5v       � ��	 5w      �� 5x   "   " ��62   .R�5y   &   �� #��z��6-   ._�5z   *   �� $��Center=�6;   $��Center=�6X   5{   '   5|   #   5~       5      P� 5�       �@J�IsMouseOver . . �5�   "   ��  � ��#	 [ 6?   5�      5�      5�      �� 5�      $��40��6   $��0q�6    ��64   �� *QuitCommand+.��5�      �� $��../Assets/Icons/Quit.png��6   ��5�      �� 5�      " ��6   .�5�      ��  � ��
	 ���� 67   �� 5�       � ��	 5�      �� 5�   "   " ��62   .R�5�   &   �� #��z��6-   ._�5�   *   �� $��Center=�6;   $��Center=�6X   5�   '   5�   #   5�       5�      P� 5�       �@J�IsMouseOver . . �5�   "   ��  � ��	 �#e811236?   5�      5�      5�      5�      �� 5�   
   $��0��6   $��1��6!   $��1��6.   $��1��6B   $��4,0,2,2q�6S   ._�5�      "   Topographer3D.Views.Viewport  5�      �� 5�   
   $��1��6   $��1��6"   $��1��6/   $��2��6C   $��2,0,4,0q�6T   ._�5�      &    Topographer3D.Views.LayerDetails  5�      �� 5�   
   $��0��6   $��2��6!   $��4,2,2,0q�6.   ._�5�      �� $��Visible=�6   $��Hidden=�6C   .��5�      &    Topographer3D.Views.LayerManager  5�      5�      �� 5�   
   $��0��6   $��3��6$   $��1��61   $��25��6E   $��5,2,0,3q�6Q   ��6b   �� *LayerManager.StatusBarText+��6�   �� *LayerManager.StatusBarColor+5�      5�      
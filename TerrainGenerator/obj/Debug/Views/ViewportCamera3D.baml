   M S B A M L   `   `   `  ����K  GTerrainGenerator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:Topographer.ViewModelsTopographer.ViewModels  5      (    "Topographer.Views.ViewportCamera3D   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;
ViewModels$clr-namespace:Topographer.ViewModels     5      .��5
      �   � ColumnDefinitions  5   
   �� 5      $��* �6   �� 5      $��30 �6    � RowDefinitions 5   
   �� 5      $��* �6   .+�5   
   '� -   5         '�Name$ viewport��5      $��0��6'   $��0��67   $��2��6D    '� Camera 5      =� 5      -   5        
camera�� � FarPlaneDistance$ 50Q�5   4    � NearPlaneDistance$ 0Q�6J    � LookDirection$ 0,0,-105�6`    � UpDirection$ 0,1,05�6x    � Position$ 0,0,51�6�   	 =� FieldOfView$	 45Q�6�   .x�5      g� 
 g� Content
 5      h� 5      .��5      t�  �� Color$ 
WhiteSmoke��6+    t� 	Direction$ 
-1, -1, -35�6>   t� 5      $ 	AliceBlue��6+   $ 
1.4,-0.5,25�6=   � 5        � Geometry 6*   �� *31TerrainMeshProperty.TerrainMeshGeometry3DProperty+ � Material 5!      u� 5"   "    u� Brush 62   �� *'%TerrainMeshProperty.TerrainImageBrush+5$      � 5&       6*   �� *20TerrainMeshProperty.BorderMeshGeometry3DProperty+ 5'      u� 5(   "    62   �� *&$TerrainMeshProperty.BorderImageBrush+5*      5,      5.      5/      �� 51   
   -   5         ��Name$ canvas��51      
������ $��0��6�   $��0��6�   $��2��6�   �� 53   
   $	��Top=�6   $��1��6-   $��0��6=   $��0,2,0,0q�6J   .+�54      �� -   5        
������ $��0q�54   .   $��3q�6B    � ToolTip$ Perspective View��6k   .��55      �� $)��#../Assets/Icons/perspectiveView.png��6   56      �� 57      -   5        
������ $��0q�57   .   $��3q�6B   $ 	Side View��6d   .��58      �� $"��../Assets/Icons/sideView.png��6   59      �� 5:      -   5        
������ $��0q�5:   .   $��3q�6B   $ Top View��6c   .��5;      �� $!��../Assets/Icons/topView.png��6   5<      5=      5?      5@      
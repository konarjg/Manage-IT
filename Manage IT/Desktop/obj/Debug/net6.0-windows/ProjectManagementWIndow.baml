   M S B A M L   `   `   `  ����B  >Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DesktopDesktop  5   	   %    Desktop.ProjectManagementWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Desktop     5   	   -   5          � Title$  	Manage IT��5   	   $
��2560��6   $
��1080��6(    � WindowState$ 	Maximized=�66    � 	Resources 5
         ProjectButton �� 
TargetType �� Property �� Value  SharedProjectButton��%,  &	        &	     �� 5   
   " ��6&   .�5      ��  � ��$ 72(�6)   �� 5       � ��$	 350��6*   �� 5       � ��$	 600��6&   �� 5       � �� �#a0a0ff6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0,0,30,0q�6'   5      �� 5   
   " ��6,   .�5      ��  � ��$ 72(�6)   �� 5       � ��$	 350��6*   �� 5       � ��$	 600��6&   �� 5       � �� �#a0a0a06+   �� 5       � ��
 �����6+   �� 5       � ��$ 0,0,30,0q�6'   5      5      .��5      � ��5   
   �� 5        �� 
StartPoint$ 0.5, 0&�6"    �� EndPoint$ 0.5, 1&�66   .��5!      �  � Color$ #000000��6   	 � Offset$		 0.0Q�6/   � 5"      $ #000040��6   $
	 0.75Q�6/   5#      
 � RowDefinitions
 5%   
   �� 5&      $��* �6   �� 5'      $��* �6   �� 5(      $��* �6   �� 5)      $��* �6   �� 5*      $��* �6   �� 5+      $��* �6   �� 5,      $��* �6   �� 5-      $��* �6   �� 5.      $��* �6   �� 5/      $��* �6   �� 50      $��* �6   �� 51      $��* �6   �� 52      $��* �6   �� 53      $��* �6   �� 54      $��* �6   �� 55      $��* �6    � ColumnDefinitions 58   
   �� 59      $��* �6   �� 5:      $��* �6   �� 5;      $��* �6   �� 5<      $��* �6   �� 5=      $��* �6   �� 5>      $��* �6   .+�5A   
   �� -   5        $��⚙️ Settings��5A      $��0q�6=   #��[" �6Q   
������ 
�������$��40(�6�   $��0��6�   $��1��6�   $��5��6�   $��1��6�   6
  �� 5B   
   -   5        $��	💬 Chat��5B      $��0q�65   #��[" �6I   
������ 
�������$��40(�6�   $��0��6�   $��1��6�   $��4��6�   $��1��6�   6  �� 5C   
   -   5        $��📅 Calendar��5C      $��0q�6=   #��[" �6Q   
������ 
�������$��40(�6�   $��0��6�   $��1��6�   $��3��6�   $��1��6�   6
  �� 5D   
   
�������$��4q�6%   $��3��69   $��1��6F   $��4��6V   $��	0,0,10,10q�6j   $��11��6}   ._�5E      �� $
}�True��6   $��Center=�6/   $��50, 0, 60, 0q�6L   $��Disabled=�6b   $
��Auto=�6�   .��5F      �� -   5         ��Name$ ProjectList��5F       �� Orientation$ 
Horizontal=�62   $��50q�6K   .+�5G      �� -   5        ���#a0a0ff
�������$��✚ Create Project��5G   E   $��72(�6{   $	��350��6�   $	��600��6�   5H      5I      5J      �� 5K   
   -   5         ��Name$ Header��5K      $��Center=�6$   $	��Top=�6A   $��1��6Y   $��6��6f   $��2��6z   $
��Bold"�6�   $��Your projects��6�   $��58(�6�    �� TextAlignment$ Center=�6�   
�������6�   5M      5N      
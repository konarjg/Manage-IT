   M S B A M L   `   `   `  ���� B  >Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DesktopDesktop  %    Desktop.ProjectManagementWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:Desktop     -     � Title$  	Manage IT��$
��2560��$
��1080�� � WindowState$ 	Maximized=� � 	Resources    ProjectButton �� 
TargetType �� Property �� Value  SharedProjectButton��%V  &	        &	 �     �� " ��.���  � ��$ 72(���  � ��$	 350����  � ��$	 600����  � �� �#a0a0ff��  � ��
 �������  � ��$ 0,0,30,0q��� " ��.���  � ��$ 72(���  � ��$	 350����  � ��$	 600����  � �� �#a0a0a0��  � ��
 �������  � ��$ 0,0,30,0q�.��� ����  �� 
StartPoint$ 0.5, 0&� �� EndPoint$ 0.5, 1&�.���  � Color$ #000000��	 � Offset$		 0.0Q�� $ #000040��$
	 0.75Q�
 � RowDefinitions
 �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* � � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $��⚙️ Settings��$��0q�#��[" �
������ 
�������$��40(�$��0��$��1��$��5��$��1���� -   $��	💬 Chat��$��0q�#��[" �
������ 
�������$��40(�$��0��$��1��$��4��$��1���� -   $��📅 Calendar��$��0q�#��[" �
������ 
�������$��40(�$��0��$��1��$��3��$��1���� 
�������$��4q�$��3��$��1��$��4��$��	0,0,10,10q�$��11��._��� $
}�True��$��Center=�$��50, 0, 60, 0q�$��Disabled=�$
��Auto=�.���� -    ��Name$ ProjectList�� �� Orientation$ 
Horizontal=�$��50q�.+��� -   ���#a0a0ff
�������$��✚ Create Project��$��72(�$	��350��$	��600���� -    ��Name$ Header��$��Center=�$	��Top=�$��1��$��6��$��2��$
��Bold"�$��Your projects��$��58(� �� TextAlignment$ Center=�
�������
   M S B A M L   `   `   `  ���� B  >Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DesktopDesktop  !    Desktop.CreateProjectWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:Desktop       � Title$  	Manage IT��$
��2560��$
��1080�� � WindowState$ 	Maximized=�.��� ����  �� 
StartPoint$ 0.5, 0&� �� EndPoint$ 0.5, 1&�.���  � Color$ #000000�� � Offset$	 0.0Q�� $ #000040��$
 0.75Q� � RowDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* � � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -    ��Name$ 
BackButton��$	��120(�$��0��$��0��
������ $
��🔙��$��0q�
�������$��4��$��
0,0,120,66q��� -   	 ��Name$	 Header��$��Center=�$	��Top=�$��1��$��6��$��2��$
��Bold"�$��Create Project��$��58(�
 �� TextAlignment$
 Center=�
��������� -   $	 Error��$��Center=�$	��Top=�$��2��$��2��$��36(�$
 Center=�
���  ��$��2��$��0,15,0,0q��� $
��Name��
�������$��3��$��2��$��48(�$��2���� -    ��Name$ ProjectName��$��4��$��2��$��2��$��48(� �� TextAlignment$
 Left=�
������ 
�������
������� �� 
CaretBrush
 �����$��5q��� $��Description��
�������$��5��$��2��$��48(�$��2���� $��Disabled=�$
��Auto=�$��6��$��6��$��2��$��2��.���� -   $ Description��$��48(� �� MaxLines$ 30�� �� AcceptsReturn$
 True��
������ 
�������
�������$��5q�
 ����� �� TextWrapping$
 Wrap=��� -   $ CreateButton��$��Create��$��12��$��2��$��42(�$��2��$��2��$��214,32,214,34q�
���   �
�������
���   �$��5q�
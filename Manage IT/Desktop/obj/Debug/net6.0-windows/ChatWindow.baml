   M S B A M L   `   `   `  ����B  >Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DesktopDesktop  5   	       Desktop.ChatWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Desktop     5   	     � Title$  
ChatWindow��5   	   $
��2560��6   $
��1080��6)    � WindowState$ 	Maximized=�67    � 	Resources 5	         Message �� TextWrapping" �� VerticalScrollBarVisibility �� 
IsReadOnly  ConversationItem � RowDefinitions � ColumnDefinitions ��Name
	 � Tag��%�  &	        &	 �     �� 5
   
   .R�5      �� $	��700��6,   $	��150��68   $��10q�6E   $��36(�6Q   
�������$
 Wrap=�6r   $��5q�6�   $ Visible=�6�   $
 True��6�   ��6   �� *Body+��6�   �� *Color+6�   5      �� 5   
   .R�5      � $	��350��6   $��50��6   $��	10,0,10,0q�6+    5      �� 5      $��* �6$    5      �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   .+�5      �� $ ConversationName��6   $��5��6a   
�������$��36(�6�   $��CharacterEllipsis=�6�   	 66   �� *Name+��6K   �� *Name+6�   �� 5      -   5        $��7��5   Q   $	��❌��6a   $��20(�6m   $��5q�6{   $
��Bold"�6�   ���#ffa0a0���#800000$��2q�6�   ���#800000	 64   �� *Conversation+6�   5       5!      5"      .��5$      � ��5%   
   �� 5&      
 �� 
StartPoint$
 0.5, 0&�6"    �� EndPoint$ 0.5, 1&�66   .��5'      �  � Color$ #000000��6    � Offset$	 0.0Q�6/   � 5(      $ #000040��6   $
 0.75Q�6/   5)       5,   
   �� 5-      $��* �6   �� 5.      $��* �6   �� 5/      $��* �6   �� 50      $��* �6   �� 51      $��* �6   �� 52      $��* �6   �� 53      $��* �6   �� 54      $��* �6   �� 55      $��* �6   �� 56      $��* �6   �� 57      $��* �6   �� 58      $��* �6   �� 59      $��* �6   �� 5:      $��* �6   �� 5;      $��* �6   �� 5<      $��* �6    5?   
   �� 5@      $��* �6   �� 5A      $��* �6   �� 5B      $��* �6   �� 5C      $��* �6   �� 5D      $��* �6   �� 5E      $��* �6   .+�5H   
   �� -   5         ��Name$ 
BackButton��5H      $	��120(�67   $��0��6F   $��0��6S   
������ $
��🔙��6|   $��0q�6�   
�������$��4��6�   $��
0,0,120,66q�6�   �� 5I   
   
�������$��5q�6%    �� CornerRadius$ 10��69   $��2��6K   $��12��6X   $��1��6j   $��4��6z   ._�5J      � $��10q�6    5K      �� 5L      $��* �6$   �� 5M      $��* �6$   �� 5N      $��* �6$   �� 5O      $��* �6$   �� 5P      $��* �6$   �� 5Q      $��* �6$   �� 5R      $��* �6$   �� 5S      $��* �6$   �� 5T      $��* �6$   �� 5U      $��* �6$   �� 5V      $��* �6$   �� 5W      $��* �6$   �� 5X      $��* �6$   �� 5Y      $��* �6$   �� 5Z      $��* �6$   �� 5[      $��* �6$    5^      �� 5_      $��* �6'   �� 5`      $��* �6'   �� 5a      $��* �6'   �� 5b      $��* �6'   �� 5c      $��* �6'   �� 5d      $��* �6'   .+�5g      � $��14��6   $��2��6%   $��4��66    5h      �� 5i      $��* �6+   �� 5j      $��* �6+   �� 5k      $��* �6+   �� 5l      $��* �6+   �� 5m      $��* �6+   �� 5n      $��* �6+   �� 5o      $��* �6+   �� 5p      $��* �6+   .+�5s      � $��2q�6   $��6��6&   .+�5t      �� -   5         ��Name$ 
MessageBox��5t   "   $��6,0,6,0q�6]   
������  �� 
CaretBrush
 �����
�������
�������$��2q�6�   $��32(�6�   $��Arial)�6�   $ Visible=�6�   {�66   �� *MessageBoxActive+6  �� 5u      -   5        $ MessageBoxPlaceholder��5u   $   $��2q�6C    �� Padding$ 6,0,6,0q�6N   
������ 
�������$��32(�6�   $��Arial)�6�   $��Type a message...��6�    J� IsHitTestVisible$ False��6�   5v      �� 5x      -   5        $ SendMessageButton��5x      $��6��6s   $��2q�6�   $	��➤��6�   $��48(�6�   ���#a0a0ff���#000080���#000080$��2q�6�   {�68   �� *MessageBoxActive+ 5y       �� 
TargetType �� Property �� Value��%i   '��       �� 5z      " ��6$   .�5{   "   ��  �@��CornerRadius$ 0,5,5,0��6A   6S   5|      5}      5~      5      �� 5�      $��Visible=�6   $��0��6E   $��4��6R   $��14��6f   $��5q�6x   .��5�      �� -   5        #��[  5�   ?   
������ ��6   �� *
Messages+5�      5�      �� 5�      $��0��6   $��4��6&   $��2��66   $��16��6J   
�������$��3q�6p   $ 10��6�   ._�5�      � $��5q�6    5�      �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,   �� 5�      $��* �6,    5�      �� 5�      $��* �6/   .+�5�      �� $��Conversations��6$   
�������$��36(�6L   $ 5,0,5,0q�6Z   $
��Bold"�6l   �� 5�      $��1��6'   $��14��64   $��Visible=�6F   $��Disabled=�6l   .��5�      �� -   5         ��Name$ Conversations��5�   &   $ �� IsSynchronizedWithCurrentItem$ FalseY�6=   #��[ 6�   
������ ��6c   �� *Conversations+ �� SelectedItem 6�   �� *ActiveConversation+ �� Mode$ TwoWay=�5�      5�      � 5�      $��15��6   $��0q�6-    5�      �� 5�   "   $��* �63   �� 5�   "   $��* �63   �� 5�   "   $��* �63   �� 5�   "   $��* �63   �� 5�   "   $��* �63   �� 5�   "   $��* �63   .+�5�      � $��5��6#   .+�5�   "   �� -   5        $ 
Credential��5�   *   
������ 
 �����
�������$��2q�6~   
�������$��5,0,5,0q�6�   $��18(�6�   6�   �� 5�   "   -	   5        $ CredentialPlaceholder��5�   ,   $��Email or username��6K   
������ 
�������$��5,0,5,0q�6�   $��18(�6�   $ False��6�   6�   5�      �� 5�      -
   5        $��6��5�   @   $	��0.5q�6P   ���#a0a0ff���#000080���#000080$��1q�6�   $
��📩��6�   $��24(�6�   6�   5�      5�      5�      5�      5�      �� 5�   
   -   5         ��Name$ 
ErrorPopup��5�      $y�	Collapsed=�6%   
�������$��5q�6P   $��5��6d   $��2��6q   $��2��6�   $��6��6�   ._�5�      � ��5�      �� 5�      $
 0.5, 0&�6*   $ 0.5, 1&�6>   .��5�      � $ #000000��6'   $	 0.0Q�67   � 5�      $ #000040��6'   $
 0.75Q�67   5�       5�      �� 5�      $��* �6$   �� 5�      $��* �6$   �� 5�      $��* �6$    5�      �� 5�      $��* �6'   �� 5�      $��* �6'   �� 5�      $��* �6'   .+�5�      �� -   5        $ Error��5�      $$��There was an unexpected error!��6+   
���  ��$��36(�6b    �� TextAlignment$ Center=�6p   $��3��6�   $��10q�6�   $ 5q�6�   6�   �� 5�      -   5        $ 
CloseError��5�      $��1��6E   $��1��6R   $��1��6b   $��Ok��6v   
�������$��48(�6�   $
��Bold"�6�   5�      5�      5�      5�      
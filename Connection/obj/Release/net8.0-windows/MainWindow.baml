   M S B A M L   `   `   `  ����   
Connection    Connection.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  
Connection��$	��614��$	��819��$	��600��$	��800�� � WindowStartupLocation$ CenterScreen=�
���   � � 	Resources    MainMenuButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  
TitleStyle3  -System.Windows.Media.Effects.DropShadowEffect   Color   
BlurRadius   ShadowDepth   Opacity��%  &	        &	 T    �� " ��.���  � ��$	 300����  � ��$ 60����  � ��$
 0,10q���  � ��$ 20(���  � ��$
 Bold"���  � �� �#2C3E50��  � ��
 �������  � �� �#34495E��  � ��$ 2q���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��#��z��#��z��$ 5��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver$

 True��. ���  � �� �#34495E��  � �� �#3498DBP� 	 � ��$

 True��. ���  � �� �#1ABC9CP� 	 � {�$
 False��. ���  � �� �#7F8C8D��  � �� �#BDC3C7�� " ��.���  � ��$ 72(���  � ��$
 Bold"���  � ��
 �������  � ��$ Center=���  �@J�Effect   $
 Blue��$ 10Q�$ 3Q�$	 0.7Q�.��� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ #0F0F23�� � Offset$ 0Q�� $ #1A1A2E��$	 0.5Q�� $ #16213E��$ 1Q� � RowDefinitions �� $��* ��� $
��Auto ��� $��* �.+��� $��0��$��
Connection��#��[ $��Bottom=�$��0,0,0,50q��� $��1��$��Center=�$��Center=�.+��� #��[  ���� *NewGameButtonText+���� *NewGameCommand+�� #��[  ���� *ContinueButtonText+���� *ContinueGameCommand+{��� *CanContinueGame+�� #��[  ���� *SettingsButtonText+���� *ShowSettingsCommand+�� #��[  ���� *ExitButtonText+���� *ExitGameCommand+� $��2��$��Bottom=�$��20q� � ColumnDefinitions �� $��* ��� $��* �.+��� $��0��
�������$��12(�$
��Left=�$��Bottom=����� *VersionText+�� $��1��
�������$��12(�$��Right=�$��Bottom=����� *LastPlayTimeText+
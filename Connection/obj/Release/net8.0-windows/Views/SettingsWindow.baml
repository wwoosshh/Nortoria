   M S B A M L   `   `   `  ����   
Connection%    Connection.Views.SettingsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $	��600��$	��800��  � WindowStartupLocation$  CenterOwner=� � 
ResizeMode$ NoResize=����#2C3E50 � Title �� *SettingsWindowTitle+ � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName   SettingHeaderStyle  SettingControlStyle��%g  '��       &	  �    &	     �� " ��.���  � �� �� " ��.R��� $��Border�����#34495E���#2C3E50$��1,1,1,0q�$ 5,5,0,0��$	��2,0q�._��� $	 ContentSite��$��Center=�$��Center=�$��Header��$
��10,5q�
 P�  �@��
IsSelected$
 True��. ��� $ Border�� � �� �#3498DBP�  �@J�IsMouseOver$
 True��. ��� $ Border�� � �� �#5DADE2��  � ��
 �������  � ��$
 Bold"��� " ��.���  � ��$ 14(���  � ��$
 Bold"���  � ��
 �������  � ��$ 0,10,0,5q��� " ��.���  � ��$	 0,5q���  � ��$ 30��.��� $��20q� � RowDefinitions �� $��* ��� $
��Auto �.+��� $��0�����#34495E���#2C3E50.@��� ���� *GraphicsTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *DisplayModeText+�� #��[  �� SelectedItem �� *DisplayMode+���� *DisplayModes+�� #��[  ���� *FrameRateText+�� #��[  �� *	FrameRate+���� *
FrameRates+�� #��[  ���� *GraphicsQualityText+�� #��[  �� *GraphicsQuality+���� *GraphicsQualities+�� #��[  ���� *WindowSizeText+�  � ColumnDefinitions �� $��* ��� $
��Auto ��� $��* �.+��� $��0��#��[ ���� *WindowWidth+�� $��1��$
�� × ��
�������$��Center=�$	��5,0q��� $��2��#��[ ���� *WindowHeight+�� ���� *ControlsTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *KeySettingsText+�� ���� *KeyBindings+���� .R�� $	��0,5q� �� $	��150 ��� $��* ��� $	��100 �.+��� $��0��
�������$��Center=����� *
ActionName+�� $��1�� �� 
IsReadOnly$
 True��#��[ ���� *	KeyName+�� $��2��#��[ ���� *DataContext.ChangeText+ �� RelativeSource ��  �� AncestorType
 Window���� *ChangeKeyCommand+�� ���� *AudioTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *MasterVolumeText+�  �� $��* ��� $��50 �.+��� $��0�� � Minimum$ 0Q� � Maximum$ 1Q� �� TickFrequency$	 0.1Q� �� IsSnapToTickEnabled$
 True��$��Center=� � Value �� *MasterVolume+�� $��1��
�������$��Center=�$��Center=����� *MasterVolumePercent+�� #��[  ���� *	MusicText+�  �� $��* ��� $��50 �.+��� $��0��$ 0Q�$ 1Q�$	 0.1Q�$
 True��$��Center=� �� *MusicVolume+�� $��1��
�������$��Center=�$��Center=����� *MusicVolumePercent+�� #��[  ���� *	VoiceText+�  �� $��* ��� $��50 �.+��� $��0��$ 0Q�$ 1Q�$	 0.1Q�$
 True��$��Center=� �� *VoiceVolume+�� $��1��
�������$��Center=�$��Center=����� *VoiceVolumePercent+�� #��[  ���� *EffectsText+�  �� $��* ��� $��50 �.+��� $��0��$ 0Q�$ 1Q�$	 0.1Q�$
 True��$��Center=� �� *EffectVolume+�� $��1��
�������$��Center=�$��Center=����� *EffectVolumePercent+�� 
�������$��0,20,0,0q����� *
MuteText+ d� 	IsChecked �� *	IsMuted+�� ���� *LanguageTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *GameLanguageText+�� #��[  �� *GameLanguage+���� *	Languages+�� #��[  ���� *VoiceLanguageText+�� #��[  �� *VoiceLanguage+���� *	Languages+�� ���� *ResourcesTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *ResourceManagementText+�� 
�������$
��0,10q����� *AutoDownloadText+ �� *AutoDownload+�� 
�������$
��0,10q����� *AutoDeleteText+ �� *
AutoDelete+�� #��[  ���� *DownloadQualityText+�� #��[  �� *DownloadQuality+���� *ResourceQualities+�� #��[  ���� *CacheSizeLimitText+�� #��[ ���� *CacheSizeGB+�� #��[ $��0,20,0,0q����� *ClearCacheText+���� *ClearCacheCommand+�� #��[ ���� *RedownloadResourcesText+���� *RedownloadResourcesCommand+�� ���� *MiscTabText+.���� $
��Auto=�.���� $��20q����#34495E.+��� #��[  ���� *GameInfoText+�� $��Connection v1.0.0��
�������$	��0,5q��� 
�������$	��0,5q��� ��� *DeveloperText+ �� Mode$ OneWay=� �� $�: Connection Team���� 
�������$	��0,5q��� ��� *
EngineText+$ OneWay=� �� $�: WPF (.NET)���� #��[  ���� *CopyrightInfoText+�� $3��-© 2025 Connection Team. All rights reserved.��
�������$
��Wrap=�$	��0,5q��� #��[ $��0,30,0,0q����� *ResetToDefaultText+���� *ResetToDefaultCommand+� $��1��$��0,20,0,0q� �� $��* ��� $	��100 ��� $	��100 �.+��� $��1��$��35��$	��5,0q����� *SaveButtonText+���� *SaveCommand+�� $��2��$��35��$	��5,0q����� *CancelButtonText+���� *CancelCommand+
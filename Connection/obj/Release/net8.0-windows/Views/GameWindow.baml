   M S B A M L   `   `   `  ����   
Connection!    Connection.Views.GameWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  
Connection��$	��614��$	��819�� � WindowStartupLocation$ CenterScreen=�
���   �$
|�True�� � 	Resources    DialogueBoxStyle �� 
TargetType �� Property �� Value �� 
StartPoint �� EndPoint �� Opacity	 � Color
 � Offset  SpeakerNameStyle3  -System.Windows.Media.Effects.DropShadowEffect   Color   
BlurRadius   ShadowDepth   Opacity  DialogueTextStyle  ContinuePromptStyle �� Triggers :� RoutedEvent g� RepeatBehavior �� TargetProperty W� From	 W� To g� Duration g� AutoReverse  ChoiceButtonStyle �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  MenuButtonStyle  UIOverlayTextStyle��%�  &	        &	     &	 �    &	 }    &	 �    &	 �    &	 �    �� " ��.���  � �� �� $	 0,0&�$	 0,1&�$	 0.9Q�.��� $	 	#80000000��$
 0Q�� $	 	#A0000000��$
 1Q���  �@��CornerRadius$ 15����  �@��Padding$ 25q���  � ��$ 
50,0,50,50q���  � �� �	#40FFFFFF��  � ��$ 1q��� " ��.���  � ��$ 20(���  � ��$
 Bold"���  � �� �#3498DB��  � ��$ 0,0,0,15q���  �@J�Effect   $ Black��$ 3Q�$ 1Q�$	 0.7Q��� " ��.���  � ��$ 18(���  � ��
 �������  � ��$
 Wrap=���  �@��
LineHeight$ 28����  �@J�Effect   $ Black��$ 2Q�$ 1Q�$	 0.8Q��� " ��.���  � �� �#BDC3C7��  � ��$ 14(���  � ��$ Right=���  � ��$ 0,15,0,0q���  �@J�Opacity$	 0.8Q� :� $ Loaded��.[��� .���� $ Forever��.��W� $ Opacity� 0.3 1.0$ 0:0:1D�$
 True���� " ��.���  � ��$	 200����  � ��$	 600����  � ��$ 50����  � ��$ 10q���  � ��$ 20,10q���  � ��$ 16(���  � ��$ Medium"���  � �� �#2C3E50��  � ��
 �������  � �� �#34495E��  � ��$ 2q���  �@�Cursor$
 Hand����  � ��$ Center=���  � ��$ Center=���  � �� �� " ��.R��� #��z��#��z��#��z��$ 8��._��� #��z��#��z��#��z�� P�  �@J�IsMouseOver$
 True��. ���  � �� �#34495E��  � �� �#3498DBP�  � ��$
 True��. ���  � �� �#1ABC9C��  � �� �#16A085�� " ��.���  � ��$	 250����  � ��$ 45����  � ��$	 0,8q���  � ��$ 16(���  � ��$ Medium"���  � �� �#2C3E50��  � ��
 �������  � �� �#34495E��  � ��$ 1q���  � �� �� " ��.R��� #��z��#��z��#��z��$ 5��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ���  � �� �#34495EP�  � ��$
 True��. ���  � �� �#1ABC9C�� " ��.���  � ��
 �������  � ��$ 12(���  � �� �	#80000000��  �@��Padding$
 10,5q���  �@J�Effect   $ Black��$ 2Q�$ 1Q�.��� .+��� -    ��Name$ BackgroundImage��$��UniformToFill=� K� ZIndex$ 0������ *CurrentBackgroundImage+�� -   $ CharacterImage��$��Uniform=�$ 1��$��Center=�$��Bottom=�$��	0,0,0,200q����� *CurrentCharacterImage+�� -    ��Name$ DialogueBox��#��[  $��Bottom=�$ 2��y��� *DialogueBoxVisibility+._��� .+��� -    ��Name$ SpeakerName��#��[ ���� *CurrentSpeaker+y��� *SpeakerNameVisibility+�� -   $ DialogueText��#��[ ���� *CurrentDialogue+�� #��[ ���� *ContinuePrompt+y��� *ContinuePromptVisibility+�� -     ��Name$  ChoicesPanel��$��Center=�$��Center=�$ 3��$	��800��y��� *ChoicesVisibility+.+��� ���� *CurrentChoices+���� .R��� #��[ ���� *Text+���� *SelectCommand+! J� Effect!   $ Black��$ 5Q�$ 2Q�$	 0.7Q�� -   " �Name$" GameMenu�����	#E0000000$ 10��y��� *GameMenuVisibility+.+��� ���#2C3E50$ 10��# �� Padding$# 30q�$��Center=�$��Center=����#34495E$��2q�._��� .+��� $��게임 메뉴��$��28(�$
��Bold"�
�������$��Center=�$��0,0,0,25q��� $��게임으로 돌아가기��#��[ ���� *ResumeGameCommand+�� $��저장하기��#��[ ���� *SaveGameCommand+�� $��불러오기��#��[ ���� *LoadGameCommand+�� $��설정��#��[ ���� *ShowSettingsCommand+�� $��타이틀로 돌아가기��#��[ ���� *ReturnToTitleCommand+� -	   $" 	UIOverlay��$	��Top=�$��Right=�$��20q�$ 5��.+��� $ �� Orientation$$ 
Horizontal=�.+��� #��[ ���� *CurrentStoryPosition+�� #��[ 
��� ���$��5,0,0,0q����� *AutoPlayStatus+y��� *AutoPlayVisibility+�� -
   % ��Name$% 	ClickArea��
������ $ 1��y��� *DialogueBoxVisibility+
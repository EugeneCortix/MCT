   M S B A M L   `   `   `  ���� >  :SKT, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SKTSKT      SKT.Inverse   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:SKT       � Title$  Inverse problem��$	��500��$	��800��.��� -    �Name$ InverseWindow�� � ColumnDefinitions �� $
��340* ��� $
��131* � � RowDefinitions �� $
��357* ��� $
��127* �.+��� -    ��Name$ 
graphImage��$
��Fill=�$��0��$��0��$
��Left=�$	��300��$��	20,20,0,0q�$	��Top=�$	��500���� -    ��Name$ fieldButtonInv��$��1��$��Build the field��$��Center=�$	��Top=�$	��100��$��20��$��	0,145,0,0q��� $��1��$��Field X:��$
��Left=�$��	33,21,0,0q�$	��Top=�$��56��$��23���� -    ��Name$ xValInv��$��1��$
��Left=�$��23��$��
112,21,0,0q� �� TextWrapping$
 Wrap=�$��10��$	��Top=�$��73���� $��1��$��Field Z:��$
��Left=�$��	33,46,0,0q�$	��Top=�$��56��$��23���� -   $ yValInv��$��1��$
��Left=�$��23��$��
112,46,0,0q�$
 Wrap=�$��10��$	��Top=�$��73���� $��1��$��Columns:��$
��Left=�$��	33,83,0,0q�$	��Top=� J� RenderTransformOrigin$ 1.677,-7.316&�$��60��$��23���� -   $ 	xCrushInv��$��1��$
��Left=�$��23��$��
112,83,0,0q�$
 Wrap=�$��4��$	��Top=�$��73���� $��1��$��Rows:��$
��Left=�$��
33,107,0,0q�$	��Top=�$��60��$��23���� -   $ 	yCrushInv��$��1��$
��Left=�$��23��$��112,107,0,0q�$
 Wrap=�$��4��$	��Top=�$��73��&   System.Windows.Controls.DataGrid  -   	  Name$	 
GridOfWInv��$��2��$��577,220,15,2q��� -	   $ CALC_P��$��1��$��Calculate P...��$
��Left=�$��
120,57,0,0q�$��1��$	��Top=�$��20��$��88���� -
   $ alfa_regular_tBox��$
��Left=�$��
120,10,0,0q�$��1��$
 Wrap=�$��0��$	��Top=�$��23��$��88��$��1���� $��1��$!��Регуляризация:��$
��Left=�$��21,9,0,0q�$��1��$	��Top=��� -   $ Graph��$��График��$
��Left=�$��	25,57,0,0q�$��1��$	��Top=�$��1��
   M S B A M L   `   `   `  ����H  DExamClientWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ExamClientWPFExamClientWPF  5   	       ExamClientWPF.ExamWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -localclr-namespace:ExamClientWPF     5   	     � Title$  Exam Window��5   	   $	��450��6   $	��600��6*   .��5      � .+�5   
   �� -   5         ��Name$ txtTimeLeft��5       �� TextAlignment$ Center=�6)   $��20(�6@   $	��Top=�6N   $��0,10,0,0q�6f   �� 5	   
   $��Center=�6   $��Center=�60   $	��420��6M   .+�5
      �� -   5         ��Name$ txtQuestion��5
      $��17(�6)   �� 5      $��3��6    �� Orientation$ 
Horizontal=�6&   $��0,0,0,5q�6?   .+�5      	� -   5         	�Name$ 
radAnswer0��5      $��Center=�62   $��0,0,10,0q�6M    	� 	GroupName$ AnswerGroup��6_   �� 5      -   5        $ 
lblAnswer0��5      $��0,0,0,5q�6,   $	��380��6=   $��30��6I   $��Center=�6U   $��14(�6w   5      �� 5      $��4��6   $ 
Horizontal=�6&   $��0,0,0,5q�6?   .+�5      	� -   5        $ 
radAnswer1��5      $��Center=�62   $��0,0,10,0q�6M   $ AnswerGroup��6_   �� 5      -   5        $ 
lblAnswer1��5      $��0,0,0,5q�6,   $	��380��6=   $��30��6I   $��Center=�6U   $��14(�6w   5      �� 5      $��5��6   $ 
Horizontal=�6&   $��0,0,0,5q�6?   .+�5      	� -   5        $ 
radAnswer2��5      $��Center=�62   $��0,0,10,0q�6M   $ AnswerGroup��6_   �� 5      -   5        $ 
lblAnswer2��5      $��0,0,0,5q�6,   $	��380��6=   $��30��6I   $��Center=�6U   $��14(�6w   5      �� 5      $��6��6   $ 
Horizontal=�6&   $��0,0,0,5q�6?   .+�5      	� -	   5        $ 
radAnswer3��5      $��Center=�62   $��0,0,10,0q�6M   $ AnswerGroup��6_   �� 5      -
   5        $ 
lblAnswer3��5      $��0,0,0,5q�6,   $	��380��6=   $��30��6I   $��Center=�6U   $��14(�6w   5      5      �� 5   
   $��Bottom=�6   $��Center=�60   .+�5      �� $ 
Horizontal=�6   .+�5      �� -   5         ��Name$ btnPrev��5      $��Previous��6*   $��16(�6=   $��10 0 0 0q�6K   $	��100��6]   6i   �� 5       -   5        $ btnNext��5       $
��Next��6*   $��16(�69   $��10 0 0 0q�6G   $	��100��6Y   6e   5!      �� 5"      -   5        $��Submit��5"      $��16(�6&   $��10q�64   $	��210��6@   6L   5#      5$      5%      
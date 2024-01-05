Feature: ProbabilityTest

# С ростом количества измерений отклонение от 50/50 должно снижаться
Scenario: Probability should decrease deviation with measurments amount grouth
	Given System has quantums
	| Name | Angle |
	| A    | 45    |
	When Define deviation from probability 1/2 after 100 measurments of quantum 'A'	
	And Define deviation from probability 1/2 after 10000 measurments of quantum 'A'
	Then Measurment deviation after 10000 measurments of quantum 'A' is less than deviation after 100 measurments of quantum 'A'

# Технически если система при вероятности 1/2 выдает постоянно значения подсчета false < 0.5 но близкое к нему (0.495 например) и значения подсчета true > 0.5 но близкое к нему (0.505)
# то предыдущий тест будет пройден. Но при этом вероятность не будет равномерной. Система будет 'наваливать' на одну сторону (всегда боьлше true исходов чем false или наоборот).
# Нужно что бы выход true и false подсчетов был примерно одинаковым
Scenario: Probability should be distributed uniformly
	Given System has quantums
		| Name | Angle |
		| A    | 45    |
	When Collect side results of 10000 measurment sets of qunat 'A' size of 100 measurments
	Then Balance deviation should not exceed 5 percents 
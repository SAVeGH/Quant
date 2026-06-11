Feature: EntangledStatesTest

Тесты проверяют выполняются ли состояния Бэлла, GHZ и W
Такие состояния нельзя получить тензорным умножением матриц

# состояния Бэлла:
# 1/sqrt(2)00> + 1/sqrt(2)11>
# 1/sqrt(2)00> - 1/sqrt(2)11> 
# 1/sqrt(2)01> + 1/sqrt(2)10>
# 1/sqrt(2)01> - 1/sqrt(2)10>
Scenario: Bell state
	Given System has quantums
	| Name | Angle		  |
	| A    | <A_Angle>    |
	| B    | <B_Angle>    |
	And Quantums 'A' and 'B' are entangled
	When Measure to '<mValue>' quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	Then Measurment result of quantum 'A' is '<MeasurmentResult>' to measurment result of quantum 'B'

Examples:
| A_Angle | B_Angle | MeasurmentResult | mValue  |
| 90      | 90      | match            | true    |
| 270     | 270     | match            | true    |
| 90      | 270     | opposed          | true    |
| 270     | 90      | opposed          | true    |
| 90      | 90      | match            | false   |
| 270     | 270     | match            | false   |
| 90      | 270     | opposed          | false   |
| 270     | 90      | opposed          | false   |

#1/sqrt(2)000> + 1/sqrt(2)111>
Scenario: GHZ state
Given System has quantums
	| Name | Angle		  |
	| A    | <A_Angle>    |
	| B    | <B_Angle>    |
	| C    | <C_Angle>    |
	And Quantums 'A' and 'B' are entangled
	And Quantums 'B' and 'C' are entangled
	And Quantums 'A' and 'C' are entangled
	# связи из A <-> B <-> C <-> A
	# преобразуются в A -> B -> C -> A
	And Ringify 'A'
	When Measure to '<mValue>' quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	And Measure quantum 'C' in basis 0
	Then Measurment result of quantum 'A' is '<MeasurmentResult>' to measurment result of quantum 'B'
	And Measurment result of quantum 'B' is '<MeasurmentResult>' to measurment result of quantum 'C'

Examples:
| A_Angle | B_Angle | C_Angle | MeasurmentResult | mValue  |
| 90      | 90      | 90      | match            | true    |
| 270     | 270     | 270     | match            | true    |
| 90      | 90      | 90      | match            | false   |
| 270     | 270     | 270     | match            | false   |


# один из вариантов W состояния
#1/sqrt(3)001> + 1/sqrt(3)010> + 1/sqrt(3)100>
# угол 70.5288 это апмлитуда вероятности 0.57735 = sin(35.2644) (35.2644 = 70.5288 / 2), что является sqrt(0.3333...) т.е. вероятность получения 1 = 1/3
# Если 'A' измерится в 0 то 'B','C' станут 90 градусов, что дает вероятность 'B' и 'C' 1 = 1/2, если в 1 то 'B','C' станут 0 градусов 
# (угол пройденный 'A' 109.4712 (2/3 вероятности) поделится на 2 (количество связей), инвертируется и добавится к положению 'B' и 'C' (по -1/3). Вероятность 1 для 'B' и 'C' станет 0)
# Если 'B','C' стали 90 градусов, то измерение 'B' в 1 сбросит 'C' в 0 и наоборот.
# Таким образом в 1/3 случаев получим 'A' = 1 и 'B','C' = 0, а в оставшихся случаях 'A' = 0 и 'B','C' поделят пополам состояния 01 и 10.
# Начинать мерять можно с любого кванта - результат будет симметричным.
Scenario: W state
Given System has quantums
	| Name | Angle     |
	| A    | <A_Angle> |
	| B    | <B_Angle> |
	| C    | <C_Angle> |
	And Quantums 'A' and 'B' are entangled inverse
	And Quantums 'B' and 'C' are entangled inverse
	And Quantums 'A' and 'C' are entangled inverse
	When Measure to '<mValue>' quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	And Measure quantum 'C' in basis 0
	Then Measurment result corresponds to W state
Examples:
| A_Angle | B_Angle | C_Angle | mValue  |
| 70.5288 | 70.5288 | 70.5288 | true    |
| 70.5288 | 70.5288 | 70.5288 | false   |

# Начальное состояние квантов: A - 1/2 вероятности 1, B - 1/6 вероятности 1 и 5/6 вероятности 0. Соотношение квантов (Values) 1(A) : 3(B)
# Если начать измерение с А: 
# - если 'A' измерится в 0 (вероятность изменится на +1/2) то 'B' станет 0 (вероятность изменится на + 1/6 ( (1/2)/3.0 (Value B))), 
# - если 'A' измерится в 1 (вероятность изменится на -1/2) то вероятность 0 для B станет 1/3 (5/6 - (1/2)/3.0)
# Если начать измерение с B:
# - если 'B' измерится в 0 (вероятность изменится на +1/6) то 'A' станет 0 (вероятность изменится на +1/2 ( (1/6) * 3.0 (Value B)))
# - если 'B' измерится в 1 (вероятность изменится на -5/6) то 'A' станет 1 (вероятность изменится на -2.5 ( (-5/6) * 3.0 (Value B) = -15/6 ))
# т.е. вектор А совершит 1 полный оборот и еще 1/4 оборота по часовой стрелке (-) (полный оборот в вероятностях +/- 2.0) и ляжет на линию измерения
# в напрвлении 1.
# Таким образом система никогда не переходит в состояние A0 B1
Scenario: A0B1 state does not exists
Given System has quantums
	| Name | Angle     | Value     |
	| A    | <A_Angle> | <A_Value> |
	| B    | <B_Angle> | <B_Value> |
	And Quantums 'A' and 'B' are entangled
	When Measure to '<mValue>' quantum '<mFirstName>' in basis 0
	And Quantum '<mSecondName>' is measured 100 times in 0 basis
	And Measure quantum '<mSecondName>' in basis 0
	Then Quantum '<mSecondName>' probability corresponds to <secondMeasurmentUnityProbability> with deviation of <deviation>	
	And State 'A' is 0 and 'B' is 1 does not exists
Examples:
| A_Angle | A_Value | B_Angle | B_Value | mValue | mFirstName | mSecondName | secondMeasurmentUnityProbability | deviation |
| 90.0    | 1.0     | 48.1897 | 3.0     | false  | A          | B           | 0.0                              | 0         |
| 90.0    | 1.0     | 48.1897 | 3.0     | true   | A          | B           | 0.33333333                       | 5         |
| 90.0    | 1.0     | 48.1897 | 3.0     | false  | B          | A           | 0.0                              | 0         |
| 90.0    | 1.0     | 48.1897 | 3.0     | true   | B          | A           | 1.0                              | 0         |





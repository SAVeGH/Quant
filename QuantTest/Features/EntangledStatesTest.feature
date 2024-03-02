Feature: EntangledStatesTest

Тест проверяет выполняются ли состояния Бэлла, GHZ и W
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
	When Measure quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	Then Measurment result of quantum 'A' is '<MeasurmentResult>' to measurment result of quantum 'B'

Examples:
| A_Angle | B_Angle | MeasurmentResult |
| 45      | 45      | match            |
| 315     | 315     | match            |
| 45      | 135     | opposed          |
| 45      | 315     | opposed          |

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
	And Ringify 'A'
	When Measure quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	And Measure quantum 'C' in basis 0
	Then Measurment result of quantum 'A' is '<MeasurmentResult>' to measurment result of quantum 'B'
	And Measurment result of quantum 'B' is '<MeasurmentResult>' to measurment result of quantum 'C'

Examples:
| A_Angle | B_Angle | C_Angle | MeasurmentResult |
| 45      | 45      | 45      | match            |
| 315     | 315     | 315     | match            |


# один из вариантов W состояния
#1/sqrt(3)001> + 1/sqrt(3)010> + 1/sqrt(3)100>
# угол 35.2644 это апмлитуда вероятности 0.57735 = sin(35.2644), что является sqrt(0.3333...) т.е. вероятность получения 1 = 1/3
# Если 'A' измерится в 0 то 'B','C' станут 45 градусов, что дает вероятность 'B' и 'C' 1 = 1/2, если в 1 то 'B','C' станут 0 градусов 
# (угол пройденный 'A' 54,7356 поделится на Scale (-2), инвертируется и добавится к положению 'B' и 'C'. Вероятность 1 для 'B' и 'C' = 0)
# Если 'B','C' стали 45 градусов, то измерение 'B' в 1 сбросит 'C' в 0 и наоборот.
# Таким образом в 1/3 случаев получим 'A' = 1 и 'B','C' = 0, а в оставшихся случаях 'A' = 0 и 'B','C' поделят пополам состояния 01 и 10.
Scenario: W state
Given System has quantums
	| Name |  Angle		   |
	| A    |  <A_Angle>    |
	| B    |  <B_Angle>    |
	| C    |  <C_Angle>    |
	And Quantums 'A' and 'B' are entangled
	And Quantums 'B' and 'C' are entangled
	And Quantums 'A' and 'C' are entangled
	When Measure quantum 'A' in basis 0
	And Measure quantum 'B' in basis 0
	And Measure quantum 'C' in basis 0
	Then Measurment result corresponds to W state
Examples:
| A_Angle      | B_Angle      | C_Angle      |
| 35.2644      | 35.2644      | 35.2644      |




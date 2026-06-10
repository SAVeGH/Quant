Feature: MeasurmentTests

Измерения кваната под разными углами

Scenario: Measurment must give correct propability 
	Given System has quantums
	| Name | Angle       |
	| A    | <qangle>    |
	When Quantum 'A' is measured 10000 times in <mangle> basis
	Then Quantum 'A' probability corresponds to <uprobalility> with deviation of <deviation>

Examples: 
| qangle | mangle | uprobalility | deviation |
| 0      | 0      | 0            | 0         |
| 90     | 0      | 0.5          | 10        |
| 180    | 0      | 1            | 0         |
| 270    | 0      | 0.5          | 10        |
| 360    | 0      | 0            | 0         |
| 0      | 90     | 0.5          | 10        |
| 90     | 90     | 0            | 0         |
| 180    | 90     | 0.5          | 10        |
| 270    | 90     | 1            | 0         |
| 360    | 90     | 0.5          | 10        |
| 120    | 0      | 0.75         | 10        |
| 240    | 0      | 0.75         | 10        |
| 0      | 120    | 0.75         | 10        |
| 240    | 120    | 0.75         | 10        |
| 0      | 240    | 0.75         | 10        |
| 120    | 240    | 0.75         | 10        |



Scenario: Measurment must give correct angle 
	Given System has quantums
	| Name | Angle       |
	| A    | <qangle>    |
	When Quantum 'A' is measured to '<measureResult>' in <measureAngle> basis
	Then Quantum 'A' angle corresponds to <resultAngle>

Examples: 
| qangle    | measureAngle | measureResult | resultAngle |
| 0         | 0            | false         | 0           |
| 70.5288   | 0            | false         | 0           |
| 70.5288   | 0            | true          | 180         |
| 90.0      | 0            | false         | 0           |
| 90.0      | 0            | true          | 180         |
| 109.4712  | 0            | false         | 0           |
| 109.4712  | 0            | true          | 180         |
| 180.0     | 0            | true          | 180         |
| 250.5288  | 0            | false         | 0           |
| 250.5288  | 0            | true          | 180         |
| 270.0     | 0            | false         | 0           |
| 270.0     | 0            | true          | 180         |
| 289.4712  | 0            | false         | 0           |
| 289.4712  | 0            | true          | 180         |
| 360       | 0            | false         | 0           |

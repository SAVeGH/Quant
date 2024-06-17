Feature: MeasurmentTests

Измерения кваната под разными углами

Scenario: Measurment must give correct propability 
	Given System has quantums
	| Name | Angle       |
	| A    | <qangle>    |
	When Quantum 'A' is measured 10000 times in <mangle> basis
	Then Probability corresponds to <uprobalility> with deviation of <deviation>

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

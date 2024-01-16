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
| 45     | 0      | 0.5          | 10        |
| 90     | 0      | 1            | 0         |
| 135    | 0      | 0.5          | 10        |
| 180    | 0      | 0            | 0         |
| 225    | 0      | 0.5          | 10        |
| 270    | 0      | 1            | 0         |
| 315    | 0      | 0.5          | 10        |
| 360    | 0      | 0            | 0         |
| 0      | 90     | 0.5          | 10        |
| 45     | 90     | 0            | 0         |
| 90     | 90     | 0.5          | 10        |
| 135    | 90     | 1            | 0         |
| 180    | 90     | 0.5          | 10        |
| 225    | 90     | 0            | 0         |
| 270    | 90     | 0.5          | 10        |
| 315    | 90     | 1            | 0         |
| 360    | 90     | 0.5          | 10        |
| 0      | 120    | 0.75         | 10        |
| 0      | 240    | 0.75         | 10        |
| 90     | 120    | 0.25         | 10        |
| 90     | 240    | 0.25         | 10        |

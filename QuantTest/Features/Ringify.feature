Feature: Ringify

A short summary of the feature

Scenario: Ringify three quants
	Given System has quantums
		| Name | Angle |
		| A    | 0     |		
		| B    | 0     |
		| C    | 0     |
	And Quantum 'A' entangled with quantum 'B' with level 'None'
	And Quantum 'B' entangled with quantum 'C' with level 'None'
	And Quantum 'C' entangled with quantum 'A' with level 'None'
	When Ringify 'A' with level 'Recursive'
	Then Quantum 'A' has one way reference to quantum 'B'
	And Quantum 'B' has one way reference to quantum 'C'
	And Quantum 'C' has one way reference to quantum 'A'
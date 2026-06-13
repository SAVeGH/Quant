Feature: Deutch

Проверка алгоритма Дойча

Background:
	Given System has quantums
	| Name | Angle |
	| X    | 0.0   |
	| Y    | 0.0   |

Scenario: Deutch circuit
	Given Gate H is applied to quantum 'X'
	And Gate X is applied to quantum 'Y'
	And Gate H is applied to quantum 'Y'
	And Quantums 'X' and 'Y' are entangled
	When Run Deutch circuit with quantum X as parameter and <functionType> with <outputType>
	Then Circuit output corresponds to <functionType>
	Examples:
    | functionType | outputType |
	# константная функция, которая всегда возвращает false
    | constant     |  false     | 
	# константная функция, которая всегда возвращает true
	| constant     |  true      |
	# сбалансированная функция, которая всегда возвращает переданное значение (true -> true, false -> false)
	| balanced     |  id        |
	# сбалансированная функция, которая всегда возвращает обратное переданному значение (true -> false, false -> true)
	| balanced     |  not       |
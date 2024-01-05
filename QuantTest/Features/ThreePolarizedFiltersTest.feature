Feature: ThreePolarizedFiltersTest

Прохождение потоком квантов трёх поляризационных фильтров
1. Половина потока квантов проходит первый фильтр пропускающий кванты в состоянии 0 (кванты пришедшие на фильтр в произвольном состоянии измеряются на фильтре
и переходят в состояние 0 или 1 относительно базиса, но проходят фильтр только те, что ориентированы так же как поляризация фильтра), 
при этом второй фильтр пропускающий кванты в состоянии 1 блокирует поток
2. Все кванты прошедшие первый фильтр в состоянии 0 пройдут и второй фильтр пропускающий так же в состоянии 0
3. Половина начального потока проходит первый фильтр пропускающий кванты в состоянии 0. Половина из прошедших первый фильтр
проходит и второй фильтр т.к. он установлен под 45 градусов и на нем квант может быть измерян в 1 или 0 с вероятностью 1/2. Половина прошедших второй фильтр
пройдет и третий фильтр т.к. по отношению к нему прошедшие второй фильтр находятся в состоянии 1/2. Таким образом все 3 фильтра пройдет 1/8 часть потока.


Scenario: Do not pass two filters
	Given System has quantums stream of 100000 quantums with arbitrary state
	# первый фильтр пропускающий кванты в состоянии 0
	And Quantums stream fall on polarization filter set up with angle 0
	# второй фильтр пропускающий кванты в состоянии 1
	And Quantums stream fall on polarization filter set up with angle 90
	When Detection after the filters is performed
	# второй фильтр не пропускает кванты пришедшие в состоянии 0
	Then There is 0 part of stream is passed with deviation of 0

Scenario: Pass two filters
	Given System has quantums stream of 100000 quantums with arbitrary state
	# первый фильтр пропускающий кванты в состоянии 0 (проходит половина начального потока)
	And Quantums stream fall on polarization filter set up with angle 0
	# второй фильтр пропускающий кванты так же в состоянии 0 (проходят все т.к. уже измеряны в 0 первым фильтром)
	And Quantums stream fall on polarization filter set up with angle 0
	When Detection after the filters is performed
	# второй фильтр проходит половина начального потока
	Then There is 0.5 part of stream is passed with deviation of 10

Scenario: Pass three filters
	Given System has quantums stream of 100000 quantums with arbitrary state
	And Quantums stream fall on polarization filter set up with angle 0
	And Quantums stream fall on polarization filter set up with angle 45
	And Quantums stream fall on polarization filter set up with angle 90
	When Detection after the filters is performed
	# 1/8 часть начального потока пройдет через третий фильр
	Then There is 0.125 part of stream is passed with deviation of 10
Feature: Ekkert

Проверка работы протокола Эккерта для передачи ключей

@Ekkert
Scenario: Ekkert protocol key transmittion without intercept
Given Alice generates 3n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums streams
When Alice sends quantums stream to Bob
And 'Alice' measure own quantums stream with chosen basises for each quantum
And 'Bob' measure own quantums stream with chosen basises for each quantum
And Alice and Bob compares their basises in unencripted form
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
And Alice and Bob compares key bits where basises differ in unencripted form
Then Compared key bits with different basises matches in 1/4 cases with deviation 3
And Alice and Bob keys are identical

@Ekkert
Scenario: Ekkert protocol key transmittion with intrusion
Given Alice generates 3n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums streams
When Alice sends quantums stream to Bob
# На этапе перехвата у Евы нет информации о базисах Алисы и Боба
# Есть только поток квантов переданных Алисой. Закэшировать (клонировать) кванты до получения 
# информации о базисах она то же не может. Поэтому лучшее что она может сделать - это
# сгенерировать свою последовательноть базисов и измерить кванты. 
And 'Eva' chose basis for each bit in the key
And Eva intercepts transmittion
And 'Alice' measure own quantums stream with chosen basises for each quantum
And 'Bob' measure own quantums stream with chosen basises for each quantum
And Alice and Bob compares their basises in unencripted form
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
And Alice and Bob compares key bits where basises differ in unencripted form
Then Compared key bits with different basises matches in 3/8 cases with deviation 3
And Alice and Bob keys are not identical

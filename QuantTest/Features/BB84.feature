﻿Feature: BB84

Проверка работы протокола передачи ключей BB84

@BB84
Scenario: BB84 key transmittion without intercept
Given Alice generates 4n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums stream
When Alice sends quantums stream to Bob
And Alice and Bob compares their basises in unencripted form
And Bob measure quantums stream with chosen basises for each quantum
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
And Alice and Bob compares one half of their key bits in unencripted form
Then Compared key bits are identical
And Alice and Bob keys are identical

@BB84
Scenario: BB84 key transmittion with intrusion
Given Alice generates 4n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums stream
When Alice sends quantums stream to Bob
# На этапе перехвата у Евы нет информации о базисах Алисы и Боба
# Есть только поток квантов переданных Алисой. Закэшировать (клонировать) кванты до получения 
# информации о базисах она то же не может. Поэтому лучшее что она может сделать - это
# сгенерировать свою последовательноть базисов и измерить кванты. 
And 'Eva' chose basis for each bit in the key
And Eva intercepts transmittion
And Alice and Bob compares their basises in unencripted form
And Bob measure quantums stream with chosen basises for each quantum
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
And Alice and Bob compares one half of their key bits in unencripted form
Then Compared key bits are not identical and differ for 1/4 with deviation of 10
And Alice and Bob keys are not identical
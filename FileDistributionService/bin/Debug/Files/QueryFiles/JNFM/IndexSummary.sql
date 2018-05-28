SELECT 
LongName as IndexName,
IndexPrice as Last,
(IndexPrice-OpeningPrice) as Change,
case when (IndexPrice-OpeningPrice) <> 0 then ((IndexPrice-OpeningPrice)/OpeningPrice) * 100  else 0 end as PercentageChange,
Low as DayLow, 
High as DayHigh
FROM [dbo].[VW_JseIndexHighLows]
WHERE TradeDate = convert(date,getdate())
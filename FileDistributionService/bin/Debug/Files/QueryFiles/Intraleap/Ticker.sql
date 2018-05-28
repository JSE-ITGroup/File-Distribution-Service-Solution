	SELECT		  
		
		SymbolCode,
		LastTradedQuantity,
		LastTradedPrice,
		PriceChange,
		PercentageChange,
		High,
		Low,
		TotalVolumeTraded,
		[Close],
		LastTradeDate,
		[Last Update]		
  FROM [dbo].[VW_JSEGetTicker]
  WHERE CONVERT(DATE,[LastTradeDate]) = CONVERT(DATE, GETDATE())
   AND High > 0 and Low > 0
SELECT [SymbolCode]     
      ,[LastTradedQuantity]   as    VolumeTraded
      ,[LastTradedPrice] as Price
      ,[PriceChange]
      ,[PercentageChange]
      ,[LastTradeDate]
      ,[Last Update]
  FROM [dbo].[VW_JSEGetTicker]

				  
				  
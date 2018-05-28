SELECT           
				SymbolCode,  
                TotalVolumeTraded as 'VolumeTraded',  
				--LastTradedQuantity
                LastTradedPrice as 'Price',  
                PriceChange,
                PercentageChange,  
            --    LastTradeDate,
                  case 	
		           when PercentageChange > 0 then 'up'   
				   when PercentageChange = 0 then 'none'
                   else 'down' end as 'Movement',  
                convert(varchar(50), [Last Update],120) as [Last Update]
                 FROM [dbo].[VW_JSEGetTicker]  
         WHERE High> 0 and Low > 0
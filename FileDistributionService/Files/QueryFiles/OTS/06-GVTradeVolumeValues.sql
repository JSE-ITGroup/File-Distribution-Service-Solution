

-----------------------------------------------------------------------------------------------------------
use GVREPOSITORY
----------------------------------------------------------------------------------------------------------

SELECT
	cast( COUNT(*) as bigint) as NoOfTrades
	,TradingPlatform
	,CASE 
		WHEN Action='Sell' THEN BuyCompany 
			ELSE SellCompany
		END AS Broker
	,case 
		when MARKET=518 then 'Junior Market' 
		when MARKET=510 then 'Main Market'
		when MARKET=519 then 'Junior Block'
		when MARKET=513 then 'Main Block'		
		else '' end as Market
	,SUM(cast(VOLUME as decimal(18,3)) ) as Volumne
	,cast( SUM(VALUE) as decimal(18,4)) as Value
	,DateName(mm,TDATE) Month
	,Month(TDATE) MonthNumber
	,Year(TDATE) as Year
FROM

 (
SELECT 	
        case when lower(InitiatorTrader)   LIKE '%online%'  then 'OTS' ELSE 'GV' END AS TradingPlatform,
		CONVERT(date, DateTime) as TDATE,
		rtrim(cast(b.InstCode as CHAR(12))) as SYMBOL,
		case when upper(ManualDeal) = 'TRUE' and PriceSetting = 'false' and upper(b.[Parent ]) = 'JUNIOR' then 519
			 when upper(ManualDeal) = 'TRUE' and PriceSetting = 'false' and upper(b.[Parent ]) <> 'JUNIOR' then 513
		     when PriceSetting = 'true' and upper(b.[Parent ]) = 'JUNIOR' then 518
			 when PriceSetting = 'true' and upper(b.[Parent ]) <> 'JUNIOR' then 510
			 end as MARKET,
		Volume as VOLUME, 
		cast(Price as decimal(12,3)) as TPRICE,
		cast(TradeID as Char(8)) as TICKET,
		cast(null as char(8)) as SETTDATE,
		AggressorAction as Action,
		cast(case when AggressorAction = 'Buy' then  PersistentOrderID2 else
					 PersistentOrderID1 end as char(8)) as BUYORD,
				  cast(case when AggressorAction = 'Buy' then AggressorCompany
			 when InitiatorAction = 'Buy'  then InitiatorCompany
			 else '' end as varchar(100))as BuyCompany,

		     AggressorCompany ,
			 InitiatorCompany ,			
		rtrim(case when AggressorAction = 'Buy' then 
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = AggressorCompanyID) else
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = InitiatorCompanyID) end ) 
					as BBROKERNO,				  
		cast(case when AggressorAction = 'Sell' then  PersistentOrderID2 else
					 PersistentOrderID1 end as Char(8)) as SELORD,

			 cast(case when AggressorAction = 'Sell' then AggressorCompany
			 when InitiatorAction = 'Sell'  then InitiatorCompany
			 else '' end as varchar(100))as SellCompany,


		rtrim(case when AggressorAction = 'Sell' then
		(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = AggressorCompanyID) else
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = InitiatorCompanyID) end ) 
					 as SBROKERNO,
		cast((Price * Volume)as decimal(18,6)) as VALUE,
	    case when Action = 'Remove' then 'C' else '' end as CANCEL,    
		case when b.[Parent ] <> 'US$' then 'JMD'
		 else'USD' 
		 end as CURRENCY
	from 	Trades join
			(select	a.InstID,
	a.InstTypeID,
	a.InstCode,
	a.InstName,
	b.GroupID,
	c.GroupName,
	c.ParentGroupID,
	c.Flags,
	d.GroupName as "Parent " from GVGLOBAL80.dbo.Instruments a left join 
			(select max(GroupID) as GroupID,
				   InstID from GVGLOBAL80.dbo.InstTreeInstruments
				   group by InstID) b on
				a.InstID = b.InstID 	   left join GVGLOBAL80.dbo.InstTreeGroups c on
				b.GroupID = c.GroupID 
										   left join GVGLOBAL80.dbo.InstTreeGroups d on
				c.ParentGroupID = d.GroupID 
				where c.ParentGroupID in (22,31) 
				 ) b on (Trades.InstID = b.InstID)
	where	
		 ( 
		1=1
		--lower(AggressorTrader) LIKE '%online%' or
		--lower(InitiatorTrader)  NOT LIKE '%online%' --or  remove to exclude online trading
		 --lower(InitiatorUser)  LIKE '%online%'
		 )
		 
) as t
WHERE t.Action IN ('Sell','Buy')
and MARKET not IN (510,518)
AND MONTH(TDATE) = 5
AND Year(TDATE) = 2017			
--and TDATE IN ('2017-02-03','2015-02-01','2017-02-02')					
group by 
		CASE WHEN Action='Sell' THEN BuyCompany ELSE SellCompany END
		,MARKET		
		,DateName(mm,TDATE) 
		,Month(TDATE) 
		,Year(TDATE)
		,TradingPlatform
		
order by CASE WHEN Action='Sell' THEN BuyCompany ELSE SellCompany END		
		,Year
		,MonthNumber
		,MARKET
		,TradingPlatform

		/*


			when MARKET=518 then 'Junior Market' 
		when MARKET=510 then 'Main Market'

		when MARKET=519 then 'Junior Block'
		when MARKET=513 then 'Main Block'		

select
  distinct *
,case 
     when (select Account_Reference  from [dbo].[Accounts] where Account_Reference =a.AccountNumber) is null then 'New Account'
else 'Existing Account' end as AccountNumber
      from [dbo].UniqueAccountsJan as a
WHERE MONTH(DateRegistered) = 1
    and Year(DateRegistered) = 2017
	*/
<QUERY>
select 	CONVERT(VARCHAR(8), DateTime, 112) as TDATE,
		rtrim(cast(b.InstCode as CHAR(12))) as SYMBOL,
		case when upper(ManualDeal) = 'TRUE' and PriceSetting = 'false' and upper(b.[Parent ]) = 'JUNIOR' then 519
			 when upper(ManualDeal) = 'TRUE' and PriceSetting = 'false' and upper(b.[Parent ]) <> 'JUNIOR' then 513
			 when PriceSetting = 'true' and upper(b.[Parent ]) = 'JUNIOR' then 518
			 when PriceSetting = 'true' and upper(b.[Parent ]) <> 'JUNIOR' then 510
			 end as MARKET,
		Volume as VOLUME, 
		cast(Price as decimal(12,3)) as TPRICE,
		cast(TradeID as Char(8)) as TICKET,
/*		settlement_date as SETTDATE,	*/
		cast(null as char(8)) as SETTDATE,
		cast(case when AggressorAction = 'Buy' then OrderID1 else
					OrderID2 end as char(8)) as BUYORD,
		cast(case when AggressorAction = 'Buy' and AggressorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(BuyerAccount))
			 when InitiatorAction = 'Buy' and InitiatorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(BuyerAccount))
			 else '' end as Char(11)) as BUYACCT,
		rtrim(case when AggressorAction = 'Buy' then 
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = AggressorCompanyID) else
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = InitiatorCompanyID) end ) 
					as BBROKERNO,				  
		cast(case when AggressorAction = 'Sell' then OrderID1 else
					OrderID2 end as Char(8)) as SELORD,
		cast(case when AggressorAction = 'Sell' and AggressorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(SellerAccount)) 
			 when InitiatorAction = 'Sell' and InitiatorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(SellerAccount)) 
			 else '' end as Char(11))as SELLACCT,
		rtrim(case when AggressorAction = 'Sell' then
		(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = AggressorCompanyID) else
			(select CompanyCode from GVLOCAL80.dbo.Companies where CompanyID = InitiatorCompanyID) end ) 
					 as SBROKERNO,
		cast((Price * Volume)as decimal(12,3)) as VALUE,
	case when Action = 'Remove' then 'C' else '' end as CANCEL,    
	cast(null as CHAR(40)) as BBREF,
	cast(null as CHAR(40)) as SBREF,  
	cast(case when AggressorAction = 'Buy' and AggressorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(BuyerOrderRef))
			 when InitiatorAction = 'Buy' and InitiatorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(BuyerOrderRef))
			 else '' end as CHAR(12)) as BUYREF,
	cast(case when AggressorAction = 'Sell' and AggressorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(SellerOrderRef)) 
			 when InitiatorAction = 'Sell' and InitiatorCompany = 'Stocks & Securities Limited' then ltrim(rtrim(SellerOrderRef)) 
			 else '' end as CHAR(12)) as SELLREF
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
	where	(InitiatorCompany = 'Stocks & Securities Limited' or AggressorCompany = 'Stocks & Securities Limited')   and
		cast(DateTime as date) in ('<END_DATE>') 
			
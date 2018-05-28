select 	replace(NAME_TEXT,'???','') as Name,
		ACCOUNT_REFERENCE as Account,
		(select name_text from name as n where n.name_sequence =2 and n.NAME_ACCOUNT =a.BALTIME_ACCOUNT) as Jnt_Holder1,
		(select name_text from name as n where n.name_sequence =3 and n.NAME_ACCOUNT =a.BALTIME_ACCOUNT)  as Jnt_Holder2,
		(select name_text from name as n where n.name_sequence =4 and n.NAME_ACCOUNT =a.BALTIME_ACCOUNT)  as Jnt_Holder3,
		'' as Ref,
		DEPOPART_CODE as Member, 
		ISIN_SHORT_NAME as Symbol,
		cast(BALTIME_BALANCE as Bigint) as Balance,
		replace(cast(BALTIME_START as Date),'-','') as Date
	from  	baltime A join (select		BALTIME_ACCOUNT,
										BALTIME_ISIN,
										max(BALTIME_ID) as BALTIME_ID
								from 	BALTIME 
								where 	((cast(baltime_start as date)<=cast(getdate() as date)
										and cast(getdate() as date)<=cast(baltime_end as date)   
										))
									
								group by BALTIME_ACCOUNT,
									BALTIME_ISIN) B on A.BALTIME_ISIN = B.BALTIME_ISIN and A.BALTIME_ID = B.BALTIME_ID and
									A.BALTIME_ACCOUNT = B.BALTIME_ACCOUNT	
									join ACCOUNT on ACCOUNT_ID=A.BALTIME_ACCOUNT 
									join ISIN on ISIN_ID=A.BALTIME_ISIN  and ISIN_STATUS = 'O' and ISIN_CCY in ('USD')
									join DPACCESS on DPACCESS_ACCOUNT=ACCOUNT_ID and DPACCESS_OWNER='Y'
									join DEPOPART on DEPOPART_ID=DPACCESS_PART and DEPOPART_CODE = '3'
									left join NAME on ACCOUNT_ID = NAME_ACCOUNT and NAME_SEQUENCE = 1 
	where BALTIME_BALANCE > 0 
	order by ACCOUNT_OWNER_LIST		


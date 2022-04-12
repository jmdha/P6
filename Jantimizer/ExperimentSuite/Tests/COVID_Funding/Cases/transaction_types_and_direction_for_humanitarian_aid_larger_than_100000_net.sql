SELECT Ts.transaction_type, Fl.transaction_direction FROM 
	(transactions AS Ts JOIN flows AS Fl ON 
	 	Fl.reporting_org_id = Ts.reporting_org_id AND
	 	Ts.humanitarian = true AND
	 	Ts.net_money > 100000
	)
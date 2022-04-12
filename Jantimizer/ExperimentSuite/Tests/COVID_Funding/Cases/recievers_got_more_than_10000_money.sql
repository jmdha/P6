SELECT Fl.reciever_org_name, Ts.net_money FROM 
	(transactions AS Ts JOIN flows AS Fl ON 
	 	Fl.reporting_org_id = Ts.reporting_org_id AND
	 	Ts.net_money > 10000
	)
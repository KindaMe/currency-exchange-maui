﻿namespace api_access.Models;

public class ExchangeRateTableModel
{
    public string table { get; set; }
    public string currency { get; set; }
    public string code { get; set; }
    public string no { get; set; }
    public string effectiveDate { get; set; }
    public List<Rate> rates { get; set; }
    public string flagUrl { get; set; }
}

public class Rate
{
    public string currency { get; set; }
    public string code { get; set; }
    public string no { get; set; }
    public string effectiveDate { get; set; }
    public decimal mid { get; set; }
    public decimal bid { get; set; }
    public decimal ask { get; set; }
    public string flagUrl { get; set; }
}
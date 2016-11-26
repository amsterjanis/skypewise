using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkypeWise.Storage
{
    public class Transfer
    {
        public string UID { get; }
        string transferFrom;
        string transferTo;
        double ammount;
        string currency;
        string status;

        public Transfer(string transferFrom, string transferTo, double ammount, string currency = "$")
        {
            this.UID = "0";
            this.transferFrom = transferFrom;
            this.transferTo = transferTo;
            this.ammount = ammount;
            this.currency = currency;
            this.status = "New";
        }
    }
}